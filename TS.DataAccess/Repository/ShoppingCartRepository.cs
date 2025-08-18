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
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private ApplicationDbContext _db;
        public ShoppingCartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        #pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        public void Update(ShoppingCart obj)
        #pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        {
            _db.ShoppingCarts.Update(obj);
        }
    }
}
