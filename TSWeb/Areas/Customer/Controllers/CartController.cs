using TS.DataAccess.Repository.IRepository;
using TS.Models;
using TS.Models.ViewModels;
using TS.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace TSWeb.Areas.Customer.Controllers
{

    [Area("customer")]
    [Authorize]
    public class CartController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private void SetCartItemCount(string userId)
        {
            var cartItems = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId);
            ViewBag.ItemCount = cartItems?.Sum(item => item.Count) ?? 0;
        }

        public IActionResult Index()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            
            SetCartItemCount(userId);

            return View(ShoppingCartVM);
        }


        public IActionResult Summary(string? paymentMethod)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            if (paymentMethod == "Card")
            {
                ShoppingCartVM.OrderHeader.surchargeAmount = ShoppingCartVM.OrderHeader.OrderTotal * .05;
                ShoppingCartVM.OrderHeader.OrderTotal += ShoppingCartVM.OrderHeader.surchargeAmount;
                ShoppingCartVM.OrderHeader.paymentMethod = "Card";
            }
            else
            {
                ShoppingCartVM.OrderHeader.surchargeAmount = 0;
                ShoppingCartVM.OrderHeader.paymentMethod = "ACH";
            }

            SetCartItemCount(userId);

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST(string? paymentMethod)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");

            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

            ApplicationUser ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            if (paymentMethod == "Card")
            {
                ShoppingCartVM.OrderHeader.surchargeAmount = ShoppingCartVM.OrderHeader.OrderTotal * .05;
                ShoppingCartVM.OrderHeader.OrderTotal += ShoppingCartVM.OrderHeader.surchargeAmount;
                ShoppingCartVM.OrderHeader.paymentMethod = "Card";
            }
            else
            {
                ShoppingCartVM.OrderHeader.paymentMethod = "ACH";
            }

            if (ApplicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            if (ApplicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                var domain = $"{Request.Scheme}://{Request.Host}/";
                var successUrl = Url.Action("OrderConfirmation", "Cart", new { area = "Customer", id = ShoppingCartVM.OrderHeader.Id }, Request.Scheme);
                var options = new SessionCreateOptions{};

                if (paymentMethod == "Card")
                {
                    options = new SessionCreateOptions
                    {
                        SuccessUrl = successUrl,
                        CancelUrl = domain + "Customer/Cart/Index",
                        PaymentMethodTypes = new List<string> { "card" },
                        LineItems = new List<SessionLineItemOptions>(), 
                        Mode = "payment"
                    };
                }
                else
                {
                    options = new SessionCreateOptions
                    {
                        SuccessUrl = successUrl,
                        CancelUrl = domain + "Customer/Cart/Index",
                        PaymentMethodTypes = new List<string> { "us_bank_account" },
                        LineItems = new List<SessionLineItemOptions>(), 
                        Mode = "payment"
                    };
                }

                foreach (var item in ShoppingCartVM.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                if (paymentMethod == "Card" && ShoppingCartVM.OrderHeader.surchargeAmount > 0)
                {
                    var surchargeLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(ShoppingCartVM.OrderHeader.surchargeAmount * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Convenience Fee (5%)"
                            }
                        },
                        Quantity = 1
                    };
                    options.LineItems.Add(surchargeLineItem);
                }

                var service = new SessionService();
                Session session = service.Create(options);
                _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                SetCartItemCount(userId);

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }

            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
        }

        public IActionResult OrderConfirmation(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id);

            // Retrieve cart items for the current user
            var cartItems = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).ToList();

            // If we have valid cart items and an existing order header
            if (cartItems.Count > 0 && orderHeader != null)
            {
                // Populate OrderItems from cart items
                foreach (var cartItem in cartItems)
                {
                    var product = _unitOfWork.Product.Get(cartItem.ProductId);
                    if (product != null)
                    {
                        var orderItem = new OrderItem
                        {
                            ProductId = cartItem.ProductId,
                            Quantity = cartItem.Count,
                            Price = GetPriceBasedOnQuantity(cartItem)
                        };

                        // Link the OrderItem to the OrderHeader
                        orderItem.OrderHeader = orderHeader;
                    }
                }

                // Remove the cart items after transferring them to the OrderHeader
                _unitOfWork.ShoppingCart.RemoveRange(cartItems);

                // Update the order header to mark it as completed
                orderHeader.OrderStatus = SD.StatusApproved;
                orderHeader.PaymentStatus = SD.PaymentStatusApproved;
                orderHeader.PaymentDate = DateTime.Now;
                orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now);
                orderHeader.ShippingDate = DateTime.Now.AddDays(3);

                // Save changes to the OrderHeader (including OrderItems) and shopping cart
                _unitOfWork.OrderHeader.Update(orderHeader);
                _unitOfWork.Save();
            }

            // Update cart item count (if this is relevant to your application logic)
            SetCartItemCount(userId);
            return View(id);
        }

        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if (cartFromDb.Count == 1)
            {
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
            }
            else
            {
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        private double GetPriceBasedOnQuantity(ShoppingCart ShoppingCart)
        {
            if(ShoppingCart.Count <= 50)
            {
                return ShoppingCart.Product.Price;
            }
            else if(ShoppingCart.Count <= 100)
            {
                return ShoppingCart.Product.Price50;
            }
            else
            {
                return ShoppingCart.Product.Price100;
            }
        }
    }
}