using DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace Repositories
{
    public class PaymentRepo
    {
        EquipmentRentalManagementContext context;

        public PaymentRepo()
        {
            context = new EquipmentRentalManagementContext();
        }

        public List<Payment> GetAll()
        {
            return context.Payments.ToList();
        }

        public Payment Get(int id)
        {
            return context.Payments.FirstOrDefault(x => x.PaymentId == id);
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

        public List<Payment> GetByContractId(int contractId)
        {
            return context.Payments.Where(p => p.ContractId == contractId).ToList();
        }
    }
}
