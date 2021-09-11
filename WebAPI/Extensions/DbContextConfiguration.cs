using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Extensions
{
    public static class DbContextConfiguration
    {
        public static void AddDbContextSQLite(this IServiceCollection services, IWebHostEnvironment env, IConfiguration config)
        {
            if (env.IsProduction())
            {
                //configure dbcontext for production env
            }
            else
            {
                //Configure sqlite if the environment is development
                string SqliteConnectionString = config.GetConnectionString("SQLiteConnection");
                services.AddDbContextPool<AppDbContext>(options => options.UseSqlite(SqliteConnectionString));
            }
        }

        public static void AddDbContextSQLServer(this IServiceCollection services, IWebHostEnvironment env, IConfiguration config)
        {
            if (env.IsProduction())
            {
                //configure dbcontext for production env
            }
            else
            {
                //Configure SQLServer if the environment is development
                string SqlServereConnectionString = config.GetConnectionString("SQLServerConnection");
                services.AddDbContext<AppDbContext>(options => options.UseSqlServer(SqlServereConnectionString));
            }
        }

        public static void AddDbContextMySQL(this IServiceCollection services, IWebHostEnvironment env, IConfiguration config)
        {
            if (env.IsProduction())
            {
                //configure dbcontext for production env
            }
            else
            {
                //Configure MySQL if the environment is development
                string MySqlConnectionString = config.GetConnectionString("MySQLConnection");
                services.AddDbContext<AppDbContext>(options => options
                .UseMySql(MySqlConnectionString, ServerVersion.AutoDetect(MySqlConnectionString)));
            }
        }
    }
}
