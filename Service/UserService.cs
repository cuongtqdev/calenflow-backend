using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Repository;
namespace Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        public UserService(IUserRepository repository)
        {
            userRepository = repository;
        }
        public async Task<IEnumerable<User>> GetListUsers()
        {
            return await userRepository.GetListUsers();
        }
        public async Task<User?> GetUserById(int id)
        {
            return await userRepository.GetUserById(id);
        }
        public async Task AddUser(User user)
        {
            await userRepository.AddUser(user);
        }
        public async Task UpdateUser(User user)
        {
            await userRepository.UpdateUser(user);
        }
        public async Task DeleteUser(int id)
        {
            await userRepository.DeleteUser(id);
        }
        public async Task<List<int>> GetListUserIdByUserNameAndPassword(string userName, string passwordHash)
        {
            return await userRepository.GetListUserIdByUserNameAndPassword(userName, passwordHash);
        }
        public async Task<User> GetUserByEmail(string email)
        {
            return await userRepository.GetUserByEmail(email);
        }
        public async Task<List<User>> GetListUserByEmail(string email)
        {
            return await userRepository.GetListUserByEmail(email);
        }
        public async Task<List<int>> GetListUserIdByEmail(string email)
        {
            return await userRepository.GetListUserIdByEmail(email);
        }
    }
}
