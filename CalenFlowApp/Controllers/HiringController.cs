using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repository;

namespace CalenFlowApp.Controllers
{
    public class HiringController : Controller
    {
        private readonly IHiringRepository hiringRepository;
        public HiringController(IHiringRepository hiringRepository)
        {
            this.hiringRepository = hiringRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetByCompany(int companyId)
        {
            List<Hiring> hirings = await hiringRepository.GetListHiringByCompanyId(companyId);
            var result = new List<object>();
            if ( !hirings.IsNullOrEmpty())
            {
                foreach (var h in hirings)
                {
                    var availables = await hiringRepository.GetListHiringAvailableByHiringId(h.HiringId);

                    var hiringObj = new
                    {
                        hiringId = h.HiringId,
                        position = h.Position,
                        hiringAvailables = availables.Select(a => new
                        {
                            id = a.HiringAvailableId,
                            date = a.Date.ToString("yyyy-MM-dd"),
                            time = a.Time
                        }).ToList()
                    };
                    result.Add(hiringObj);
                }
            }
            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetHiringAvailabilities(int hiringId)
        {
            List<HiringAvailable> availabilities = await hiringRepository.GetListHiringAvailableByHiringId(hiringId);
            var result = availabilities.Select(a => new
            {
                id = a.HiringAvailableId,
                date = a.Date.ToString("yyyy-MM-dd"),
                time = a.Time
            }).ToList();
            return Json(result);
        }
    }
}
