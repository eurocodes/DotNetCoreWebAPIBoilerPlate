using Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task<int> GetLastUserId();
        Task<AppUser> CreateAsync(AppUser user);
        Task<ICollection<AppUser>> GetAllUsers();
        Task<bool> SaveChangesAsync();
    }
}
