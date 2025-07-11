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

        public List<Payment> GetAll() => _repo.GetAll();

        public Payment Get(int id) => _repo.Get(id);

        public void Delete(int id) => _repo.Delete(id);

        public void Create(Payment payment) => _repo.Create(payment);

        public bool CreateSafe(Payment payment)
        {
            if (payment.AmountPaid == null || payment.AmountPaid <= 0)
                return false;

            _repo.Create(payment);
            return true;
        }

        public void Update(Payment payment) => _repo.Update(payment);

        public List<Payment> GetByContractId(int contractId) => _repo.GetByContractId(contractId);

        public decimal GetTotalPaidForContract(int contractId)
        {
            var payments = _repo.GetByContractId(contractId);
            return payments.Sum(p => p.AmountPaid ?? 0);
        }
    }
}
