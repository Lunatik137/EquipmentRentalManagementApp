using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class OwnerRepo
    {
        EquipmentRentalManagementContext context;

        public OwnerRepo()
        {
            context = new EquipmentRentalManagementContext();
        }

        public List<Owner> GetAll() => context.Owners.ToList();

        public Owner Get(int id) => context.Owners.FirstOrDefault(x => x.OwnerId == id);

        public void Create(Owner owner)
        {
            context.Owners.Add(owner);
            context.SaveChanges();
        }

        public void Update(Owner owner)
        {
            var o = Get(owner.OwnerId);
            if (o != null)
            {
                o.FullName = owner.FullName;
                o.Password = owner.Password;
                o.Username = owner.Username;
                o.IsActive = owner.IsActive;
                context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var o = Get(id);
            if (o != null)
            {
                context.Owners.Remove(o);
                context.SaveChanges();
            }
        }

        public Owner? GetByUsernameAndPassword(string username, string password)
        {
            return context.Owners.FirstOrDefault(x => x.Username == username && x.Password == password && x.IsActive);
        }

        public Owner? GetByUsername(string username)
        {
            return context.Owners.FirstOrDefault(o => o.Username == username);
        }

    }

}
