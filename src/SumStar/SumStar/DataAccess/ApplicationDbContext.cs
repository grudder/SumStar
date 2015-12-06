using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

using Microsoft.AspNet.Identity.EntityFramework;

using SumStar.Models;

namespace SumStar.DataAccess
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext()
			: base("DefaultConnection", false)
		{
		}

		public DbSet<Category> Categories
		{
			get;
			set;
		}

		public DbSet<Article> Articles
		{
			get;
			set;
		}

		public DbSet<OperationLog> OperationLogs
		{
			get;
			set;
		}

		public static ApplicationDbContext Create()
		{
			return new ApplicationDbContext();
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			// 对外键不进行级联删除
			modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

			base.OnModelCreating(modelBuilder);
		}
	}
}
