using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Repository;
namespace Service
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }
        public async Task<Company> GetCompanyById(int id)
        {
            return await _companyRepository.GetCompanyById(id);
        }
        public async Task<List<Company>> GetAllCompanies()
        {
            return await _companyRepository.GetAllCompanies();
        }
        public async Task AddCompany(Company company)
        {
            await _companyRepository.AddCompany(company);
        }
        public async Task UpdateCompany(Company company)
        {
            await _companyRepository.UpdateCompany(company);
        }
        public async Task DeleteCompany(int id)
        {
            await _companyRepository.DeleteCompany(id);
        }
    }
}
