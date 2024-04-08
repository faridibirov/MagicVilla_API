using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Models.ViewModels;
using MagicVilla_Web.Services;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MagicVilla_Web.Controllers;

public class VillaNumberController : Controller
{

	private readonly IVillaNumberService _villaNumberService;
	private readonly IVillaService _villaService;
	private readonly IMapper _mapper;

	public VillaNumberController(IVillaNumberService villaNumberService, IVillaService villaService,  IMapper mapper)
	{
		_villaNumberService = villaNumberService;
		_mapper = mapper;
		_villaService = villaService;
	}

	public async Task<IActionResult> IndexVillaNumber()
	{
		List<VillaNumberDTO> list = new();

		var response = await _villaNumberService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
		if (response != null && response.IsSuccess)
		{
			list = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Result));
		}

		return View(list);
	}

	[Authorize(Roles = "admin")]
	public async Task<IActionResult> CreateVillaNumber()
	{
		VillaNumberCreateVM villaNumberCreateVM = new();

		var response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
		if (response != null && response.IsSuccess)
		{
			villaNumberCreateVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
				(Convert.ToString(response.Result)).Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				}); 
		}

		return View(villaNumberCreateVM);
	}

	[Authorize(Roles = "admin")]
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateVM model)
	{
		if (ModelState.IsValid)
		{
			var response = await _villaNumberService.CreateAsync<APIResponse>(model.VillaNumberCreateDTO, HttpContext.Session.GetString(SD.SessionToken));
			if (response != null && response.IsSuccess)
			{
				return RedirectToAction(nameof(IndexVillaNumber));
			}

			else
			{
				if(response.ErrorMessages.Count>0)
				{
					ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
				}
			}
		}

		var resp = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
		if (resp != null && resp.IsSuccess)
		{
			model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
				(Convert.ToString(resp.Result)).Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				});
		}

		return View(model);

	}

	[Authorize(Roles = "admin")]
	public async Task<IActionResult> UpdateVillaNumber(int villaNo)
	{
		VillaNumberUpdateVM villaNumberUpdateVM = new();

		var response = await _villaNumberService.GetAsync<APIResponse>(villaNo, HttpContext.Session.GetString(SD.SessionToken));
		if (response != null && response.IsSuccess)
		{
			VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
			villaNumberUpdateVM.VillaNumberUpdateDTO = _mapper.Map<VillaNumberUpdateDTO>(model);
		}

		 response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
		if (response != null && response.IsSuccess)
		{
			villaNumberUpdateVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
				(Convert.ToString(response.Result)).Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				});

			return View(villaNumberUpdateVM);
		}

		return NotFound();
	}

	[Authorize(Roles = "admin")]
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdateVM model)
	{
		if (ModelState.IsValid)
		{
			var response = await _villaNumberService.UpdateAsync<APIResponse>(model.VillaNumberUpdateDTO, HttpContext.Session.GetString(SD.SessionToken));
			if (response != null && response.IsSuccess)
			{
				return RedirectToAction(nameof(IndexVillaNumber));
			}
			else
			{
				if (response.ErrorMessages.Count > 0)
				{
					ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
				}
			}
		}

		var resp = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
		if (resp != null && resp.IsSuccess)
		{
			model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
				(Convert.ToString(resp.Result)).Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				});
		}

		return View(model);
	}

	[Authorize(Roles = "admin")]
	public async Task<IActionResult> DeleteVillaNumber(int villaNo)
	{
		VillaNumberDeleteVM villaNumberDeleteVM = new();

		var response = await _villaNumberService.GetAsync<APIResponse>(villaNo, HttpContext.Session.GetString(SD.SessionToken));
		if (response != null && response.IsSuccess)
		{
			VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
			villaNumberDeleteVM.VillaNumberDTO = model;
		}

		response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
		if (response != null && response.IsSuccess)
		{
			villaNumberDeleteVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
				(Convert.ToString(response.Result)).Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				});

			return View(villaNumberDeleteVM);
		}

		return NotFound();
	}

	[Authorize(Roles = "admin")]
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteVillaNumber(VillaNumberDeleteVM model)
	{

		var response = await _villaNumberService.DeleteAsync<APIResponse>(model.VillaNumberDTO.VillaNo, HttpContext.Session.GetString(SD.SessionToken));
		if (response != null && response.IsSuccess)
		{
			return RedirectToAction(nameof(IndexVillaNumber));
		}

		return View(model);
	}


}
