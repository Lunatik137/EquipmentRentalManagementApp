using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class PaymentRepo
    {
        EquipmentRentalManagementContext context;

        public PaymentRepo()
        {
            context = new EquipmentRentalManagementContext();
        }

        public IQueryable<Payment> GetAll()
        {
            return context.Payments.Include(p => p.Contract).ThenInclude(c => c.User);
        }

        public Payment Get(int id)
        {
            return context.Payments.FirstOrDefault(x => x.PaymentId == id);
        }

        public void Create(Payment payment)
        {
            context.Payments.Add(payment);
            context.SaveChanges();
        }

        public void Update(Payment payment)
        {
            var p = Get(payment.PaymentId);
            if (p != null)
            {
                p.ContractId = payment.ContractId;
                p.PaymentDate = payment.PaymentDate;
                p.AmountPaid = payment.AmountPaid;
                p.Note = payment.Note;
                context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var p = Get(id);
            if (p != null)
            {
                context.Payments.Remove(p);
                context.SaveChanges();
            }
        }

        public IQueryable<Payment> GetByContractId(int contractId)
        {
            return context.Payments.Where(p => p.ContractId == contractId)
                .Include(p => p.Contract).ThenInclude(c => c.User);
        }

        public decimal GetTotalPaidForContract(int contractId)
        {
            return context.Payments
                .Where(p => p.ContractId == contractId && p.AmountPaid.HasValue)
                .DefaultIfEmpty(new Payment { AmountPaid = 0m })
                .Sum(p => p.AmountPaid ?? 0m);
        }
    }
}