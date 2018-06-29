using Microsoft.EntityFrameworkCore;
 
namespace LoginRegDemo.Models
{
    public class UserContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }


        public DbSet<User> users {get; set;}


        public DbSet<Prod> prods {get; set;}


        //public object User { get; internal set; }
    }
}