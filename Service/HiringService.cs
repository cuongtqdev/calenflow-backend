using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Repository;
namespace Service
{
    public class HiringService : IHiringService
    {
        private readonly IHiringRepository hiringRepository;
        public HiringService(IHiringRepository repository)
        {
            hiringRepository = repository;
        }
        public async Task<IEnumerable<Hiring>> GetListHirings()
        {
            return await hiringRepository.GetListHirings();
        }
        public async Task<Hiring?> GetHiringById(int id)
        {
            return await hiringRepository.GetHiringById(id);
        }
        public async Task AddHiring(Hiring hiring)
        {
            await hiringRepository.AddHiring(hiring);
        }
        public async Task UpdateHiring(Hiring hiring)
        {
            await hiringRepository.UpdateHiring(hiring);
        }
        public async Task DeleteHiring(int id)
        {
            await hiringRepository.DeleteHiring(id);
        }

        public async Task<List<Hiring>> GetListHiringByCompanyId(int companyId)
        {
            return await hiringRepository.GetListHiringByCompanyId(companyId);
        }

        public async Task<List<HiringAvailable>> GetListHiringAvailableByHiringId(int hiringId)
        {
            return await hiringRepository.GetListHiringAvailableByHiringId(hiringId);
        }
        public async Task<HiringAvailable?> GetHiringAvailableById(int id)
        {
            return await hiringRepository.GetHiringAvailableById(id);
        }
        public async Task AddHiringAvailable(HiringAvailable hiringAvailable)
        {
            await hiringRepository.AddHiringAvailable(hiringAvailable);
        }
        public async Task UpdateHiringAvailable(HiringAvailable hiringAvailable)
        {
            await hiringRepository.UpdateHiringAvailable(hiringAvailable);
        }
        public async Task DeleteHiringAvailable(int id)
        {
            await hiringRepository.DeleteHiringAvailable(id);
        }
    }
}
