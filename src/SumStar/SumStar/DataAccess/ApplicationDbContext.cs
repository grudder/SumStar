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

		public DbSet<OperationLog> OperationLogs
		{
			get;
			set;
		}

		public DbSet<Category> Categories
		{
			get;
			set;
		}

		public DbSet<Content> Contents
		{
			get;
			set;
		}

		public DbSet<ArticleContent> ArticleContents
		{
			get;
			set;
		}

		public DbSet<ImageContent> ImageContents
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

			modelBuilder.Entity<ArticleContent>()
				.Map(m =>
				{
					m.MapInheritedProperties();
					m.ToTable("T_ArticleContent");
				});
			modelBuilder.Entity<ImageContent>()
				.Map(m =>
				{
					m.MapInheritedProperties();
					m.ToTable("T_ImageContent");
				});

			base.OnModelCreating(modelBuilder);
		}
	}
}
