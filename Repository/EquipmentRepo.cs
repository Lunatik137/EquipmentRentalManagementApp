using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class EquipmentRepo
    {
        EquipmentRentalManagementContext context;

        public EquipmentRepo()
        {
            context = new EquipmentRentalManagementContext();
        }

        public IQueryable<Equipment> GetAll()
        {
            return context.Equipment;
        }

        public Equipment Get(int id)
        {
            return context.Equipment.FirstOrDefault(x => x.EquipmentId == id);
        }

        public void Create(Equipment equipment)
        {
            context.Equipment.Add(equipment);
            context.SaveChanges();
        }

        public void Update(Equipment equipment)
        {
            var e = Get(equipment.EquipmentId);
            if (e != null)
            {
                e.Name = equipment.Name;
                e.Description = equipment.Description;
                e.Category = equipment.Category;
                e.Status = equipment.Status;
                e.QuantityAvailable = equipment.QuantityAvailable;
                e.DailyRate = equipment.DailyRate;
                e.PurchaseDate = equipment.PurchaseDate;
                context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var e = Get(id);
            if (e != null)
            {
                context.Equipment.Remove(e);
                context.SaveChanges();
            }
        }

        public IQueryable<Equipment> SearchByName(string keyword)
        {
            return context.Equipment.Where(e => e.Name.Contains(keyword));
        }
    }
}