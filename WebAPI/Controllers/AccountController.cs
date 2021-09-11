using AutoMapper;
using Core.Models.Entities;
using Infrastructure.DTO;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AccountController(IMapper mapper, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<UserDto>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsers();
            if (users == null)
                return NotFound();

            var usersToReturn = new List<UserDto>();
            foreach (var item in users)
            {
                var itemToAdd = new UserDto
                {
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    EmailAddress = item.EmailAddress,
                };
                usersToReturn.Add(itemToAdd);
            }
            return Ok(usersToReturn);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(UserDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var userToCreate = new AppUser
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                EmailAddress = userDto.EmailAddress,
            };
            var createdUser = await _userRepository.CreateAsync(userToCreate);
            if (createdUser == null)
                return StatusCode(400);

            return Ok(createdUser);

            //return (IActionResult)_responseFormat.failed("Failed to create user");
        }
    }
}
