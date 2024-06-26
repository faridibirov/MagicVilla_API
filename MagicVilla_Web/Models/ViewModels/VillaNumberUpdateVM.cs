﻿using MagicVilla_Web.Models.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MagicVilla_Web.Models.ViewModels;

public class VillaNumberUpdateVM
{
	public VillaNumberUpdateVM()
	{
		VillaNumberUpdateDTO = new VillaNumberUpdateDTO();
	}

	public VillaNumberUpdateDTO VillaNumberUpdateDTO { get; set; }

	[ValidateNever]
	public IEnumerable<SelectListItem> VillaList { get; set; }
}
