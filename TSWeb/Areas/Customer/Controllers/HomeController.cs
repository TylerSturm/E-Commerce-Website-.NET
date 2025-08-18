using TS.DataAccess.Repository.IRepository;
using TS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq; // Make sure to include this for LINQ operations
using System.Security.Claims;

namespace TSWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        // Updated Index method to handle sorting and filtering
        public IActionResult Index(string sortOrder, decimal? minPrice, decimal? maxPrice)
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");

            // Apply price filtering
            if (minPrice.HasValue)
            {
                // Convert minPrice to double for comparison
                productList = productList.Where(p => p.Price100 >= (double)minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                // Convert maxPrice to double for comparison
                productList = productList.Where(p => p.Price100 <= (double)maxPrice.Value);
            }

            // Apply sorting
            switch (sortOrder)
            {
                case "AlphabeticalAsc":
                    productList = productList.OrderBy(p => p.Title);
                    break;
                case "AlphabeticalDesc":
                    productList = productList.OrderByDescending(p => p.Title);
                    break;
                case "PriceAsc":
                    productList = productList.OrderBy(p => p.Price100);
                    break;
                case "PriceDesc":
                    productList = productList.OrderByDescending(p => p.Price100);
                    break;
                default:
                    // Default sorting can be applied here if necessary
                    productList = productList.OrderBy(p => p.Title); // Default to alphabetical ascending
                    break;
            }

            // Calculate and set the item count in ViewBag
            SetCartItemCount();

            return View(productList);
        }

        public IActionResult Details(int id)
        {
            // Fetch the product details
            var product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "Category");
            if (product == null)
            {
                return NotFound();
            }
            var cart = new ShoppingCart
            {
                Product = product,
                Count = 1,
                ProductId = id
            };

            // Set the cart item count for the details view
            SetCartItemCount();

            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            ShoppingCart existingCart = _unitOfWork.ShoppingCart.Get(
                u => u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);

            if (existingCart != null)
            {
                existingCart.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(existingCart);
            }
            else
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
            }

            _unitOfWork.Save();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Privacy()
        {
            // Set the cart item count for the privacy view
            SetCartItemCount();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Helper method to set the cart item count in ViewBag
        private void SetCartItemCount()
        {
            // You may need to implement logic to fetch the current user's shopping cart items
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IEnumerable<ShoppingCart> cartItems = _unitOfWork.ShoppingCart.GetAll(
                u => u.ApplicationUserId == userId); // Adjust according to your data retrieval logic

            ViewBag.ItemCount = cartItems?.Sum(item => item.Count) ?? 0; // Set the item count in ViewBag
        }
    }
}