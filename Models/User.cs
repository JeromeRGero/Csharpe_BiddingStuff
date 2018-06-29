using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LoginRegDemo.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idUsers {get; set;}


        [Required(ErrorMessage   = "Must have first name")]
        [MinLength(2)]
        public string FirstName {get; set;}
        

        [Required(ErrorMessage = "Must have last name")]
        [MinLength(2)]
        public string LastName {get; set;}


        [MinLength(2)]
        [Required(ErrorMessage = "Must have email")]
        public string Email {get; set;}


        [Required(ErrorMessage = "Must have must have password")]
        [MinLength(8)]
        public string Password {get; set;}

        public float Wallet {get; set;}

        public List<Prod> usersPosts {get; set;}

        public User() {
            usersPosts = new List<Prod>();
        }

    }
}

