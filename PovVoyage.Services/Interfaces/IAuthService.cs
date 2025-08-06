using PovVoyage.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PovVoyage.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginDto loginDto); 
        Task<string> RegisterAsync(RegisterDto registerDto);
    }
}
