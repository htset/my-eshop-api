using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace my_eshop_api.Models
{
    public class OrderDTO
    {
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        [Required]
        public List<OrderDetailDTO> OrderDetails { get; set; }
        [Required]
        public CreditCardDTO CreditCard { get; set; }
        [Required]
        public int DeliveryAddressId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
