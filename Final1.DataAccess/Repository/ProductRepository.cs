using FinalWeb1.DataAccess.Data;
using FinalWeb1.DataAccess.Repository.IRepository;
using FinalWeb1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalWeb1.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product obj)
        {
            var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.Description = obj.Description;
                objFromDb.Price = obj.Price;
                objFromDb.Condition = obj.Condition;
                objFromDb.Status = obj.Status;
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.IsPay = obj.IsPay;
                objFromDb.isBrowsed = obj.isBrowsed;
                objFromDb.IsDeleted = obj.IsDeleted;
                objFromDb.ProductImages = obj.ProductImages;
            }
        }
    }
}
