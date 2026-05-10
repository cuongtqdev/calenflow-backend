using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace CalenFlowApp.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;
        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }
        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            List<Company> companies = await _companyService.GetAllCompanies();
            var rs = companies.Select(c => new
            {
                id = c.CompanyId,
                name = c.Name
            });
            return Json(rs);
        }
    }
}
