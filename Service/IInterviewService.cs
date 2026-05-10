using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
namespace Service
{
    public interface IInterviewService
    {
        Task<IEnumerable<Interview>> GetListInterviews();
        Task<Interview?> GetInterviewById(int id);
        Task<Interview> AddInterview(Interview interview);
        Task<Interview> UpdateInterview(Interview interview);
        Task DeleteInterview(int id);
        Task<List<Interview>> GetListInterviewByHiringId(int hiringId);
        Task UpdateStatusInterview(List<Interview> interviews);
        Task<List<Interview>> FilterListInterviewByDay(DateOnly date, List<Interview> list);
        Task<Interview> GetInterviewByCandidateId(int candidateId);
    }
}
