namespace DAL
{
    using DAL.Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
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
        public async Task<User> ActiveUserById(int userId)
        {
            try
            {
                var user = await _dbContext.Users.FindAsync(userId);
                if (user != null && user.IsActive == false)
                {
                    throw new InvalidOperationException("User is inactive");
                }
                else if (user == null)
                {
                    throw new ArgumentException("User not found");
                }
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message) ;
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

               
                await _dbContext.Users.AddAsync(user);
                _dbContext.SaveChanges();
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }




        public async Task<User> UpdateUser(User user)
            {
                try
                {
                    if (user != null)
                    {
                        _dbContext.Entry(user).State = EntityState.Modified;
                        await  _dbContext.SaveChangesAsync();
                        return user;
                    }
                    else
                    {
                        throw new Exception("User not found");
                    }
                }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the user's profile", ex);
            }
        }
        //public async Task<User> DeleteUser(int userId)
        //{
        //    try
        //    {
        //        var user = await _dbContext.Users.FindAsync(userId);
        //        if (user == null)
        //        {
        //            throw new Exception("User not found");
        //        }
        //        _dbContext.Users.Remove(user);
        //        await _dbContext.SaveChangesAsync();
        //        return user;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}


        public async Task<User> DeleteUser(int userId)
        {
            try
            {
                var user = await _dbContext.Users.FindAsync(userId);
                if (user != null && user.IsActive == false)
                {
                    throw new ArgumentException("Sorry, the university you are looking for is no longer active");
                }
                else if (user != null && user.IsActive == true)
                {
                    user.IsActive = false;
                    _dbContext.SaveChanges();
                    return user;
                }
                else
                {
                    throw new Exception("Record not Found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while searching for the user", ex);
            }
        }
    }
}


