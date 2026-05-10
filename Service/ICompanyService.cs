using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Repository;
namespace Service
{
    public interface ICompanyService
    {
        Task<Company> GetCompanyById(int id);
        Task<List<Company>> GetAllCompanies();
        Task AddCompany(Company company);
        Task UpdateCompany(Company company);
        Task DeleteCompany(int id);
    }
}
