using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TS.Models;

namespace TS.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        object Get(int productId);
        void Update(Product obj);
    }
}
