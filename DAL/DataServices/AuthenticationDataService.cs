namespace DALayer
{
    using DAL.Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Security.Cryptography;
    
    public class AuthenticationDataService
    {
        private BookStoreContext _dbContext;
        public AuthenticationDataService(BookStoreContext dbContext)
        {
            this._dbContext = dbContext;
        }
       
          public async Task<User> ValidateUser(string userIdOrEmail, string password)
        {
            User user = new User();
            try
            {
                var validuser = await _dbContext.Users.FirstOrDefaultAsync(c => c.UserId.ToString() == userIdOrEmail || c.Email == userIdOrEmail);
                if (validuser == null)
                {
                    throw new Exception("Username and password are required fields");
                }
               
                else if (GetHash(password) == validuser.Password)
                { if (validuser.IsActive == true)
                    {
                        user.FullName = validuser.FullName;
                        user.Email = validuser.Email;
                        return user;
                    }
                    else
                    {
                        throw new Exception("Your account is currently inactive. Please contact an administrator to reactivate your account.");
                    }
                }
                else 
                {
                    throw new Exception("Invalid login credentials. Please try again");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
         
         
         


        public async Task<User> RegisterUser(string userIdOrEmail, string password)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId.ToString() == userIdOrEmail || u.Email == userIdOrEmail);
                if (user == null)
                {
                    throw new Exception("Invalid username or password");
                }
                
                var passwordHash = GetHash(password);
                user.Password = passwordHash;
                await _dbContext.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        private string GetHash(string password)
        {
            SHA256 sha256 = SHA256Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha256.ComputeHash(bytes);
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("x2"));
            }
            return result.ToString();
        }

    }
}
