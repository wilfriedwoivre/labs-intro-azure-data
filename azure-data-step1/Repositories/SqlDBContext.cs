using Microsoft.EntityFrameworkCore;
using Soat.Masterclass.Labs.Models.Sql;

namespace Soat.Masterclass.Labs.Repositories
{
    public class SqlDBContext : DbContext
    {
        public SqlDBContext (DbContextOptions<SqlDBContext> options)
            : base(options)
        {
        }

        public DbSet<Todo> Todo { get; set; }
    }
}