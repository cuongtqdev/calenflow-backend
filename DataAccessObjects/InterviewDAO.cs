using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
namespace DataAccessObjects
{
    public class InterviewDAO
    {
        private readonly CalenFlowContext calenFlowContext;
        public InterviewDAO(CalenFlowContext context)
        {
            calenFlowContext = context;
        }
        public async Task<IEnumerable<Interview>> GetListInterviews()
        {
            return await calenFlowContext.Interviews.Include(i => i.Hiring.HiringNavigation.Company).Include(i => i.Candidate).ToListAsync();
        }
        public async Task<Interview?> GetInterviewById(int id)
        {
            return await calenFlowContext.Interviews.FindAsync(id);
        }
        public async Task<Interview> AddInterview(Interview interview)
        {
            calenFlowContext.Interviews.Add(interview);
            await calenFlowContext.SaveChangesAsync();
            return interview;
        }
        public async Task<Interview> UpdateInterview(Interview interview)
        {
            calenFlowContext.Interviews.Update(interview);
            await calenFlowContext.SaveChangesAsync();
            return interview;
        }
        public async Task DeleteInterview(int id)
        {
            var interview = await calenFlowContext.Interviews.FindAsync(id);
            if (interview != null)
            {
                calenFlowContext.Interviews.Remove(interview);
                await calenFlowContext.SaveChangesAsync();
            }
        }

        public async Task<List<Interview>> GetListInterviewByHiringId(int hiringId)
        {
            var interviews = await calenFlowContext.Interviews
                .Where(i => i.HiringId == hiringId)
                .Include(i => i.Candidate)
                .Include(i => i.Candidate.CandidateNavigation)
                .Include(i => i.Hiring.HiringNavigation.Company)
                .ToListAsync();
            return interviews;
        }

        public async Task<Interview> GetInterviewByCandidateId(int candidateId)
        {
            var interview = await calenFlowContext.Interviews
                .Include(i => i.Candidate)
                .Include(i => i.Candidate.CandidateNavigation)
                .Include(i => i.Hiring.HiringNavigation.Company)
                .FirstOrDefaultAsync(i => i.CandidateId == candidateId);
            return interview;
        }   
    }
}
