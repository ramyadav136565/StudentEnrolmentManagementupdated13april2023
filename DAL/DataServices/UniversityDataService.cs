namespace DAL
{
    using DAL.Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    public class UniversityDataService
    {
        private BookStoreContext _dbContext;
        public UniversityDataService(BookStoreContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<University>> ShowAllUniversities()
        {
            List<University> universities;
            try
            {
                universities = await _dbContext.Universities.ToListAsync();
                return universities;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

       public async Task<University> SearchUniversityById(int universityId)
        {
            try
            {
                var university = await _dbContext.Universities.FindAsync(universityId);
                if (university == null)
                {
                    throw new Exception($"University with ID {universityId} not found.");
                }
               
                return university;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public async Task<University> AddUniversity(University university)
        {
            try
            {
                var existingUniversity = await _dbContext.Universities.FirstOrDefaultAsync(u => u.Name == university.Name);

                if (existingUniversity != null)
                {
                    throw new Exception("University already exists");
                }
                 _dbContext.Universities.Add(university);
                  await _dbContext.SaveChangesAsync();

                return university;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<University> UpdateUniversity(University university)
        {
            try
            {
                var existingUniversity = await _dbContext.Universities.FindAsync(university.UniversityId);
                if (existingUniversity != null )
                {
                    _dbContext.Entry(existingUniversity).CurrentValues.SetValues(university);
                    await _dbContext.SaveChangesAsync();
                    return existingUniversity;
                }
                else
                {
                    throw new Exception("University not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> DeleteUniversity(int universityId)
        {
            try
            {
                var university = await _dbContext.Universities.FindAsync(universityId);

                if (university != null)
                {
                    if (!university.IsDeleted)
                    {
                        university.IsDeleted = true;
                        await _dbContext.SaveChangesAsync();
                        return "The university has successfully become inactive";
                    }
                    else
                    {
                        return $"The university with {university.UniversityId} inactive already."; 
                    }
                }
                else
                {
                    return "The university is not present.";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the status of university.", ex);
            }
        }


    }
}
