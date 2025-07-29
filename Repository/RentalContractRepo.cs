using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
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

        public IQueryable<RentalContract> GetAll()
        {
            return context.RentalContracts.Include(c => c.User);
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
                c.UserId = contract.UserId;
                c.StartDate = contract.StartDate;
                c.EndDate = contract.EndDate;
                c.ReturnDate = contract.ReturnDate;
                c.TotalAmount = contract.TotalAmount;
                c.Status = contract.Status;
                context.SaveChanges();
            }
        }

        public IQueryable<RentalContract> GetByOwnerId(int userId)
        {
            return context.RentalContracts.Where(c => c.UserId == userId).Include(c => c.User);
        }

        public IQueryable<RentalContract> SearchByUserId(int userId, string keyword)
        {
            return context.RentalContracts
                .Where(c => c.UserId == userId && (c.ContractId.ToString().Contains(keyword) || c.Status.Contains(keyword)))
                .Include(c => c.User);
        }
    }
}