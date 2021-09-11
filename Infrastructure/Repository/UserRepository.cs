using Core.Models.Entities;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using NetCore.AutoRegisterDi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    [RegisterAsScoped]
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<AppUser> CreateAsync(AppUser user)
        {
            user.Id = await GetLastUserId() + 1;
            await _context.AppUsers.AddAsync(user);

            if (await SaveChangesAsync())
                return user;

            return null;
        }

        public async Task<ICollection<AppUser>> GetAllUsers()
        {
            var users = await _context.AppUsers.ToListAsync();
            if (users != null)
                return users;
            return null;
        }

        public async Task<int> GetLastUserId()
        {
            var users = await _context.AppUsers.OrderBy(u => u.Id).LastOrDefaultAsync();
            if (users == null)
                return 0;
            return users.Id;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() >= 1;
        }
    }
}
