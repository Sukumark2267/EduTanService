using Microsoft.EntityFrameworkCore;
using PovVoyage.Data.Context;
using PovVoyage.Data.Entities;
using PovVoyage.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PovVoyage.Services.Implementations
{
    public class UserInteractionService : IUserInteractionService
    {
        private readonly PovVoyageContext _context;
        public UserInteractionService(PovVoyageContext context) 
        { 
            _context = context;
        }

        public async Task<User> RegisterUserAsync(User user) 
        { 
            user.UserId = Guid.NewGuid(); 
            await _context.Users.AddAsync(user); 
            await _context.SaveChangesAsync(); 
            return user; 
        }

        public async Task<User> LoginUserAsync(string username, string password) 
        {
           // username = "dpatil";
           // password = "123"; //need to comment before deploy
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
            if (user == null) 
            { 
                throw new Exception("Invalid username or password."); 
            } 
            return user; 
        }

        public async Task<User> UpdateUserProfileAsync(User user) 
        { 
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId); 
            if (existingUser == null) 
            { 
                throw new Exception("User not found."); 
            } 
            existingUser.Username = user.Username;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email; 
            existingUser.Mobile = user.Mobile;
            existingUser.Password = user.Password;
            _context.Users.Update(existingUser); 
            await _context.SaveChangesAsync();
            return existingUser; 
        }
    }
}
