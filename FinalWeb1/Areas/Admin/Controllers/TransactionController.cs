using FinalWeb1.Areas.Customer.Controllers;
using FinalWeb1.DataAccess.Repository.IRepository;
using FinalWeb1.Models;
using FinalWeb1.Models.ViewModels;
using FinalWeb1.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalWeb1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class TransactionController : Controller
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public TransactionController(ILogger<TransactionController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,ApplicationUser").ToList();
            return View(productList);
        }

        //public IActionResult Details(int productId)
        //{
        //    // Get the product and include the category
        //    ShoppingCart cart = new()
        //    {
        //        Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category,ProductImages,ApplicationUser"),
        //        Count = 1,
        //        ProductId = productId
        //    };
        //    return View(cart);
        //}

        public IActionResult Summary(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "Category,ApplicationUser");
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST(int? id)
        {
            Product? product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "Category,ApplicationUser");

            //// Get the shopping cart list based on the user id
            //ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
            //    includeProperties: "Product");

            //// Set the order info - order date, user id
            //ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            //ShoppingCartVM.OrderHeader.ApplicationUserId = userId;
            //ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            //// Set the price
            //foreach (var cart in ShoppingCartVM.ShoppingCartList)
            //{
            //    cart.Price = GetPrice(cart);
            //    ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            //}

            product.IsPay = true;
            _unitOfWork.Product.Update(product);
            _unitOfWork.Save();

            // Create a Stripe session   
            var domain = "https://localhost:44350/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                // SuccessUrl is the URL that the user will be redirected to after the payment is successful
                SuccessUrl = domain + $"admin/transaction/PaymentConfirmation",
                // CancelUrl is the URL that the user will be redirected to if the payment is cancelled
                CancelUrl = domain + "admin/transaction/index",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
            };

            
            // create a new session line item
            var sessionLineItem = new Stripe.Checkout.SessionLineItemOptions
            {
                // price data is used to set the price of the item
                PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)((product.Price - (product.Price * 0.1)) * 100), // $20.50 => 2050
                    Currency = "usd",
                    ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                    {
                        Name = product.Name
                    }
                },
                Quantity = 1
            };
            options.LineItems.Add(sessionLineItem); // add the item to the line items
            
            var service = new Stripe.Checkout.SessionService(); // create a new session service
            Stripe.Checkout.Session session = service.Create(options);
            Response.Headers.Add("Location", session.Url); // add the session url to the response header
            return new StatusCodeResult(303); // redirect to the payment session url

            //return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
        }


        public IActionResult PaymentConfirmation()
        {                     
            return View();
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category,ApplicationUser").ToList();
            return Json(new { data = objProductList });
        }

        //[HttpDelete]
        //public IActionResult Delete(int? id)
        //{
        //    var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
        //    if (productToBeDeleted == null)
        //    {
        //        return Json(new { success = false, message = "Error while deleting" });
        //    }

        //    string productPath = @"images\products\product-" + id; // to get the path of the product
        //    string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath); // to get the final path of the product

        //    if (Directory.Exists(finalPath))
        //    {
        //        string[] filePaths = Directory.GetFiles(finalPath); // to get the file paths
        //        foreach (string filePath in filePaths)
        //        {
        //            System.IO.File.Delete(filePath);
        //        }

        //        Directory.Delete(finalPath);
        //    }

        //    _unitOfWork.Product.Remove(productToBeDeleted);
        //    _unitOfWork.Save();
        //    return Json(new { success = true, message = "Delete Successful" });
        //}

        #endregion
    }
}
