using Domain.Entities;
using Domain.Requests;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class StudentRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Student> GetStudentByIdAsync(int studentId)
    {
        return await _context.Students.FindAsync(studentId);
    }
    public async Task<string> AddStudent(AddStudent newstudent)
    {
        if (newstudent == null)
        {
            return "Error: Student object cannot be null.";
        }

        Student student = new Student();
        student.StudentId = newstudent.StudentId;
        student.FirstName = newstudent.FirstName;
        student.LastName = newstudent.LastName;

        try
        {
            // Use your student repository to add the student
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return "Student added successfully.";
        }
        catch (Exception ex)
        {
            // Handle any exceptions that might occur during data access
            Console.WriteLine($"Error adding student: {ex.Message}");
            return $"Error adding student: {ex.Message}"; // Or a more generic error message
        }
    }


    public async Task<bool> StudentExistsAsync(int studentId)
    {
        return await _context.Students.AnyAsync(s => s.StudentId == studentId);
    }
}

public interface IUserRepository
{
    Task<Student> GetStudentByIdAsync(int studentId);
    Task<bool> StudentExistsAsync(int studentId);
    Task<string> AddStudent(AddStudent student);
}
