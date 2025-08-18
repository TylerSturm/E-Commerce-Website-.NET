using TS.DataAccess.Data;
using TS.DataAccess.Repository.IRepository;
using TS.Models;
using TS.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TS.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db) => _db = db;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        public void Update(Product obj)
        #pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        {
            var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
            if(objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.Description = obj.Description;
                objFromDb.ISBN = obj.ISBN;
                objFromDb.Price = obj.Price;
                objFromDb.Price50 = obj.Price50;
                objFromDb.Price100 = obj.Price100;
                objFromDb.ListPrice = obj.ListPrice;
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.Author = obj.Author;
                if (obj.ImageUrl != null)
                {
                    objFromDb.ImageUrl = obj.ImageUrl;
                }
            }
        }

        public Product Get(int id)
        {
            return _db.Products.FirstOrDefault(p => p.Id == id);
        }

        object IProductRepository.Get(int productId)
        {
            return Get(productId);
        }
    }
}
