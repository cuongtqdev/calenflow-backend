using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
namespace Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetListUsers();
        Task<User?> GetUserById(int id);
        Task AddUser(User user);
        Task UpdateUser(User user);
        Task DeleteUser(int id);
        Task<List<int>> GetListUserIdByUserNameAndPassword(string userName, string passwordHash);
        Task<User> GetUserByEmail(string email);
        Task<List<User>> GetListUserByEmail(string email);
        Task<List<int>> GetListUserIdByEmail(string email);
    }
}
