using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
namespace DataAccessObjects
{
    public class RescheduleDAO
    {
        private readonly CalenFlowContext calenFlowContext;
        public RescheduleDAO(CalenFlowContext context)
        {
            calenFlowContext = context;
        }
        public async Task<IEnumerable<Reschedule>> GetListReschedules()
        {
            return await calenFlowContext.Reschedules
                .Include(r => r.Candidate)
                .Include(r => r.Hiring)
                .Include(r => r.Candidate.CandidateNavigation)
                .Include(r => r.Hiring.HiringNavigation)
                .ToListAsync();
        }
        public async Task<Reschedule?> GetRescheduleById(int candidateid, int hiringid)
        {
            return await calenFlowContext.Reschedules
                .Include(r => r.Hiring)
                .Include(r => r.Candidate)
                .FirstOrDefaultAsync(r => r.CandidateId == candidateid && r.HiringId == hiringid);
        }
        public async Task AddReschedule(Reschedule reschedule)
        {
            calenFlowContext.Reschedules.Add(reschedule);
            await calenFlowContext.SaveChangesAsync();
        }
        public async Task UpdateReschedule(Reschedule reschedule)
        {
            calenFlowContext.Reschedules.Update(reschedule);
            await calenFlowContext.SaveChangesAsync();
        }
        public async Task DeleteReschedule(int candidateid, int hiringid)
        {
            var reschedule = await GetRescheduleById(candidateid, hiringid);
            if (reschedule != null)
            {
                calenFlowContext.Reschedules.Remove(reschedule);
                await calenFlowContext.SaveChangesAsync();
            }
        }
    }
}
