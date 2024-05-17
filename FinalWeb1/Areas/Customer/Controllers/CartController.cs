using FinalWeb1.DataAccess.Repository.IRepository;
using FinalWeb1.Models;
using FinalWeb1.Models.ViewModels;
using FinalWeb1.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.BillingPortal;
using Stripe.Checkout;
using System.Security.Claims;

namespace FinalWeb1.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
      
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
		[BindProperty] // This attribute is used to bind the ShoppingCartVM object to the view
		public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
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

            IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.Product.Id).ToList(); // get the product images
                cart.Price = GetPrice(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price );
            }

            return View(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
            // Get the user id from the claims
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Create a new ShoppingCartVM object
            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,includeProperties: "Product"), 
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            // Assign the user details to the order header
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            // Calculate the total price of the order
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPrice(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price );
            }
            return View(ShoppingCartVM);
        }

		[HttpPost]
		[ActionName("Summary")]
		public IActionResult SummaryPOST()
		{
            // Get user id from the claims
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Get the shopping cart list based on the user id
			ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
				includeProperties: "Product");

            // Set the order info - order date, user id
			ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
			ShoppingCartVM.OrderHeader.ApplicationUserId = userId;
			ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            
            // Set the price
			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{               
				cart.Price = GetPrice(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price );
			}

			// update the order status and payment status
			ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
			ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;

            // Add the order header & order detail to the database
			_unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
			_unitOfWork.Save();
			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
                // Create a new OrderDetail object
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

            // Create a Stripe session   
            var domain = "https://localhost:44350/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                // SuccessUrl is the URL that the user will be redirected to after the payment is successful
                SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                // CancelUrl is the URL that the user will be redirected to if the payment is cancelled
                CancelUrl = domain + "customer/cart/index",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in ShoppingCartVM.ShoppingCartList)
            {
                // create a new session line item
                var sessionLineItem = new Stripe.Checkout.SessionLineItemOptions
                {
                    // price data is used to set the price of the item
                    PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                    { 
                        UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                        Currency = "usd",
                        ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem); // add the item to the line items
            }
            var service = new Stripe.Checkout.SessionService(); // create a new session service
            Stripe.Checkout.Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url); // add the session url to the response header
            return new StatusCodeResult(303); // redirect to the payment session url

            //return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
        }

		public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader
                .Get(u => u.Id == id, includeProperties: "ApplicationUser"); // get the order header
            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Get(orderHeader.SessionId); 
            if(session.PaymentStatus == "paid")
            {

                string subject = "Order Confirmed " + orderHeader.Id;

                _unitOfWork.OrderHeader.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId); 
                _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                _unitOfWork.Save();
                HttpContext.Session.Clear();

                // Tạo nội dung HTML cho email
                string message = $@"<!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title>Booking Confirm</title>
                        <style>
                            .booking-confirm {{
                                max-width: 600px;
                                margin: 0 auto;
                                padding: 20px;
                                background-color: #f4f4f4;
                                border-radius: 10px;
                            }}
                            .booking-confirm .fw-bold {{
                                font-weight: bold;
                            }}
                            .booking-confirm .text-muted {{
                                color: #6c757d;
                            }}
                            .booking-confirm .bg-info {{
                                background-color: #0d6efd;
                            }}
                            .booking-confirm .text-info {{
                                color: #0d6efd;
                            }}
                            .booking-confirm .bg-light {{
                                background-color: #f8f9fa;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class=""booking-confirm"">
                            <h4 class=""text-info"">Order Successfully</h4>
                
                        </div>
                    </body>
                    </html>
                    ";

                _emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, subject, message);
            }
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList(); 
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();

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
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, tracked: true);
            if (cartFromDb.Count <= 1)
            {
                //remove that from cart
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
                    .GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1); // -1 because we are removing one item
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
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, tracked: true);
            // Session
            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        private double GetPrice(ShoppingCart shoppingCart)
        {
            return shoppingCart.Product.Price;            
        }
    }
}
