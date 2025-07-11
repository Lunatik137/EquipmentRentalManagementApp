using DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace Repositories
{
    public class RentalContractRepo
    {
        EquipmentRentalManagementContext context;

        public RentalContractRepo()
        {
            context = new EquipmentRentalManagementContext();
        }

        public List<RentalContract> GetAll()
        {
            return context.RentalContracts.ToList();
        }

        public RentalContract Get(int id)
        {
            return context.RentalContracts.FirstOrDefault(x => x.ContractId == id);
        }

        public void Delete(int id)
        {
            var c = Get(id);
            if (c != null)
            {
                context.RentalContracts.Remove(c);
                context.SaveChanges();
            }
        }

        public void Create(RentalContract contract)
        {
            context.RentalContracts.Add(contract);
            context.SaveChanges();
        }

        public void Update(RentalContract contract)
        {
            var c = Get(contract.ContractId);
            if (c != null)
            {
                c.OwnerId = contract.OwnerId;
                c.StartDate = contract.StartDate;
                c.EndDate = contract.EndDate;
                c.ReturnDate = contract.ReturnDate;
                c.TotalAmount = contract.TotalAmount;
                c.Status = contract.Status;
                context.SaveChanges();
            }
        }

        public List<RentalContract> GetByOwnerId(int ownerId)
        {
            return context.RentalContracts.Where(c => c.OwnerId == ownerId).ToList();
        }
    }
}
