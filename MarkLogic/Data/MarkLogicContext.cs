using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MarkLogic.Models
{
    public class MarkLogicContext : DbContext
    {
        public MarkLogicContext (DbContextOptions<MarkLogicContext> options)
            : base(options)
        {
        }

        public DbSet<MarkLogic.Models.student> student { get; set; }
    }
}
