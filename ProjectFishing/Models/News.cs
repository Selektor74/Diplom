using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectFishing.Models
{
    public class News
    {
        public int NewsId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public List<Image> Images { get; set; }
        public string Date { get; set; }
        public string UserName { get; set; }
        public News()
        {
            Images = new List<Image>();
        }
    }
}