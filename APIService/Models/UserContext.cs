using Microsoft.EntityFrameworkCore;

namespace APIService.Model
{
	public class UserContext : DbContext
	{
		public virtual DbSet<User> Users { get; set; } = null!;

		public UserContext(DbContextOptions<UserContext> options) : base(options)
		{
			Database.EnsureCreated();
		}

		public UserContext()
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>().HasData(
				new User() { Id = 1, Login = "test", Password = "test", API_KEY = "123" });
		}
	}
}