using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using DataAccessObjects;
namespace Repository
{
    public class RescheduleRepository : IRescheduleRepository
    {
        private readonly RescheduleDAO rescheduleDAO;
        public RescheduleRepository(RescheduleDAO dao)
        {
            rescheduleDAO = dao;
        }
        public async Task<List<Reschedule>> GetAll()
        {
            var reschedules = await rescheduleDAO.GetListReschedules();
            return reschedules.ToList();
        }
        public async Task<Reschedule?> GetById(int candidateid, int hiringid)
        {
            return await rescheduleDAO.GetRescheduleById(candidateid, hiringid);
        }
        public async Task Add(Reschedule reschedule)
        {
            await rescheduleDAO.AddReschedule(reschedule);
        }
        public async Task Update(Reschedule reschedule)
        {
            await rescheduleDAO.UpdateReschedule(reschedule);
        }
        public async Task Delete(int candidateid, int hiringid)
        {
            await rescheduleDAO.DeleteReschedule(candidateid, hiringid);
        }
    }
}
