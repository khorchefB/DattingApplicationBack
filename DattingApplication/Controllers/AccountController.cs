﻿using DattingApplication.Data;
using DattingApplication.DTOs;
using DattingApplication.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DattingApplication.Controllers
{
    public class AccountController : BaseController
    {
        private readonly DataContext Context;
        public AccountController(DataContext context)
        {
            Context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            if (await UserExist(registerDto.UserName)) return BadRequest("UserName is taken");
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            Context.Users.Add(user);
            await Context.SaveChangesAsync();

            return user;
        }
        
        private async Task<bool> UserExist(string userName)
        {
            return await Context.Users.AnyAsync(u => u.UserName.Equals(userName.ToLower()));
        }
    }
}
