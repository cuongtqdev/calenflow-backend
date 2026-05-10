using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
namespace Repository
{
    public interface IHiringRepository
    {
        Task<IEnumerable<Hiring>> GetListHirings();
        Task<Hiring?> GetHiringById(int id);
        Task AddHiring(Hiring hiring);
        Task UpdateHiring(Hiring hiring);
        Task DeleteHiring(int id);
        Task<List<Hiring>> GetListHiringByCompanyId(int companyId);
        Task<List<HiringAvailable>> GetListHiringAvailableByHiringId(int hiringId);
        Task<HiringAvailable?> GetHiringAvailableById(int id);
        Task AddHiringAvailable(HiringAvailable hiringAvailable);
        Task UpdateHiringAvailable(HiringAvailable hiringAvailable);
        Task DeleteHiringAvailable(int id);
    }
}
