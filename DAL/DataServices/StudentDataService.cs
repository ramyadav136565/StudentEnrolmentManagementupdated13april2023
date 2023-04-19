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
                //if (student.IsDeleted)
                //{
                //    throw new Exception($"Student with ID {studentId} has been deleted.");
                //}
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
                if (student.UniversityId == 0 || student.Email== null || student.Term == 0 || student.Course == "" || student.FullName == null || student.Address == null)
                {
                    throw new ArgumentNullException(nameof(student), "the student data provided appears to be invalid or null");
                }
                //if (student==null)
                //{
                //    throw new ArgumentNullException(nameof(student), "The student data provided appears to be invalid or null");
                //}


                //var university = await _dbContext.Universities.FindAsync(student.UniversityId);

                //if (university == null)
                //{
                //    throw new ArgumentException("The university ID you provided is invalid. Please enter a valid university ID.");
                //}

                //var existingStudent = await _dbContext.Students.FirstOrDefaultAsync(s => 
                //   s.Email == student.Email && s.UniversityId == student.UniversityId);
                var existingStudent = await _dbContext.Students.FirstOrDefaultAsync(s =>
                  s.Email == student.Email);

                if (existingStudent != null)
                {
                    throw new InvalidOperationException($"The student with {student.Email} is already enrolled in this university.");
                }
                await _dbContext.Students.AddAsync(student);
                await _dbContext.SaveChangesAsync();
                return student;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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

        //public async Task<Student> DeleteStudent(int studentId)
        //{
        //    try
        //    {
        //        var student = await _dbContext.Students.FindAsync(studentId);

        //            _dbContext.Students.Remove(student);
        //            await _dbContext.SaveChangesAsync();
        //            return student;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred while deleting the student record", ex);
        //    }
        //}

        public async Task<string> DeleteStudent(int studentId)
        {
            try
            {
                var student = await _dbContext.Students.FindAsync(studentId);

                if (student != null)
                {
                    if (!student.IsDeleted)
                    {
                        student.IsDeleted = true;
                        await _dbContext.SaveChangesAsync();
                        return "The student record has been deleted successfully.";
                    }
                    else
                    {
                        return "The student  is already inactive.";
                    }
                }
                else
                {
                    return "Student record not found.";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the student record.", ex);
            }
        }
    }
}
