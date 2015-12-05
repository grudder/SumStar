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

		public static ApplicationDbContext Create()
		{
			return new ApplicationDbContext();
		}
	}
}