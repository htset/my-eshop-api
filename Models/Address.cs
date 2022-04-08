using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my_eshop_api.Models
{
    public class Address
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

    }
}
