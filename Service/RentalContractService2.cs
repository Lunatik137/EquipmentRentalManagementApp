using DataAccess.Models;
using Repositories;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class RentalContractService2
    {
        private readonly IRentalContractRepository _repo;

        public RentalContractService2(IRentalContractRepository repo)
        {
            _repo = repo;
        }

        public RentalContract Get(int id)
        {
            return _repo.Get(id);
        }

        public List<RentalContract> GetByOwnerId(int userId)
        {
            return _repo.GetByOwnerId(userId).ToList();
        }

        public List<RentalContract> SearchByUserId(int userId, string keyword)
        {
            return _repo.SearchByUserId(userId, keyword).ToList();
        }

        public List<RentalContract> GetReturnedContractsByOwnerId(int userId)
        {
            return _repo.GetByOwnerId(userId)
                .Where(c => c.Status != null && c.Status.ToLower() == "đã trả")
                .ToList();
        }
    }
}
