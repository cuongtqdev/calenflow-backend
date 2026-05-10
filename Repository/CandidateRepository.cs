using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using DataAccessObjects;
namespace Repository
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly CandidatesDAO candidatesDAO;
        public CandidateRepository(CandidatesDAO dao)
        {
            candidatesDAO = dao;
        }
        public async Task<IEnumerable<Candidate>> GetListCandidates()
        {
            return await candidatesDAO.GetListCandidates();
        }
        public async Task<Candidate?> GetCandidateById(int id)
        {
            return await candidatesDAO.GetCandidateById(id);
        }
        public async Task AddCandidate(Candidate candidate)
        {
            await candidatesDAO.AddCandidate(candidate);
        }
        public async Task UpdateCandidate(Candidate candidate)
        {
            await candidatesDAO.UpdateCandidate(candidate);
        }
        public async Task DeleteCandidate(int id)
        {
            await candidatesDAO.DeleteCandidate(id);
        }
    }
}
