using DataAccess.Models;
using Repositories;
using System.Collections.Generic;

namespace Services
{
    public class RentalContractService
    {
        private readonly RentalContractRepo _repo;

        public RentalContractService()
        {
            _repo = new RentalContractRepo();
        }

        public List<RentalContract> GetAll() => _repo.GetAll();

        public RentalContract Get(int id) => _repo.Get(id);

        public void Create(RentalContract contract) => _repo.Create(contract);

        public void Update(RentalContract contract) => _repo.Update(contract);

        public void Delete(int id) => _repo.Delete(id);

        public List<RentalContract> GetByOwnerId(int ownerId) => _repo.GetByOwnerId(ownerId);

        public void RecalculateTotalAmount(int contractId)
        {
            var rentalDetailRepo = new RentalDetailRepo();
            var contract = _repo.Get(contractId);
            if (contract == null) return;

            if (contract.StartDate == null || contract.EndDate == null)
            {
                contract.TotalAmount = 0;
                _repo.Update(contract);
                return;
            }

            var details = rentalDetailRepo.GetByContractId(contractId);
            decimal total = 0;

            int days = contract.EndDate.Value.DayNumber - contract.StartDate.Value.DayNumber;
            if (days <= 0) days = 1;

            foreach (var d in details)
            {
                if (d.RatePerDay != null && d.Quantity != null)
                {
                    total += d.RatePerDay.Value * d.Quantity.Value * days;
                }
            }

            contract.TotalAmount = total;
            _repo.Update(contract);
        }

    }
}
