using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PovVoyage.Data.Entities;

namespace PovVoyage.Services.Interfaces
{
    public interface IUserInteractionService
    {
        Task<User> RegisterUserAsync(User user);
        Task<User> LoginUserAsync(string username, string password);

        Task<User> UpdateUserProfileAsync(User user);
    }
}
