using System.ComponentModel.DataAnnotations;

public class BookTable
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [Display(Name = "Your Name")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [DataType(DataType.Time)]
    [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
    public DateTime Time { get; set; } // Change from TimeSpan to DateTime

    [Required]
    [Range(1, 20, ErrorMessage = "Please select between 1 and 20 guests")]
    [Display(Name = "Number of Guests")]
    public int NumberOfGuests { get; set; }

    [Display(Name = "Special Requests")]
    public string SpecialRequests { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";

    [Display(Name = "Booking Date and Time")]
    public DateTime BookingDateTime { get; set; }
}