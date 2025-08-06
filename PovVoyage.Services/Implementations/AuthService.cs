using PovVoyage.Services.Models;
using PovVoyage.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PovVoyage.Services.Implementations
{
    public class AuthService : IAuthService
    {
        public async Task<string> LoginAsync(LoginDto loginDto)
        { 
            
            throw new NotImplementedException(); 
        }
        public async Task<string> RegisterAsync(RegisterDto registerDto)
        { 
            throw new NotImplementedException();
        }
    }
}
