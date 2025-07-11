using DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace Repositories
{
    public class RentalDetailRepo
    {
        EquipmentRentalManagementContext context;

        public RentalDetailRepo()
        {
            context = new EquipmentRentalManagementContext();
        }

        public List<RentalDetail> GetAll()
        {
            return context.RentalDetails.ToList();
        }

        public RentalDetail Get(int id)
        {
            return context.RentalDetails.FirstOrDefault(x => x.RentalDetailId == id);
        }

        public void Delete(int id)
        {
            var d = Get(id);
            if (d != null)
            {
                context.RentalDetails.Remove(d);
                context.SaveChanges();
            }
        }

        public void Create(RentalDetail detail)
        {
            context.RentalDetails.Add(detail);
            context.SaveChanges();
        }

        public void Update(RentalDetail detail)
        {
            var d = Get(detail.RentalDetailId);
            if (d != null)
            {
                d.ContractId = detail.ContractId;
                d.EquipmentId = detail.EquipmentId;
                d.Quantity = detail.Quantity;
                d.RatePerDay = detail.RatePerDay;
                context.SaveChanges();
            }
        }

        public List<RentalDetail> GetByContractId(int contractId)
        {
            return context.RentalDetails.Where(d => d.ContractId == contractId).ToList();
        }
    }
}
