﻿using System;

namespace my_eshop_api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string RegistrationCode { get; set; }

    }
}
