﻿using MAgicVilla_VillaAPI.Models;
using MAgicVilla_VillaAPI.Models.Dto;
using MAgicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MAgicVilla_VillaAPI.Controllers;

[Route("api/v{version:apiVersion}/UsersAuth")]
[ApiController]
[ApiVersionNeutral]
public class UsersController(IUserRepository userRepo) : ControllerBase
{
    private readonly IUserRepository _userRepo = userRepo;
    private readonly APIResponse _response = new();

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
    {
        var loginResponse = await _userRepo.Login(model);

        if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Username or password is incorrect");
            return BadRequest(_response);
        }

        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        _response.Result = loginResponse;
        return Ok(_response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO model)
    {
        if (!_userRepo.IsUniqueUser(model.UserName))
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Username already exists");
            return BadRequest(_response);
        }

        var user = await _userRepo.Register(model);

        if (user == null)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Error while registering");
            return BadRequest(_response);
        }

        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        _response.Result = user;
        return Ok(_response);
    }
}