using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Repository;
namespace Service
{
    public class RescheduleService : IRescheduleService
    {
        private readonly IRescheduleRepository rescheduleRepository;
        public RescheduleService(IRescheduleRepository repository)
        {
            rescheduleRepository = repository;
        }
        public async Task<List<Reschedule>> GetAll()
        {
            return await rescheduleRepository.GetAll();
        }
        public async Task<Reschedule?> GetById(int candidateid, int hiringid)
        {
            return await rescheduleRepository.GetById(candidateid, hiringid);
        }
        public async Task Add(Reschedule reschedule)
        {
            await rescheduleRepository.Add(reschedule);
        }
        public async Task Update(Reschedule reschedule)
        {
            await rescheduleRepository.Update(reschedule);
        }
        public async Task Delete(int candidateid, int hiringid)
        {
            await rescheduleRepository.Delete(candidateid, hiringid);
        }
    }
}
