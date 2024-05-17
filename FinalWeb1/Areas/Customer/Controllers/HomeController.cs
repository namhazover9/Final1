using FinalWeb1.DataAccess.Repository;
using FinalWeb1.DataAccess.Repository.IRepository;
using FinalWeb1.Models;
using FinalWeb1.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using OpenAI_API;
using OpenAI_API.Completions;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;




namespace FinalWeb1.Areas.Customer.Controllers
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

       
        public IActionResult Index(string searchTerm)
        {
            IEnumerable<Product> products;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = _unitOfWork.Product.GetAll(u => u.Name.Contains(searchTerm), includeProperties: "Category,ProductImages,ApplicationUser");
            }
            else
            {
                products = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductImages,ApplicationUser");
            }

            return View(products);
        }

        
        public IActionResult All(string searchTerm, string sortOrder)
        {
            IEnumerable<Product> products;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = _unitOfWork.Product.GetAll(u => u.Name.Contains(searchTerm), includeProperties: "Category,ProductImages,ApplicationUser");
            } 
            else
            {
                products = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductImages,ApplicationUser");
            }

            switch(sortOrder)
            {
                case "desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                default:
                    products = products.OrderBy(p => p.Name);
                    break;
            }
            return View(products);

        }

        //public IActionResult Search(string search)
        //{
        //    var movies = _unitOfWork.Movie.GetAll(m => m.Title.Contains(search) || m.Description.Contains(search), includeProperties: "MovieCategories.Category").ToList();
        //    ViewBag.Categories = new SelectList(_unitOfWork.Category.GetAll(), "Id", "Name");
        //    return View("Index", movies);
        //}
        public IActionResult Category1(string searchTerm, string sortOrder)
        {
            IEnumerable<Product> products;
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = _unitOfWork.Product.GetAll(u => u.Name.Contains(searchTerm) && u.CategoryId == 1, 
                    includeProperties: "Category,ProductImages,ApplicationUser");
            }
            else
            {
                products = _unitOfWork.Product.GetAll(u => u.CategoryId == 1,
                    includeProperties: "Category,ProductImages,ApplicationUser");
            }

            switch (sortOrder)
            {
                case "desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                default:
                    products = products.OrderBy(p => p.Name);
                    break;
            }
            return View(products);
        }

        public IActionResult Category2(string searchTerm, string sortOrder)
        {
            IEnumerable<Product> products;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = _unitOfWork.Product.GetAll(u => u.Name.Contains(searchTerm) && u.CategoryId == 2,
                    includeProperties: "Category,ProductImages,ApplicationUser");
            }
            else
            {
                products = _unitOfWork.Product.GetAll(u => u.CategoryId == 2,
                    includeProperties: "Category,ProductImages,ApplicationUser");
            }

            switch (sortOrder)
            {
                case "desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                default:
                    products = products.OrderBy(p => p.Name);
                    break;
            }
            return View(products);
        }

        public IActionResult Category3(string searchTerm, string sortOrder)
        {
            IEnumerable<Product> products;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = _unitOfWork.Product.GetAll(u => u.Name.Contains(searchTerm) && u.CategoryId == 3,
                    includeProperties: "Category,ProductImages,ApplicationUser");
            }
            else
            {
                products = _unitOfWork.Product.GetAll(u => u.CategoryId == 3,
                    includeProperties: "Category,ProductImages,ApplicationUser");
            }
            switch (sortOrder)
            {
                case "desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                default:
                    products = products.OrderBy(p => p.Name);
                    break;
            }
            return View(products);
        }

        public IActionResult Category4(string searchTerm, string sortOrder)
        {
            IEnumerable<Product> products;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = _unitOfWork.Product.GetAll(u => u.Name.Contains(searchTerm) && u.CategoryId == 12,
                    includeProperties: "Category,ProductImages,ApplicationUser");
            }
            else
            {
                products = _unitOfWork.Product.GetAll(u => u.CategoryId == 12,
                    includeProperties: "Category,ProductImages,ApplicationUser");
            }
            switch (sortOrder)
            {
                case "desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                default:
                    products = products.OrderBy(p => p.Name);
                    break;
            }
            return View(products);
        }

        public IActionResult Category5(string searchTerm, string sortOrder)
        {
            IEnumerable<Product> products;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = _unitOfWork.Product.GetAll(u => u.Name.Contains(searchTerm) && u.CategoryId == 13,
                    includeProperties: "Category,ProductImages,ApplicationUser");
            }
            else
            {
                products = _unitOfWork.Product.GetAll(u => u.CategoryId == 13,
                    includeProperties: "Category,ProductImages,ApplicationUser");
            }
            switch (sortOrder)
            {
                case "desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                default:
                    products = products.OrderBy(p => p.Name);
                    break;
            }
            return View(products);
        }

        
        //public IActionResult Search(string searchTerm)
        //{
        //    IEnumerable<Product> products;

        //    if (!string.IsNullOrEmpty(searchTerm))
        //    {
        //        products = _unitOfWork.Product.GetAll(p => p.CategoryId == 1);
        //    }
        //    else
        //    {
        //        products = _unitOfWork.Product.GetAll();
        //    }

        //    return View(products);
        //}

        public IActionResult Details(int productId)
        {           
            ShoppingCart cart = new() 
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category,ProductImages,ApplicationUser"), 
                Count = 1, 
                ProductId = productId
            };
            return View(cart);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Customer)]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity; // Get the user's identity
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value; // Get the user's id
            shoppingCart.ApplicationUserId = userId; // Set the user's id to the shopping cart
            
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId &&
           u.ProductId == shoppingCart.ProductId); 

            if (cartFromDb != null)
            {
                //shopping cart exists
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();
            }
            else
            {
                //add cart record
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
                _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == userId).Count());
            }
            
            TempData["success"] = "Cart updated successfully";
            return RedirectToAction(nameof(All));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
