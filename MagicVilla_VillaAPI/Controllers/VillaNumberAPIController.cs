
using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers;

[Route("api/VillaNumberAPI")]
[ApiController]
[ApiVersion("1.0")]
public class VillaNumberAPIController : ControllerBase
{
	protected APIResponse _response;
	private readonly IVillaNumberRepository _dbVillaNumber;
	private readonly IVillaRepository _dbVilla;
	private readonly IMapper _mapper;

	public VillaNumberAPIController(IVillaNumberRepository dbVillaNumber, IMapper mapper, IVillaRepository dbVilla)
	{
		_dbVillaNumber = dbVillaNumber;
		_mapper = mapper;
		this._response = new();
		_dbVilla = dbVilla;
	}

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult<APIResponse>> GetVillaNumbers()
	{
		try
		{

			IEnumerable<VillaNumber> villaNumberList = await _dbVillaNumber.GetAllAsync(includeProperties:"Villa");
			_response.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumberList);
			_response.StatusCode = HttpStatusCode.OK;
			return Ok(_response);
		}

		catch (Exception ex)
		{
			_response.IsSuccess = false;
			_response.ErrorMessages = new List<string>() { ex.ToString() };
		}

		return _response;
	}

	[HttpGet("{villaNo:int}", Name = "GetVillaNumber")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<APIResponse>> GetVillaNumbers(int villaNo)
	{
		try
		{

			if (villaNo == 0)
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				return BadRequest(_response);
			}
			var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == villaNo);

			if (villaNumber == null)
			{
				_response.StatusCode = HttpStatusCode.NotFound;

				return NotFound(_response);
			}

			_response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
			_response.StatusCode = HttpStatusCode.OK;
			return Ok(_response);

		}

		catch (Exception ex)
		{
			_response.IsSuccess = false;
			_response.ErrorMessages = new List<string>() { ex.ToString() };
		}

		return _response;
	}

	[HttpPost]
	[Authorize(Roles = "admin")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createDTO)
	{
		try
		{

			if (await _dbVillaNumber.GetAsync(u => u.VillaNo == createDTO.VillaNo) != null)
			{
				ModelState.AddModelError("ErrorMessages", "Villa Number already exist!");

				return BadRequest(ModelState);
			}

			if (await _dbVilla.GetAsync(u => u.Id == createDTO.VillaID)==null)
			{

				ModelState.AddModelError("ErrorMessages", "Villa ID is invalid!");

				return BadRequest(ModelState);
			}

			if (createDTO == null)
			{
				return BadRequest(createDTO);
			}

			VillaNumber villaNumber = _mapper.Map<VillaNumber>(createDTO);

			await _dbVillaNumber.CreateAsync(villaNumber);

			_response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
			_response.StatusCode = HttpStatusCode.Created;
			return CreatedAtRoute("GetVillaNumber", new { VillaNo = villaNumber.VillaNo }, _response);
		}

		catch (Exception ex)
		{
			_response.IsSuccess = false;
			_response.ErrorMessages = new List<string>() { ex.ToString() };
		}

		return _response;
	}

	[HttpDelete("{villaNo:int}", Name = "DeleteVillaNumber")]
	[Authorize(Roles = "admin")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int villaNo)
	{
		try
		{

			if (villaNo == 0)
			{
				return BadRequest();
			}

			var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == villaNo);

			if (villaNumber == null)
			{
				return NotFound();
			}
			await _dbVillaNumber.RemoveAsync(villaNumber);

			_response.StatusCode = HttpStatusCode.NoContent;
			_response.IsSuccess = true;

			return Ok(_response);

		}

		catch (Exception ex)
		{
			_response.IsSuccess = false;
			_response.ErrorMessages = new List<string>() { ex.ToString() };
		}

		return _response;
	}


	[HttpPut("{villaNo:int}", Name = "UpdateVillaNumber")]
	[Authorize(Roles = "admin")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int villaNo, [FromBody] VillaNumberUpdateDTO updateDTO)
	{
		try
		{

			if (updateDTO == null || villaNo != updateDTO.VillaNo)
			{
				return BadRequest();
			}


			if (await _dbVilla.GetAsync(u => u.Id == updateDTO.VillaID) == null)
			{

				ModelState.AddModelError("ErrorMessages", "Villa ID is invalid!");

				return BadRequest(ModelState);
			}

			VillaNumber villaNumber = _mapper.Map<VillaNumber>(updateDTO);

			await _dbVillaNumber.UpdateAsync(villaNumber);

			_response.StatusCode = HttpStatusCode.NoContent;
			_response.IsSuccess = true;
			return Ok(_response);
		}

		catch (Exception ex)
		{
			_response.IsSuccess = false;
			_response.ErrorMessages = new List<string>() { ex.ToString() };
		}

		return _response;
	}

}
