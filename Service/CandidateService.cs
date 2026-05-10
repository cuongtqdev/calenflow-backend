using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Repository;
namespace Service
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository candidateRepository;
        public CandidateService(ICandidateRepository repository)
        {
            candidateRepository = repository;
        }
        public async Task<IEnumerable<Candidate>> GetListCandidates()
        {
            return await candidateRepository.GetListCandidates();
        }
        public async Task<Candidate?> GetCandidateById(int id)
        {
            return await candidateRepository.GetCandidateById(id);
        }
        public async Task AddCandidate(Candidate candidate)
        {
            await candidateRepository.AddCandidate(candidate);
        }
        public async Task UpdateCandidate(Candidate candidate)
        {
            await candidateRepository.UpdateCandidate(candidate);
        }
        public async Task DeleteCandidate(int id)
        {
            await candidateRepository.DeleteCandidate(id);
        }

        public async Task UpdateStatusCandidate(List<Interview> interviews)
        {
            DateTime now = DateTime.Now;
            foreach (Interview interview in interviews)
            {
                DateTime interviewTime = interview.Date.ToDateTime(interview.Time);
                Candidate c = await candidateRepository.GetCandidateById(interview.CandidateId);
                if (interviewTime > now && c.Status != "Active")
                {
                    c.Status = "Active";
                    await candidateRepository.UpdateCandidate(c);
                } else if (interviewTime <= now && (c.Status == "hired" || c.Status == "rejected"))
                {
                    continue;
                }
                else
                {
                    c.Status = "Pending";
                    await candidateRepository.UpdateCandidate(c);
                }
            }
        }
    }
}
