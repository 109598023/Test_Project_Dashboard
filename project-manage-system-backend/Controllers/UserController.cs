﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using project_manage_system_backend.Dtos;
using project_manage_system_backend.Models;
using project_manage_system_backend.Services;
using project_manage_system_backend.Shares;
using System;

namespace project_manage_system_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(PMSContext dbContext)
        {
            _userService = new UserService(dbContext);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetUser()
        {
            return Ok(_userService.GetUser(User.Identity.Name));
        }

        [Authorize]
        [HttpGet("admin")]
        public IActionResult GetAllUser()
        {
            return Ok(_userService.GetAllUserNotInclude(User.Identity.Name));
        }

        [Authorize]
        [HttpDelete]
        public IActionResult DeleteUser(string account)
        {
            try
            {
                if (_userService.IsAdmin(User.Identity.Name))
                {
                    _userService.DeleteUserByAccount(account);
                    return Ok(new ResponseDto { success = true, message = "delete success!" });
                }
            }
            catch (System.Exception e)
            {
                return Ok(new ResponseDto { success = false, message = $"delete fail：{e.Message}" });
            }
            return BadRequest();
        }

        [Authorize]
        [HttpPost("edit")]
        public IActionResult EditUserName(UserInfoDto userinfoDto)
        {
            try
            {
                _userService.EditUserName(User.Identity.Name, userinfoDto.Name);
                return Ok(new ResponseDto
                {
                    success = true,
                    message = "已成功修改"
                });
            }
            catch (Exception ex)
            {
                return Ok(new ResponseDto
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [Authorize]
        [HttpPatch("{accounr}")]
        public IActionResult EditUserInfo(string accounr, [FromBody] JsonPatchDocument<User> newUserInfo)
        {
            if (_userService.IsAdmin(User.Identity.Name))
            {
                if (_userService.EditUserInfo(accounr, newUserInfo))
                {
                    return Ok(new ResponseDto
                    {
                        success = true,
                        message = "已成功修改"
                    });
                }

                return Ok(new ResponseDto
                {
                    success = false,
                    message = "修改失敗"
                });
            }
            return BadRequest("Who are you?");
        }
    }
}
