using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using DataAccessObjects;
namespace Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDAO userDAO;
        public UserRepository(UserDAO dao)
        {
            userDAO = dao;
        }
        public async Task<IEnumerable<User>> GetListUsers()
        {
            return await userDAO.GetListUsers();
        }
        public async Task<User?> GetUserById(int id)
        {
            return await userDAO.GetUserById(id);
        }
        public async Task AddUser(User user)
        {
            await userDAO.AddUser(user);
        }
        public async Task UpdateUser(User user)
        {
            await userDAO.UpdateUser(user);
        }
        public async Task DeleteUser(int id)
        {
            await userDAO.DeleteUser(id);
        }
        public async Task<List<int>> GetListUserIdByUserNameAndPassword(string userName, string passwordHash)
        {
            return await userDAO.GetListUserIdByUserNameAndPassword(userName, passwordHash);
        }
        public async Task<User> GetUserByEmail(string email)
        {
            return await userDAO.GetUserByEmail(email);
        }
        public async Task<List<User>> GetListUserByEmail(string email)
        {
            return await userDAO.GetListUserByEmail(email);
        }
        public async Task<List<int>> GetListUserIdByEmail(string email)
        {
            return await userDAO.GetListUserIdByEmail(email);
        }
    }
}
