using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Obrazovashka.DTOs;
using Obrazovashka.Services;

namespace Obrazovashka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebugController : ControllerBase
    {
        private readonly IUserService _userService;

        public DebugController(IUserService userService)
        {
            _userService = userService;
        }


    }
}
