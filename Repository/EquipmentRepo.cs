using DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace Repositories
{
    public class EquipmentRepo
    {
        EquipmentRentalManagementContext context;

        public EquipmentRepo()
        {
            context = new EquipmentRentalManagementContext();
        }

        public List<Equipment> GetAll()
        {
            return context.Equipment.ToList();
        }

        public Equipment Get(int id)
        {
            return context.Equipment.FirstOrDefault(x => x.EquipmentId == id);
        }

        public void Delete(int id)
        {
            var eq = Get(id);
            if (eq != null)
            {
                context.Equipment.Remove(eq);
                context.SaveChanges();
            }
        }

        public void Create(Equipment equipment)
        {
            context.Equipment.Add(equipment);
            context.SaveChanges();
        }

        public void Update(Equipment equipment)
        {
            var eq = Get(equipment.EquipmentId);
            if (eq != null)
            {
                eq.Name = equipment.Name;
                eq.Description = equipment.Description;
                eq.Category = equipment.Category;
                eq.Status = equipment.Status;
                eq.DailyRate = equipment.DailyRate;
                eq.PurchaseDate = equipment.PurchaseDate;
                eq.QuantityAvailable = equipment.QuantityAvailable;
                context.SaveChanges();
            }
        }
    }
}
