using System;
using System.ComponentModel.DataAnnotations.Schema;
using Forest_Rangers.Areas.Identity.Data;
using Forest_Rangers.Models;

namespace Forest_Rangers.Models
{
    public class Comment
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public String ImagePath { get; set; }
        public DateTime? Created_at { get; set; }
        public DateTime? Updated_at { get; set; }

        [ForeignKey("Forest_RangersUser")]
        public string Forest_RangersUserId { get; set; }
        public Forest_RangersUser Forest_RangersUser { get; set; }

        [ForeignKey("Post")]
        public string PostId { get; set; }
        public Post Post { get; set; }
    }
}