using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Forest_Rangers.Models;
using Forest_Rangers.Models;
using Microsoft.AspNetCore.Identity;

namespace Forest_Rangers.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the Forest_RangersUser class
    public class Forest_RangersUser : IdentityUser
    {
        public Forest_RangersUser()
        {
            this.Posts = new HashSet<Post>();
            this.Comments = new HashSet<Comment>();
        }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }

        public ICollection<Post> Posts { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}
