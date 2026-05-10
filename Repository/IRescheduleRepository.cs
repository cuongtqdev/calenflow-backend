using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
namespace Repository
{
    public interface IRescheduleRepository
    {
        Task<List<Reschedule>> GetAll();
        Task<Reschedule?> GetById(int candidateid, int hiringid);
        Task Add(Reschedule reschedule);
        Task Update(Reschedule reschedule);
        Task Delete(int candidateid, int hiringid);
    }
}
