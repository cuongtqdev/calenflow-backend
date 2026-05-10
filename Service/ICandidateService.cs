using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
namespace Service
{
    public interface ICandidateService
    {
        Task<IEnumerable<Candidate>> GetListCandidates();
        Task<Candidate?> GetCandidateById(int id);
        Task AddCandidate(Candidate candidate);
        Task UpdateCandidate(Candidate candidate);
        Task DeleteCandidate(int id);
        Task UpdateStatusCandidate(List<Interview> interviews);
    }
}
