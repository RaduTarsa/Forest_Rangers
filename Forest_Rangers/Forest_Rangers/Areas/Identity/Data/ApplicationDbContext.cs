using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forest_Rangers.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Forest_Rangers.Data
{
    public class ApplicationDbContext : IdentityDbContext<Forest_RangersUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Forest_Rangers.Models.Post> Post { get; set; }

        public DbSet<Forest_Rangers.Models.Comment> Comment { get; set; }
    }
}
