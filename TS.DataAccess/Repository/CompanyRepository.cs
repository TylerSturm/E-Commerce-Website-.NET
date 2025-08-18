using TS.DataAccess.Data;
using TS.DataAccess.Repository;
using TS.DataAccess.Repository.IRepository;
using TS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TS.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        #pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        public void Update(Company obj)
        #pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        {
            _db.Companies.Update(obj);
        }
    }
}