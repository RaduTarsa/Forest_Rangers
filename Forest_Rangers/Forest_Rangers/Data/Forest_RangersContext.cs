using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Forest_Rangers.Models;

namespace Forest_Rangers.Data
{
    public class Forest_RangersContext : DbContext
    {
        public Forest_RangersContext (DbContextOptions<Forest_RangersContext> options)
            : base(options)
        {
        }

        public DbSet<Forest_Rangers.Models.Post> Post { get; set; }

        public DbSet<Forest_Rangers.Models.Comment> Comment { get; set; }
    }
}
