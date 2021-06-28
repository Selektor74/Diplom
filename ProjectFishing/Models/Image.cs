using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectFishing.Models
{
    public class Image
    {
        public int ImageId { get; set; }
        public string Name { get; set; }
        public Post Post { get; set; }
        public News News { get; set; }
    }
}