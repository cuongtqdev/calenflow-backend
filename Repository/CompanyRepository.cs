using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using DataAccessObjects;
namespace Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly CompanyDAO _companyDAO;
        public CompanyRepository(CompanyDAO companyDAO)
        {
            _companyDAO = companyDAO;
        }
        public async Task<Company> GetCompanyById(int id)
        {
            return await _companyDAO.GetCompanyById(id);
        }
        public async Task<List<Company>> GetAllCompanies()
        {
            return await _companyDAO.GetAllCompanies();
        }
        public async Task AddCompany(Company company)
        {
            await _companyDAO.AddCompany(company);
        }
        public async Task UpdateCompany(Company company)
        {
            await _companyDAO.UpdateCompany(company);
        }
        public async Task DeleteCompany(int id)
        {
            await _companyDAO.DeleteCompany(id);
        }
    }
}
