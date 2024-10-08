﻿using FinalWeb1.DataAccess.Data;
using FinalWeb1.DataAccess.Repository;
using FinalWeb1.DataAccess.Repository.IRepository;
using FinalWeb1.Models;
using FinalWeb1.Models.ViewModels;
using FinalWeb1.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FinalWeb1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork; // to access the database
        private readonly IWebHostEnvironment _webHostEnvironment; // to get the root path of the application
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return View(objProductList);

        }
       
        // GET - UPSERT
        public IActionResult Browse(int? id)
        {
            var browseList = new List<string> {
                "Pending",
                "Approved",
                "Denied",
            };
            ViewBag.BrowseList = new SelectList(browseList);
            ProductVM productVM = new() // to pass the product and category list to the view
            {

                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                //create
                productVM.Product.ApplicationUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "ProductImages");
                return View(productVM);
            }

        }
        // POST - UPSERT
        [HttpPost]
        public IActionResult Browse(ProductVM productVM, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                if (productVM.Product.Id == 0)
                {
                    productVM.Product.ApplicationUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }

                _unitOfWork.Save();

                string wwwRootPath = _webHostEnvironment.WebRootPath; // to get the root path of the application
                if (files != null)
                {

                    foreach (IFormFile file in files)
                    {

                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); // to get the file name
                        string productPath = @"image\products\product-" + productVM.Product.Id; // to get the path of the image
                        string finalPath = Path.Combine(wwwRootPath, productPath); // to get the final path of the image

                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);

                        // to get the file stream and copy the file to the final path
                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        // to save the image path to the database
                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = productVM.Product.Id,
                        };

                        // to add the image to the product
                        if (productVM.Product.ProductImages == null)
                            productVM.Product.ProductImages = new List<ProductImage>();

                        productVM.Product.ProductImages.Add(productImage);

                    }

                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save();

                }


                TempData["success"] = "Product browsed successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
        }


        // Delete
        public IActionResult SoftDelete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product? product = _unitOfWork.Product.Get(u => u.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost, ActionName("SoftDelete")]
        public IActionResult SoftDeletePOST(int? id)
        {
            Product? obj = _unitOfWork.Product.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            obj.IsDeleted = true;
            _unitOfWork.Product.Update(obj);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            string productPath = @"images\products\product-" + id; // to get the path of the product
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath); // to get the final path of the product

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath); // to get the file paths
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }

                Directory.Delete(finalPath);
            }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
