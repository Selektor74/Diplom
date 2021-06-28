using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectFishing.Models
{
    public class Ad:Post
    {

        public int CategoryId { get; set; }

        [DisplayName("Телефонный номер")]
        [DataType(DataType.PhoneNumber)]
        [Required]
        public string PhoneNumber { get; set; }
        public bool Closed { get; set; }

        [DisplayName("Цена")]
        [Required]
        public decimal Price { get; set; }
        
        public string Date { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }

        public int ViewCount { get; set; }

    }
}