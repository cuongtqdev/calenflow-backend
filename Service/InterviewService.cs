using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository;
using BusinessObjects.Models;
namespace Service
{
    public class InterviewService : IInterviewService
    {
        private readonly IInterviewRepository interviewRepository;
        public InterviewService(IInterviewRepository repository)
        {
            interviewRepository = repository;
        }
        public Task<Interview> AddInterview(Interview interview)
        {
            return interviewRepository.AddInterview(interview);
        }
        public Task DeleteInterview(int id)
        {
            return interviewRepository.DeleteInterview(id);
        }

        public Task<List<Interview>> FilterListInterviewByDay(DateOnly date, List<Interview> list)
        {
            List<Interview> rs = new List<Interview> ();
            foreach ( Interview i in list )
            {
                if (i.Date == date )
                {
                    rs.Add(i);
                }
            }
            return Task.FromResult(rs);
        }

        public Task<Interview?> GetInterviewById(int id)
        {
            return interviewRepository.GetInterviewById(id);
        }
        public Task<List<Interview>> GetListInterviewByHiringId(int hiringId)
        {
            return interviewRepository.GetListInterviewByHiringId(hiringId);
        }
        public Task<IEnumerable<Interview>> GetListInterviews()
        {
            return interviewRepository.GetListInterviews();
        }
        public Task<Interview> UpdateInterview(Interview interview)
        {
            return interviewRepository.UpdateInterview(interview);
        }

        public async Task UpdateStatusInterview(List<Interview> interviews)
        {
            DateTime now = DateTime.Now;
            foreach (var interview in interviews)
            {
                DateTime interviewTime = interview.Date.ToDateTime(interview.Time);
                if (interviewTime > now && interview.Status == "Active") continue;
                if (interviewTime <= now && ( interview.Candidate.Status != "Pending" && interview.Status == "Completed" || 
                                              interview.Candidate.Status == "Pending" && interview.Status == "Pending")) continue;
                interview.Status = interviewTime > now ? "Active" : (interview.Candidate.Status == "Pending" ? "Pending" : "Completed");
                await interviewRepository.UpdateInterview(interview);
            }
        }
        public Task<Interview> GetInterviewByCandidateId(int candidateId)
        {
            return interviewRepository.GetInterviewByCandidateId(candidateId);
        }
    }
}
