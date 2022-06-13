using AssessmentApiProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace AssessmentApiProject.Services
{
    public class UserService : IUserService
    {
        private readonly IMemoryCache _cache;
        public const string CacheKey = "AddressCacheKey";

        public UserService(IMemoryCache cache)
        {
            _cache = cache;
        }
        public async Task<string> AddUser(User user)
        {
            // Check if the name already exists in the cache
            if (_cache.TryGetValue(CacheKey, out List<User> newUser))
            {
                if (newUser.Exists(a => a.Name.Equals(user.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    return "Name already exists. Please use a different name.";
                }
            }
            else
            {
                newUser = new List<User>();
            }

            // Add the new user to the cache
            newUser.Add(user);
            _cache.Set(CacheKey, newUser);

            return "User added successfully.";
        }




        public async Task<string> UpdateUser(string name, User updatedUser)
        {
            if (_cache.TryGetValue(CacheKey, out List<User>? users))
            {
                // Find the user to update based on the name
                User? existingUser =  users.Find(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (existingUser != null)
                {
                    // Update the existing user properties with data from updatedUser
                    existingUser.Name = updatedUser.Name;
                    existingUser.Address = updatedUser.Address;

                    // If needed, update other properties of User based on your model

                    _cache.Set(CacheKey, users);

                    return "User updated successfully.";
                }
                else
                {
                    return "User not found.";
                }
            }

            return "No user found.";
        }



        public async Task<string> DeleteUser(string name)
        {
            if (_cache.TryGetValue(CacheKey, out List<User> user))
            {
                // Find the user to delete based on the name
                User? existingUser = user.Find(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (existingUser != null)
                {
                    // Remove the user from the cache
                    user.Remove(existingUser);
                    _cache.Set(CacheKey, user);

                    return "User deleted successfully.";
                }
                else
                {
                    return "User not found.";
                }
            }

            return "No user found.";
        }

        public async Task<List<User>> GetAllUsers()
        {
            if (_cache.TryGetValue(CacheKey, out List<User> user))
            {
                return user;
            }

            return new List<User>();
        }
    }
}
