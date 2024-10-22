﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using WebApi.Interfaces.Services;
using WebApi.Models.Models.Authentication;
using WebApi.Models.Models.Identity.Attributes.Admin;
using WebApi.Models.Models.Identity.Attributes.Machine;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationService AuthenticationService;
        private readonly IConfiguration _config;

        public AuthenticationController(IAuthenticationService authService, IConfiguration configuration)
        {
            AuthenticationService = authService;
            _config = configuration;
        }

        [HttpPost("machine")]
        public ActionResult<JwtResponse> AuthenticateMachine([FromBody] MachineLoginModel model)
        {
            var secret = _config["Jwt:Key"];

            if (secret != model.AppSecret)
                return Unauthorized();

            var token = AuthenticationService.AuthenticateMachine(model.AppSecret);

            var jwt = new JwtResponse
            {
                AccessToken = token
            };

            return Ok(jwt);
        }

        [HttpPost("user")]
        public async Task<ActionResult<JwtResponse>> AuthenticateUser([FromBody] UserLoginModel model)
        {
            var secret = _config["Jwt:Key"];

            if (secret != model.AppSecret)
                return Unauthorized();

            var response = await AuthenticationService.AuthenticateUser(model.UserId, model.AppSecret);
            if (string.IsNullOrEmpty(response.AccessToken))
                return Unauthorized();

            return Ok(response);
        }

        [HttpGet]
        [Authorize]
        [RequiresMachine]
        public ActionResult Test()
        {
            return Ok();
        }
    }
}
