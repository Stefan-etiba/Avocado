using Domain.Entities;
using Domain.Requests;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/students")]
public class StudentController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public StudentController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet("{studentId}")]
    public async Task<IActionResult> GetStudentById(int studentId)
    {
        var student = await _userRepository.GetStudentByIdAsync(studentId);
        if (student == null)
        {
            return NotFound();
        }

        return Ok(student);
    }

    [HttpPost("AddStudent")]
    public async Task<IActionResult> AddNewStudent(AddStudent student)
    {
        try
        {
            var newStudent = await _userRepository.AddStudent(student);
            
            if (newStudent == null)
            {
                return BadRequest();
            }

            return Ok(newStudent);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
    // Add additional methods for other student-related functionalities
    // (e.g., POST to create a student, etc.)
}
