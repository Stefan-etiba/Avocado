using System.ComponentModel.DataAnnotations;

namespace Domain.Requests;

public class RegisterModel
{
    [Required(ErrorMessage = "User Name is required")]
    public string Username { get; set; }

    [EmailAddress]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }


    [Required(ErrorMessage = "FirstName is required")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "LastName is required")]
    public string LastName { get; set; }


    [Required(ErrorMessage = "PhoneNumber is required")]
    public string PhoneNumber { get; set; }
}

public class AddStudent
{
    public int StudentId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}