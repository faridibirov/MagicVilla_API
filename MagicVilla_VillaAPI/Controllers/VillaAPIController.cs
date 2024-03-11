using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers;

[Route("api/VillaAPI")]
[ApiController]
public class VillaAPIController : ControllerBase
{
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public ActionResult <IEnumerable<VillaDTO>> GetVillas()
	{
		return Ok(VillaStore.VillaList);
	}

	[HttpGet("{id:int}", Name ="GetVilla")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public ActionResult<VillaDTO> GetVilla(int id)
	{
		if(id==0)
		{
			return BadRequest();
;		}
		var villa = VillaStore.VillaList.FirstOrDefault(u => u.Id == id);

		if (villa==null)
		{
			return NotFound();
		}

		return Ok(villa);
	}

	[HttpPost]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public ActionResult<VillaDTO>  CreateVilla([FromBody]VillaDTO villaDTO)
	{
		if(villaDTO==null)
		{
			return BadRequest(villaDTO);
		}

		if(villaDTO.Id>0)
		{
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		villaDTO.Id = VillaStore.VillaList.OrderByDescending(u => u.Id).FirstOrDefault().Id+1;

		VillaStore.VillaList.Add(villaDTO);

		return CreatedAtRoute("GetVilla", new { id =villaDTO.Id}, villaDTO );

	}
}
