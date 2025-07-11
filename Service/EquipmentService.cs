using DataAccess.Models;
using Repositories;
using System.Collections.Generic;

namespace Services
{
    public class EquipmentService
    {
        private readonly EquipmentRepo _repo;

        public EquipmentService()
        {
            _repo = new EquipmentRepo();
        }

        public List<Equipment> GetAll() => _repo.GetAll();

        public Equipment Get(int id) => _repo.Get(id);

        public void Create(Equipment equipment) => _repo.Create(equipment);

        public void Update(Equipment equipment) => _repo.Update(equipment);

        public void Delete(int id) => _repo.Delete(id);

        public bool IsAvailable(int equipmentId, int quantity)
        {
            var equipment = _repo.Get(equipmentId);
            return equipment != null
                && equipment.Status?.ToLower() == "active"
                && equipment.QuantityAvailable >= quantity;
        }


    }
}
