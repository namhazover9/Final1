using FinalWeb1.DataAccess.Repository.IRepository;
using FinalWeb1.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinalWeb1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var orders = _unitOfWork.OrderDetail.GetAll(includeProperties: "Product,Product.ApplicationUser,Product.Category,OrderHeader");

            var data = orders.GroupBy(b => b.Product.ApplicationUser.Name) // Nhóm các sản phẩm theo người bán
            .Select(g => new
            {
                SellerName = g.Key,
                TotalProducts = g.Sum(x => x.Count), // Tính tổng số lượng sản phẩm
                TotalRevenue = g.Sum(x => x.Price * x.Count) // Tính tổng doanh thu
            })
            .ToList();
            string[] labels = new string[data.Count()];
            string[] totalProducts = new string[data.Count()];
            string[] totalRevenue = new string[data.Count()];
            string[] rgbs = new string[data.Count()];
            var dataByCategory = orders.GroupBy(b => b.Product.Category.Name) // Nhóm các sản phẩm theo người bán
            .Select(g => new
            {
                Category = g.Key,
                CategoryTotalProducts = g.Sum(x => x.Count), // Tính tổng số lượng sản phẩm
                CategoryTotalRevenue = g.Sum(x => x.Price * x.Count) // Tính tổng doanh thu
            })
            .ToList();
            string[] categoryLabels = new string[dataByCategory.Count()];
            string[] categoryTotalProducts = new string[dataByCategory.Count()];
            string[] categoryTotalRevenue = new string[dataByCategory.Count()];
            string[] categoryrgbs = new string[dataByCategory.Count()];

            Random rnd = new Random();

            for (int i = 0; i < data.Count(); i++)
            {
                labels[i] = data[i].SellerName;
                totalProducts[i] = data[i].TotalProducts.ToString();
                totalRevenue[i] = data[i].TotalRevenue.ToString();

                int red = rnd.Next(0, 255);
                int blue = rnd.Next(0, 255);
                int green = rnd.Next(0, 255);
                rgbs[i] = $"'rgb({red}, {green}, {blue})'";
            }
            for (int i = 0; i < dataByCategory.Count(); i++)
            {
                categoryLabels[i] = dataByCategory[i].Category;
                categoryTotalProducts[i] = dataByCategory[i].CategoryTotalProducts.ToString();
                categoryTotalRevenue[i] = dataByCategory[i].CategoryTotalRevenue.ToString();

                int red = rnd.Next(0, 255);
                int blue = rnd.Next(0, 255);
                int green = rnd.Next(0, 255);
                categoryrgbs[i] = $"'rgb({red}, {green}, {blue})'";
            }

            ViewData["labels"] = string.Join(",", labels.Select(l => $"'{l}'"));
            ViewData["totalProducts"] = string.Join(",", totalProducts);
            ViewData["totalRevenue"] = string.Join(",", totalRevenue);
            ViewData["rgbs"] = string.Join(",", rgbs);
            ViewData["categoryLabels"] = string.Join(",", categoryLabels.Select(l => $"'{l}'"));
            ViewData["categoryTotalProducts"] = string.Join(",", categoryTotalProducts);
            ViewData["categoryTotalRevenue"] = string.Join(",", categoryTotalRevenue);
            
            ViewData["categoryrgbs"] = string.Join(",", categoryrgbs);

            return View();
        }
    }
}
