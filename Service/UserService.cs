using DataAccess.Models;
using Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class UserService
    {
        private readonly UserRepo _repo;

        public UserService()
        {
            _repo = new UserRepo();
        }

        public List<User> GetAll()
        {
            return _repo.GetAll().ToList();
        }

        public User Get(int id)
        {
            return _repo.Get(id);
        }

        public void Create(User user)
        {
            _repo.Create(user);
        }

        public void Update(User user)
        {
            _repo.Update(user);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public User? GetByUsernameAndPassword(string username, string password)
        {
            return _repo.GetByUsernameAndPassword(username, password);
        }

        public User? GetByUsername(string username)
        {
            return _repo.GetByUsername(username);
        }

        public bool IsOwner(User user)
        {
            return user.Role?.ToLower() == "owner";
        }
    }
}