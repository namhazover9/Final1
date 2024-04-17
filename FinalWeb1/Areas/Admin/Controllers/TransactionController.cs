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
