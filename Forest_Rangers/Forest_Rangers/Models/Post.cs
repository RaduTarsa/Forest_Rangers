using Forest_Rangers.Areas.Identity.Data;
using Forest_Rangers.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forest_Rangers.Models
{
    public class Post
    {
        public Post()
        {
            this.Comments = new HashSet<Comment>();
        }

        public string Id { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public String ImagePath { get; set; }
        public String CoordinatesLong { get; set; }
        public String CoordinatesLat { get; set; }
        public DateTime? Created_at { get; set; }
        public DateTime? Updated_at { get; set; }

        [ForeignKey("Forest_RangersUser")]
        public string Forest_RangersUserId { get; set; }
        public Forest_RangersUser Forest_RangersUser { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}
