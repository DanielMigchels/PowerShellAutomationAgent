using PowerShellAutomationAgent.API.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace PowerShellAutomationAgent.API.Data;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Secret> Secrets { get; set; }
}
