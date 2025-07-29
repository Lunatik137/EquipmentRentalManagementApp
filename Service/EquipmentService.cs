using DataAccess.Models;
using Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class EquipmentService
    {
        private readonly EquipmentRepo _repo;

        public EquipmentService()
        {
            _repo = new EquipmentRepo();
        }

        public List<Equipment> GetAll()
        {
            return _repo.GetAll().ToList();
        }

        public Equipment Get(int id)
        {
            return _repo.Get(id);
        }

        public void Create(Equipment equipment)
        {
            _repo.Create(equipment);
        }

        public void Update(Equipment equipment)
        {
            _repo.Update(equipment);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public List<Equipment> SearchByName(string keyword)
        {
            return _repo.SearchByName(keyword).ToList();
        }
    }
}