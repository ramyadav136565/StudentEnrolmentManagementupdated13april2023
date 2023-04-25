namespace DAL
{
    using DAL.Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    public class UserDataService
    {
  
        private BookStoreContext _dbContext;

        public UserDataService(BookStoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<User>> ShowAllUsers()
        {
            List<User> users;
            try
            {
                users = await _dbContext.Users.ToListAsync();
                return users;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<User> GetUserById(int userId)
        {
            try
            {
                var user = await _dbContext.Users.FindAsync(userId);
                if (user == null)
                {
                    throw new Exception($"User with ID {userId} not found.");
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> AddNewUser(User user)
        {
            try
            {
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    throw new Exception("User with this email already exists");
                }

                var password = GenerateRandomPassword();

                var hashedPassword = HashPassword(password);

                user.Password = hashedPassword;

                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();

                user.Password = password;
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string GenerateRandomPassword()
        {

            const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var chars = new char[64];
            using (var rng = new RNGCryptoServiceProvider())
            {
                for (var i = 0; i < chars.Length; i++)
                {
                    var byteBuffer = new byte[1];
                    rng.GetBytes(byteBuffer);
                    var randomChar = allowedChars[byteBuffer[0] % allowedChars.Length];
                    chars[i] = randomChar;
                }
            }
            return new string(chars);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public async Task<User> UpdateUser(User user)
        {
            try
            {
                var existinguser = await _dbContext.Users.FindAsync(user.UserId);
                if (existinguser != null)
                {
                    _dbContext.Entry(existinguser).CurrentValues.SetValues(user);
                    _dbContext.SaveChanges();
                    return existinguser;
                }
                else
                {
                    throw new Exception("User Record Not Found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> DeleteUser(int userId)
        {
            try
            {
                var user = await _dbContext.Users.FindAsync(userId);

                if (user != null)
                {
                    if (!user.IsActive)
                    {
                        user.IsActive = true;
                        await _dbContext.SaveChangesAsync();
                        return "The user's status has been successfully updated to inactive..";
                    }
                    else
                    {
                        return "The user  is already inactive.";
                    }
                }
                else
                {
                    return "user record not found.";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the status of user.", ex);
            }
        }
    }
}
