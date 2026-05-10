using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using DataAccessObjects;

namespace Repository
{
    public class InterviewRepository : IInterviewRepository
    {
        private readonly InterviewDAO interviewDAO;
        public InterviewRepository(InterviewDAO dao)
        {
            interviewDAO = dao;
        }

        public Task<Interview> AddInterview(Interview interview)
        {
            return interviewDAO.AddInterview(interview);
        }

        public Task DeleteInterview(int id)
        {
            return interviewDAO.DeleteInterview(id);
        }

        public Task<Interview?> GetInterviewById(int id)
        {
            return interviewDAO.GetInterviewById(id);
        }

        public Task<List<Interview>> GetListInterviewByHiringId(int hiringId)
        {
            return interviewDAO.GetListInterviewByHiringId(hiringId);
        }

        public Task<IEnumerable<Interview>> GetListInterviews()
        {
            return interviewDAO.GetListInterviews();
        }

        public Task<Interview> UpdateInterview(Interview interview)
        {
            return interviewDAO.UpdateInterview(interview);
        }
        public Task<Interview> GetInterviewByCandidateId(int candidateId)
        {
            return interviewDAO.GetInterviewByCandidateId(candidateId);
        }
    }
}
