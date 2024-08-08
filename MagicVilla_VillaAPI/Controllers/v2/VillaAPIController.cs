﻿using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace MagicVilla_VillaAPI.Controllers.v2;

[Route("api/v{version:ApiVersion}/VillaAPI")]
[ApiController]
[ApiVersion("2.0")]
public class VillaAPIController : ControllerBase
{
    protected APIResponse _response;
    private readonly IVillaRepository _dbVilla;
    private readonly IMapper _mapper;

    public VillaAPIController(IVillaRepository dbVilla, IMapper mapper)
    {
        _dbVilla = dbVilla;
        _mapper = mapper;
        _response = new();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<APIResponse>> GetVillas([FromQuery(Name = "filterOccupancy")] int? occupancy,
        [FromQuery] string? search, int pageSize = 0, int pageNumber = 1)
    {
        try
        {

            IEnumerable<Villa> villaList;

            if (occupancy > 0)
            {
                villaList = await _dbVilla.GetAllAsync(u => u.Occupancy == occupancy, pageSize: pageSize,
                    pageNumber: pageNumber);
            }

            else
            {
                villaList = await _dbVilla.GetAllAsync(pageSize: pageSize,
                    pageNumber: pageNumber);
            }

            if (!string.IsNullOrEmpty(search))
            {
                villaList = villaList.Where(u => u.Name.ToLower().Contains(search));
            }

            Pagination pagination = new() { PageNumber = pageNumber, PageSize = pageSize };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));
            _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
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

    [HttpGet("{id:int}", Name = "GetVilla")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse>> GetVilla(int id)
    {
        try
        {

            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            var villa = await _dbVilla.GetAsync(u => u.Id == id);

            if (villa == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;

                return NotFound(_response);
            }

            _response.Result = _mapper.Map<VillaDTO>(villa);
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
    public async Task<ActionResult<APIResponse>> CreateVilla([FromForm] VillaCreateDTO createDTO)
    {
        try
        {
        
            if (await _dbVilla.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("ErrorMessages", "Villa already exist!");

                return BadRequest(ModelState);
            }

            if (createDTO == null)
            {
                return BadRequest(createDTO);
            }


            Villa villa = _mapper.Map<Villa>(createDTO);

            await _dbVilla.CreateAsync(villa);

            if(createDTO.Image!=null)
            {
                string fileName = villa.Id + Path.GetExtension(createDTO.Image.FileName);
                string filePath = @"wwwroot\ProductImage\" + fileName;

                var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                FileInfo file = new FileInfo(directoryLocation);

                if(file.Exists)
                {
                    file.Delete();
                }

                using (FileStream fileStream = new FileStream(directoryLocation, FileMode.Create))
                {
                    createDTO.Image.CopyTo(fileStream);
                }

                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";

                villa.ImageUrl = baseUrl + "/ProductImage/" +fileName;
                villa.ImageLocalPath = filePath;
            }

            else
            {
                villa.ImageUrl = "https://placehold.co/600x400";
            }

            await _dbVilla.UpdateAsync(villa);
            _response.Result = _mapper.Map<VillaDTO>(villa);
            _response.StatusCode = HttpStatusCode.Created;
            return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
        }

        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }

    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
    {
        try
        {

            if (id == 0)
            {
                return BadRequest();
            }

            var villa = await _dbVilla.GetAsync(u => u.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

			if (!string.IsNullOrEmpty(villa.ImageLocalPath))
			{
				var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), villa.ImageLocalPath);

				FileInfo file = new FileInfo(oldFilePathDirectory);

				if (file.Exists)
				{
					file.Delete();
				}
			}

			await _dbVilla.RemoveAsync(villa);

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


    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromForm] VillaUpdateDTO updateDTO)
    {
        try
        {

            if (updateDTO == null || id != updateDTO.Id)
            {
                return BadRequest();
            }

            Villa model = _mapper.Map<Villa>(updateDTO);

			if (updateDTO.Image != null)
			{

                if(!string.IsNullOrEmpty(model.ImageLocalPath))
                {
                    var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), model.ImageLocalPath);

					FileInfo file = new FileInfo(oldFilePathDirectory);

					if (file.Exists)
					{
						file.Delete();
					}
				}

				string fileName = updateDTO.Id + Path.GetExtension(updateDTO.Image.FileName);
				string filePath = @"wwwroot\ProductImage\" + fileName;

				var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

				

				using (FileStream fileStream = new FileStream(directoryLocation, FileMode.Create))
				{
					updateDTO.Image.CopyTo(fileStream);
				}

				var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";

				model.ImageUrl = baseUrl + "/ProductImage/" + fileName;
				model.ImageLocalPath = filePath;
			}

			else
			{
				model.ImageUrl = "https://placehold.co/600x400";
			}

			await _dbVilla.UpdateAsync(model);

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

    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
    {
        if (patchDTO == null || id == 0)
        {
            return BadRequest();
        }

        var villa = await _dbVilla.GetAsync(u => u.Id == id, tracked: false);

        VillaUpdateDTO updateDTO = _mapper.Map<VillaUpdateDTO>(villa);


        if (villa == null)
        {
            return BadRequest();
        }

        patchDTO.ApplyTo(updateDTO, ModelState);

        Villa model = _mapper.Map<Villa>(updateDTO);

        await _dbVilla.UpdateAsync(model);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return NoContent();

    }

}
