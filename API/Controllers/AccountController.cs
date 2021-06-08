using API.Errors;
using AutoMapper;
using Entities.Dtos;
using Entities.Interfaces;
using Entities.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, 
                ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var user = await _userManager.FindByEmailAsync(email);

            return Ok(new UserDto
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = _tokenService.CreateToken(user)
            });
        }

        [Authorize]
        [HttpGet("address")]
        public async Task<IActionResult> GetUserAddress()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var user = await _userManager.Users.Include(u => u.Address).FirstOrDefaultAsync(x => x.Email == email);

            return Ok(_mapper.Map<Address, AddressDto>(user.Address));
        }

        [Authorize]
        [HttpPut("address")]
        public async Task<IActionResult> UpdateUserAddress(AddressDto address)
        { 
            var email = User.FindFirstValue(ClaimTypes.Email);

            var user = await _userManager.Users.Include(u => u.Address).FirstOrDefaultAsync(x => x.Email == email);

            user.Address = _mapper.Map<AddressDto, Address>(address);
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Ok(_mapper.Map<Address, AddressDto>(user.Address));

            return BadRequest("Could not update user address");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
                return Unauthorized(new ApiResponse(401));

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
                return Unauthorized(new ApiResponse(401));

            return Ok(new UserDto 
                { 
                    Email = user.Email, 
                    DisplayName = user.DisplayName, 
                    Token = _tokenService.CreateToken(user)
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (await CheckEmailAsync(registerDto.Email))
                return BadRequest(new ApiValidationErrorResponse { Errors = new[] { "Email address is in use" } });

            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400));

            return Ok(new UserDto 
                { 
                    DisplayName = user.DisplayName, 
                    Email = user.Email, 
                    Token = _tokenService.CreateToken(user)
            });
        }

        [HttpGet("emailexists")]
        public async Task<bool> CheckEmailAsync([FromQuery] string email) => await _userManager.FindByEmailAsync(email) != null;

    }
}
