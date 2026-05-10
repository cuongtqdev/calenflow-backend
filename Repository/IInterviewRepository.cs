using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
namespace Repository
{
    public interface IInterviewRepository
    {
        Task<IEnumerable<Interview>> GetListInterviews();
        Task<Interview?> GetInterviewById(int id);
        Task<Interview> AddInterview(Interview interview);
        Task<Interview> UpdateInterview(Interview interview);
        Task DeleteInterview(int id);
        Task<List<Interview>> GetListInterviewByHiringId(int hiringId);
        Task<Interview> GetInterviewByCandidateId(int candidateId);
    }
}
