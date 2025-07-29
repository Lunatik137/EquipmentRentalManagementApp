using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class UserRepo
    {
        EquipmentRentalManagementContext context;

        public UserRepo()
        {
            context = new EquipmentRentalManagementContext();
        }

        public IQueryable<User> GetAll()
        {
            return context.Users;
        }

        public User Get(int id)
        {
            return context.Users.FirstOrDefault(x => x.UserId == id);
        }

        public void Create(User user)
        {
            context.Users.Add(user);
            context.SaveChanges();
        }

        public void Update(User user)
        {
            var u = Get(user.UserId);
            if (u != null)
            {
                u.FullName = user.FullName;
                u.Username = user.Username;
                u.Password = user.Password;
                u.Phone = user.Phone;
                u.Email = user.Email;
                u.IsActive = user.IsActive;
                u.Role = user.Role;
                context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var u = Get(id);
            if (u != null)
            {
                context.Users.Remove(u);
                context.SaveChanges();
            }
        }

        public User? GetByUsernameAndPassword(string username, string password)
        {
            return context.Users.FirstOrDefault(x => x.Username == username && x.Password == password && x.IsActive);
        }

        public User? GetByUsername(string username)
        {
            return context.Users.FirstOrDefault(u => u.Username == username);
        }
    }
}