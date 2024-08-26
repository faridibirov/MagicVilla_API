﻿using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers;


[Route("api/v{version:ApiVersion}/UsersAuth")]
[ApiController]
[ApiVersionNeutral]
public class UsersController : Controller
{
    private readonly IUserRepository _userRepository;
    protected APIResponse _response;
    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _response = new();
    }

    [HttpGet("Error")]
	public async Task<IActionResult> Error()
    {
        throw new FileNotFoundException();
    }

	[HttpGet("ImageError")]
	public async Task<IActionResult> ImageError()
	{
		throw new BadImageFormatException("Fake Image Exception");
	}


	[HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
    {
        var tokenDTO = await _userRepository.Login(model);

        if (tokenDTO == null || string.IsNullOrEmpty(tokenDTO.AccessToken))
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Username or password is incorrect");

            return BadRequest(_response);
        }

        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        _response.Result = tokenDTO;
        return Ok(_response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO model)
    {
        bool isUserNameUnique = _userRepository.IsUniqueUser(model.UserName);

        if (!isUserNameUnique)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Username already exists");
            return BadRequest(_response);
        }


        var user = await _userRepository.Register(model);

        if (user == null)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Error while registering");
            return BadRequest(_response);
        }


        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        //_response.Result = user;

        return Ok(_response);

    }

	[HttpPost("refresh")]
	public async Task<IActionResult> GetNewTokenFromRefreshToken([FromBody] TokenDTO tokenDTO)
	{
        if (ModelState.IsValid)
        {
            var tokenDTOResponse = await _userRepository.RefreshAccessToken(tokenDTO);

			if (tokenDTOResponse==null || string.IsNullOrEmpty(tokenDTOResponse.AccessToken))
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("Token Invalid");
				return BadRequest(_response);
			}

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = tokenDTOResponse;

            return Ok(_response);
		}

        else
        {
            _response.IsSuccess = false;
            _response.Result = "Invalid Input";
            return BadRequest(_response);
        }

	}


	[HttpPost("revoke")]
	public async Task<IActionResult> RevokeRefreshToken([FromBody] TokenDTO tokenDTO)
	{
		
		if (ModelState.IsValid)
		{
            await _userRepository.RevokeRefreshToken(tokenDTO);
			_response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
		}

		_response.IsSuccess = false;
        _response.Result = "Invalid Input";

		return BadRequest(_response);

	}

}
