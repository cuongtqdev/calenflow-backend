using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
namespace DataAccessObjects
{
    public class UserDAO
    {
        private readonly CalenFlowContext calenFlowContext;
        public UserDAO(CalenFlowContext context)
        {
            calenFlowContext = context;
        }
        public async Task<IEnumerable<User>> GetListUsers()
        {
            return await calenFlowContext.Users.ToListAsync();
        }
        public async Task<User?> GetUserById(int id)
        {
            return await calenFlowContext.Users.Include(u => u.Candidate).FirstOrDefaultAsync(u => u.UserId == id);
        }
        public async Task AddUser(User user)
        {
            calenFlowContext.Users.Add(user);
            await calenFlowContext.SaveChangesAsync();
        }
        public async Task UpdateUser(User user)
        {
            calenFlowContext.Users.Update(user);
            await calenFlowContext.SaveChangesAsync();
        }
        public async Task DeleteUser(int id)
        {
            var user = await calenFlowContext.Users.FindAsync(id);
            if (user != null)
            {
                calenFlowContext.Users.Remove(user);
                await calenFlowContext.SaveChangesAsync();
            }
        }
        public async Task<List<int>> GetListUserIdByUserNameAndPassword(string userName, string passwordHash)
        {
            return await calenFlowContext.Users
                .Where(u => u.UserName == userName && u.PasswordHash == passwordHash)
                .Select(u => u.UserId)
                .ToListAsync();
        }
        public async Task<User> GetUserByEmail(string email)
        {
            return await calenFlowContext.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<List<User>> GetListUserByEmail(string email)
        {
            return await calenFlowContext.Users
                .Where(u => u.Email == email)
                .ToListAsync();
        }
        public async Task<List<int>> GetListUserIdByEmail(string email)
        {
            return await calenFlowContext.Users
                .Where(u => u.Email == email)
                .Select(u => u.UserId)
                .ToListAsync();
        }
    }
}
