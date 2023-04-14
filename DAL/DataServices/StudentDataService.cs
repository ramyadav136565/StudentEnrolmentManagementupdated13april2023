namespace DAL
{
    using DAL.Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    public class StudentDataService
    {
        private BookStoreContext _dbContext;
        public StudentDataService(BookStoreContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Student>> ShowAllStudents()
        {
            List<Student> students;
            try
            {
                students = await _dbContext.Students.ToListAsync();
                return students;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        //To find all not deleted student record
        //public async Task<List<Student>> GetAllStudents()
        //{
        //    try
        //    {
        //        var students = await _dbContext.Students
        //                                        .Where(s => !s.IsDeleted)
        //                                        .ToListAsync();
        //        return students;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Error fetching students: {ex.Message}");
        //    }
        //}

        public async Task<Student> GetStudentById(int studentId)
        {
            try
            {
                var student = await _dbContext.Students.FindAsync(studentId);
                if (student == null)
                {
                    throw new Exception($"Student with ID {studentId} not found.");
                }
                if (student.IsDeleted)
                {
                    throw new Exception($"Student with ID {studentId} has been deleted.");
                }
                return student;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Student> AddStudent(Student student)
        {
            try
            {
                if (student == null)
                {
                    throw new ArgumentNullException(nameof(student), "Invalid or null student data provided.");
                }


                var university = await _dbContext.Universities.FindAsync(student.UniversityId);

                if (university == null)
                {
                    throw new ArgumentException("The university ID you provided is invalid. Please enter a valid university ID.");
                }

                var existingStudent = await _dbContext.Students.FirstOrDefaultAsync(s => s.StudentId == student.StudentId
                    && s.FullName == student.FullName && s.Email == student.Email && s.UniversityId == student.UniversityId);

                if (existingStudent != null)
                {
                    throw new InvalidOperationException("This student is already enrolled in this university.");
                }
                await _dbContext.Students.AddAsync(student);
                await _dbContext.SaveChangesAsync();
                return student;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while saving the student record.", ex);
            }
        }

        public async Task<Student> UpdateStudent(Student student)
        {
            try
            {
                var existingStudent = await _dbContext.Students.FindAsync(student.StudentId);
                if (existingStudent != null)
                {
                    _dbContext.Entry(existingStudent).CurrentValues.SetValues(student);
                    _dbContext.SaveChanges();
                    return existingStudent;
                }
                else
                {
                    throw new Exception("Student Record Not Found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Student> DeleteStudent(int studentId)
        {
            try
            {
                var student = await _dbContext.Students.FindAsync(studentId);
                if (student != null && student.IsDeleted == false)
                {
                    student.IsDeleted = true;
                    _dbContext.Entry(student).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    return student;
                }
                else if (student != null && student.IsDeleted == true)
                {
                    throw new ArgumentException("Sorry, the student you are looking for is no longer active");
                }
                else
                {
                    throw new Exception("Student Record not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while searching for the student",ex);
            }
        }

    }
}
