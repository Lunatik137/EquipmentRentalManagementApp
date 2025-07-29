using DataAccess.Models;
using Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class PaymentService
    {
        private readonly PaymentRepo _repo;

        public PaymentService()
        {
            _repo = new PaymentRepo();
        }

        public List<Payment> GetAll()
        {
            return _repo.GetAll().ToList();
        }

        public Payment Get(int id)
        {
            return _repo.Get(id);
        }

        public void Create(Payment payment)
        {
            _repo.Create(payment);
        }

        public void Update(Payment payment)
        {
            _repo.Update(payment);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public List<Payment> GetByContractId(int contractId)
        {
            return _repo.GetByContractId(contractId).ToList();
        }

        public decimal GetTotalPaidForContract(int contractId)
        {
            return _repo.GetTotalPaidForContract(contractId);
        }
    }
}