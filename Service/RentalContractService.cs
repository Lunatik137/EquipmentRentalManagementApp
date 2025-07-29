using DataAccess.Models;
using Repositories;
using Repository;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class RentalContractService
    {
        private readonly RentalContractRepo _repo;

        public RentalContractService()
        {
            _repo = new RentalContractRepo();
        }

        public List<RentalContract> GetAll()
        {
            return _repo.GetAll().ToList();
        }

        public RentalContract Get(int id)
        {
            return _repo.Get(id);
        }

        public void Create(RentalContract contract)
        {
            _repo.Create(contract);
        }

        public void Update(RentalContract contract)
        {
            _repo.Update(contract);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public List<RentalContract> GetByOwnerId(int userId)
        {
            return _repo.GetByOwnerId(userId).ToList();
        }

        public List<RentalContract> SearchByUserId(int userId, string keyword)
        {
            return _repo.SearchByUserId(userId, keyword).ToList();
        }

        public void RecalculateTotalAmount(int contractId)
        {
            var contract = _repo.Get(contractId);
            if (contract == null) return;

            if (contract.StartDate == null || contract.EndDate == null)
            {
                contract.TotalAmount = 0m;
                _repo.Update(contract);
                return;
            }

            var rentalDetailRepo = new RentalDetailRepo();
            var details = rentalDetailRepo.GetByContractId(contractId).ToList();
            decimal total = 0m;

            int days = contract.EndDate.Value.DayNumber - contract.StartDate.Value.DayNumber;
            if (days <= 0) days = 1;

            foreach (var d in details)
            {
                if (d.RatePerDay.HasValue && d.Quantity.HasValue)
                {
                    total += d.RatePerDay.Value * d.Quantity.Value * days;
                }
            }

            contract.TotalAmount = total;
            _repo.Update(contract);
        }

        public List<RentalContract> GetReturnedContractsByOwnerId(int userId)
        {
            return _repo.GetByOwnerId(userId)
                .Where(c => c.Status != null && c.Status.ToLower() == "đã trả")
                .ToList();
        }
    }
}