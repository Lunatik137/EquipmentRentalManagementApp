using DataAccess.Models;
using Repositories;
using System.Collections.Generic;

namespace Services
{
    public class RentalDetailService
    {
        private readonly RentalDetailRepo _repo;

        public RentalDetailService()
        {
            _repo = new RentalDetailRepo();
        }

        public List<RentalDetail> GetAll() => _repo.GetAll();

        public RentalDetail Get(int id) => _repo.Get(id);

        public void Create(RentalDetail detail) => _repo.Create(detail);

        public void Update(RentalDetail detail) => _repo.Update(detail);

        public void Delete(int id) => _repo.Delete(id);

        public List<RentalDetail> GetByContractId(int contractId) => _repo.GetByContractId(contractId);

        public bool CreateRentalDetailWithInventoryCheck(RentalDetail detail)
        {
            if (detail.EquipmentId == null || detail.ContractId == null || detail.Quantity == null)
                return false;

            var equipmentRepo = new EquipmentRepo();
            var equipment = equipmentRepo.Get(detail.EquipmentId.Value);

            if (equipment == null || equipment.QuantityAvailable < detail.Quantity || equipment.Status?.ToLower() != "active")
            {
                return false;
            }

            equipment.QuantityAvailable -= detail.Quantity;
            equipmentRepo.Update(equipment);

            _repo.Create(detail);

            var contractService = new RentalContractService();
            contractService.RecalculateTotalAmount(detail.ContractId.Value);

            return true;
        }

        public void ReturnRentalDetail(int rentalDetailId)
        {
            var detail = _repo.Get(rentalDetailId);
            if (detail == null || detail.EquipmentId == null || detail.ContractId == null || detail.Quantity == null) return;

            var equipmentRepo = new EquipmentRepo();
            var equipment = equipmentRepo.Get(detail.EquipmentId.Value);
            if (equipment != null)
            {
                equipment.QuantityAvailable += detail.Quantity;
                equipmentRepo.Update(equipment);
            }

            _repo.Delete(rentalDetailId);

            var contractService = new RentalContractService();
            contractService.RecalculateTotalAmount(detail.ContractId.Value);
        }

    }
}
