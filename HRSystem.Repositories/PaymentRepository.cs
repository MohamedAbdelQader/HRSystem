using HRSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.Repositories
{
    public class PaymentRepository : GeneralRepositories<Payment>
    {
        public PaymentRepository(HRContext _DBContext) : base(_DBContext)
        {
        }

        public new Payment Add(Payment obj) =>
            base.Add(obj).Entity;
    }
}
