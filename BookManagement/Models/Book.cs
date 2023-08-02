using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BookManagement.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required, DisplayName("Title")]
        public string Name { get; set; }

        [Required, DisplayName("Author")]
        public string Authors { get; set; }

        [Required, DisplayName("Publisher")]
        public string Publisher { get; set; }

        [DisplayName("Publishing year")]
        public int Year { get; set; }

        [DisplayName("File")]
        public string? DataFile { get; set; }
    }
}

