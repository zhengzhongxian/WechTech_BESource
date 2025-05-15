using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Users
{
    public class CustomerDTO
    {
        public string CustomerId { get; set; }
        public string Surname { get; set; }
        public string MiddleName { get; set; }
        public string FirstName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string? Avatar { get; set; }
        public DateTime Dob { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public int? Coupoun { get; set; }
    }
}
