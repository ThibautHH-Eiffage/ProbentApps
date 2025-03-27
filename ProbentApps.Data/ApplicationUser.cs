using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace ProbentApps.Data;

public class ApplicationUser : IdentityUser<Guid>;
