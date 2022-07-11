using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CPMSDbFirst.Models
{
    [Table("Author")]
    public partial class Author
    {
        public Author()
        {
            Papers = new HashSet<Paper>();
        }

        [Key]
        [Column("AuthorID")]
        public int AuthorId { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [StringLength(1)]
        [Display(Name = "Middle Initial")]
        public string? MiddleInitial { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [StringLength(50)]
        public string? Affiliation { get; set; }
        [StringLength(50)]
        public string? Department { get; set; }
        [StringLength(50)]
        public string? Address { get; set; }
        [StringLength(50)]
        public string? City { get; set; }
        [StringLength(2)]
        public string? State { get; set; }
        [StringLength(10)]
        public string ZipCode { get; set; }
        [StringLength(50)]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        [StringLength(100)]
        public string? EmailAddress { get; set; }
        [Required]
        [DataType (DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(5)]
        public string Password { get; set; }

        [InverseProperty("Author")]
        public virtual ICollection<Paper> Papers { get; set; }
    }
}
