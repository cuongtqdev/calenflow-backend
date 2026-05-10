using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
namespace DataAccessObjects
{
    public class CandidatesDAO
    {
        private readonly CalenFlowContext calenFlowContext;
        public CandidatesDAO(CalenFlowContext context)
        {
            calenFlowContext = context;
        }
        public async Task<IEnumerable<Candidate>> GetListCandidates()
        {
            return await calenFlowContext.Candidates.ToListAsync();
        }
        public async Task<Candidate?> GetCandidateById(int id)
        {
            return await calenFlowContext.Candidates.Include(c => c.CandidateNavigation).FirstOrDefaultAsync(c => c.CandidateId == id);
        }
        public async Task AddCandidate(Candidate candidate)
        {
            calenFlowContext.Candidates.Add(candidate);
            await calenFlowContext.SaveChangesAsync();
        }
        public async Task UpdateCandidate(Candidate candidate)
        {
            calenFlowContext.Candidates.Update(candidate);
            await calenFlowContext.SaveChangesAsync();
        }
        public async Task DeleteCandidate(int id)
        {
            var candidate = await calenFlowContext.Candidates.FindAsync(id);
            if (candidate != null)
            {
                calenFlowContext.Candidates.Remove(candidate);
                await calenFlowContext.SaveChangesAsync();
            }
        }
    }
}
