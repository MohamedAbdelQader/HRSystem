using HRSystem.Models;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRSystem.Repositories
{
    public class GroupRepository : GeneralRepositories<Group>
    {
        private readonly EmployeeRepository EmpRepo;
        public GroupRepository(EmployeeRepository _EmpRepo,HRContext context) :base(context)
        {
            EmpRepo = _EmpRepo;
        }

        public Group GetByID(int ID)
        {
            var filter = PredicateBuilder.New<Group>();
            if (ID != 0)
                filter = filter.Or(f => f.Id == ID);
            var result = base.GetByID(filter);
            if (result != null)
                return result;
            else return new Group();
        }

        public async Task<Employee> AddToGroup(Employee Emp ,Group gro)
        {
            Emp.GroupID = gro.Id;
            var result = await EmpRepo.UpdateGroup(Emp);
            return result;
        }
        public new Group Add(Group obj) =>
            base.Add(obj).Entity;

        public bool Remove(int ID)
        {
            var result = GetByID(ID);
            var Del = base.Remove(result).Entity;
            return true;
        }
    }
}
