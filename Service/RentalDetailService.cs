using DataAccess.Models;
using Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class RentalDetailService
    {
        private readonly RentalDetailRepo _repo;

        public RentalDetailService()
        {
            _repo = new RentalDetailRepo();
        }

        public List<RentalDetail> GetAll()
        {
            return _repo.GetAll().ToList();
        }

        public RentalDetail Get(int id)
        {
            return _repo.Get(id);
        }

        public void Create(RentalDetail detail)
        {
            _repo.Create(detail);
        }

        public void Update(RentalDetail detail)
        {
            _repo.Update(detail);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public List<RentalDetail> GetByContractId(int contractId)
        {
            return _repo.GetByContractId(contractId).ToList();
        }

        public void ReturnRentalDetail(int rentalDetailId)
        {
            var detail = _repo.Get(rentalDetailId);
            if (detail != null)
            {
                if (detail.EquipmentId.HasValue)
                {
                    var equipmentRepo = new EquipmentRepo();
                    var equipment = equipmentRepo.Get(detail.EquipmentId.Value);
                    if (equipment != null)
                    {
                        equipment.QuantityAvailable += (int)(detail.Quantity ?? 0); 
                        equipmentRepo.Update(equipment);
                    }
                }
                _repo.Delete(rentalDetailId);
            }
        }
    }
}