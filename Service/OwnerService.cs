using DataAccess.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class OwnerService
    {
        private readonly OwnerRepo _repo;

        public OwnerService()
        {
            _repo = new OwnerRepo();
        }

        public List<Owner> GetAll() => _repo.GetAll();

        public Owner Get(int id) => _repo.Get(id);

        public void Create(Owner owner) => _repo.Create(owner);

        public void Update(Owner owner) => _repo.Update(owner);

        public void Delete(int id) => _repo.Delete(id);

        public Owner? Login(string username, string password)
        {
            return _repo.GetByUsernameAndPassword(username, password);
        }

        public Owner? GetByUsername(string username)
        {
            return _repo.GetByUsername(username);
        }

    }

}
