using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
namespace DataAccessObjects
{
    public class CompanyDAO
    {
        private readonly CalenFlowContext _context;
        public CompanyDAO(CalenFlowContext context)
        {
            _context = context;
        }
        public async Task<Company> GetCompanyById(int id)
        {
            return await _context.Companies.FindAsync(id);
        }
        public async Task<List<Company>> GetAllCompanies()
        {
            return await Task.FromResult(_context.Companies.ToList());
        }
        public async Task AddCompany(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateCompany(Company company)
        {
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteCompany(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company != null)
            {
                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();
            }
        }
    }
}
