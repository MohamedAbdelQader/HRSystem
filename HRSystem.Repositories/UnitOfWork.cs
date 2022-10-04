using HRSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.Repositories
{
    public class UnitOfWork
    {
        private readonly HRContext HRContext;
        public UnitOfWork(HRContext hRContext)
        {
            HRContext = hRContext;
        }
        public void Save()
        {
            HRContext.SaveChanges();
        }
    }
}
