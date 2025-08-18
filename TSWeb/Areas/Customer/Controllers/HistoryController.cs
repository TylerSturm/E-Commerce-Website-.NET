using TS.DataAccess.Repository.IRepository;
using TS.Models;
using TS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace TSWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class HistoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private void SetCartItemCount(string userId)
        {
            var cartItems = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId);
            ViewBag.ItemCount = cartItems?.Sum(item => item.Count) ?? 0;
        }

        public HistoryController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(string sortOrder)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            IEnumerable<OrderHeader> userOrderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");

            switch (sortOrder)
            {
                case "IdIncrease":
                    userOrderHeaders = userOrderHeaders.OrderBy(p => p.Id).ToList();
                    break;
                case "IdDecrease":
                    userOrderHeaders = userOrderHeaders.OrderByDescending(p => p.Id).ToList();
                    break;
                case "PriceIncrease":
                    userOrderHeaders = userOrderHeaders.OrderBy(p => p.OrderTotal).ToList();
                    break;
                case "PriceDecrease":
                    userOrderHeaders = userOrderHeaders.OrderByDescending(p => p.OrderTotal).ToList();
                    break;
                default:
                    // Default sorting can be applied here if necessary
                    userOrderHeaders = userOrderHeaders.OrderByDescending(p => p.Id).ToList(); 
                    break;
            }

            SetCartItemCount(userId);

            return View(userOrderHeaders);
        }

        public IActionResult Details(int id)
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id);
            if (orderHeader == null)
            {
                return NotFound();
            }

            var orderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderHeader.Id, includeProperties: "Product").ToList();
    
            if (!orderDetails.Any())
            {
                return NotFound();
            }

            var viewModel = new OrderDetailsVM
            {
                OrderHeader = orderHeader,
                OrderDetails = orderDetails
            };

            return View(viewModel); 
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        #endregion
    }
}