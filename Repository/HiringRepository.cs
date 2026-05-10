using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using DataAccessObjects;
namespace Repository
{
    public class HiringRepository : IHiringRepository
    {
        private readonly HiringDAO hiringDAO;
        public HiringRepository(HiringDAO dao)
        {
            hiringDAO = dao;
        }
        public async Task<IEnumerable<Hiring>> GetListHirings()
        {
            return await hiringDAO.GetListHirings();
        }
        public async Task<Hiring?> GetHiringById(int id)
        {
            return await hiringDAO.GetHiringById(id);
        }
        public async Task AddHiring(Hiring hiring)
        {
            await hiringDAO.AddHiring(hiring);
        }
        public async Task UpdateHiring(Hiring hiring)
        {
            await hiringDAO.UpdateHiring(hiring);
        }
        public async Task DeleteHiring(int id)
        {
            await hiringDAO.DeleteHiring(id);
        }
        public async Task<List<Hiring>> GetListHiringByCompanyId(int companyId)
        {
            return await hiringDAO.GetListHiringByCompanyId(companyId);
        }
        public async Task<List<HiringAvailable>> GetListHiringAvailableByHiringId(int hiringId)
        {
            return await hiringDAO.GetListHiringAvailableByHiringId(hiringId);
        }
        public async Task<HiringAvailable?> GetHiringAvailableById(int id)
        {
            return await hiringDAO.GetHiringAvailableById(id);
        }
        public async Task AddHiringAvailable(HiringAvailable hiringAvailable)
        {
            await hiringDAO.AddHiringAvailable(hiringAvailable);
        }
        public async Task UpdateHiringAvailable(HiringAvailable hiringAvailable)
        {
            await hiringDAO.UpdateHiringAvailable(hiringAvailable);
        }
        public async Task DeleteHiringAvailable(int id)
        {
            await hiringDAO.DeleteHiringAvailable(id);
        }
    }
}
