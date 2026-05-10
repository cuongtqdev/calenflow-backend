using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
namespace DataAccessObjects
{
    public class HiringDAO
    {
        private readonly CalenFlowContext calenFlowContext;
        public HiringDAO(CalenFlowContext context)
        {
            calenFlowContext = context;
        }
        public async Task<IEnumerable<Hiring>> GetListHirings()
        {
            return await calenFlowContext.Hirings.ToListAsync();
        }
        public async Task<Hiring?> GetHiringById(int id)
        {
            return await calenFlowContext.Hirings.Include(h => h.HiringNavigation.Company).FirstOrDefaultAsync(h => h.HiringId == id);
        }
        public async Task AddHiring(Hiring hiring)
        {
            calenFlowContext.Hirings.Add(hiring);
            await calenFlowContext.SaveChangesAsync();
        }
        public async Task UpdateHiring(Hiring hiring)
        {
            calenFlowContext.Hirings.Update(hiring);
            await calenFlowContext.SaveChangesAsync();
        }
        public async Task DeleteHiring(int id)
        {
            var hiring = await calenFlowContext.Hirings.FindAsync(id);
            if (hiring != null)
            {
                calenFlowContext.Hirings.Remove(hiring);
                await calenFlowContext.SaveChangesAsync();
            }
        }
        public async Task<List<Hiring>> GetListHiringByCompanyId(int companyId)
        {
            return await calenFlowContext.Hirings
                .Where(h => h.HiringNavigation.CompanyId == companyId)
                .ToListAsync();
        }
        public async Task<List<HiringAvailable>> GetListHiringAvailableByHiringId(int hiringId)
        {
            return await calenFlowContext.HiringAvailables
                .Where(ha => ha.HiringId == hiringId)
                .ToListAsync();
        }
        public async Task<HiringAvailable?> GetHiringAvailableById(int id)
        {
            return await calenFlowContext.HiringAvailables.FindAsync(id);
        }
        public async Task AddHiringAvailable(HiringAvailable hiringAvailable)
        {
            calenFlowContext.HiringAvailables.Add(hiringAvailable);
            await calenFlowContext.SaveChangesAsync();
        }
        public async Task UpdateHiringAvailable(HiringAvailable hiringAvailable)
        {
            calenFlowContext.HiringAvailables.Update(hiringAvailable);
            await calenFlowContext.SaveChangesAsync();
        }
        public async Task DeleteHiringAvailable(int id)
        {
            var hiringAvailable = await calenFlowContext.HiringAvailables.FindAsync(id);
            if (hiringAvailable != null)
            {
                calenFlowContext.HiringAvailables.Remove(hiringAvailable);
                await calenFlowContext.SaveChangesAsync();
            }
        }
    }
}
