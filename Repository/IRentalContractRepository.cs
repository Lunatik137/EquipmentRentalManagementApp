using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IRentalContractRepository
    {
        RentalContract Get(int id);
        IEnumerable<RentalContract> GetByOwnerId(int ownerId);
        IEnumerable<RentalContract> SearchByUserId(int userId, string keyword);
    }

}
