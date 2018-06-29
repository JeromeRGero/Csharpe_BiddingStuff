using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LoginRegDemo.Models
{
    public class Prod
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProdId {get; set;}

        [ForeignKey("User")]
        public int PostersId {get; set;}


        [Required(ErrorMessage = "Product Name Required")]
        public string ProductName {get; set;}
        

        [Required(ErrorMessage = "Minimum len must be at lease 10 characters!")]
        [MinLength(10)]
        public string Description {get; set;}

        
        public float StartingBid {get; set;}


        [Required(ErrorMessage = "Future End Date is Required")]
        public DateTime EndDate {get; set;}


        public int BidderId {get; set;}

        public string BidderName {get; set;}

        public User User {get; set;}
    }
}
