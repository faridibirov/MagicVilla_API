﻿using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using System.Runtime.ExceptionServices;

namespace MagicVilla_VillaAPI.Middlewares;

public class CustomExceptionMiddleware
{
	private readonly RequestDelegate _requestDelegate;

	public CustomExceptionMiddleware(RequestDelegate requestDelegate)
	{
		_requestDelegate = requestDelegate;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _requestDelegate(context);
		}

		catch (Exception ex)
		{
			await ProcessException(context, ex);
		}
	}

	private async Task ProcessException(HttpContext context, Exception ex)
	{
		context.Response.StatusCode = 500;
		context.Response.ContentType = "application/json";

		if (ex is BadImageFormatException badImageException)
		{
			await context.Response.WriteAsync(JsonConvert.SerializeObject(new
			{
			
				StatusCode =776,
				ErrorMessage = "Hello From Custom Middleware! Image Format is invalid"
			}));
		}

		else
		{
			await context.Response.WriteAsync(JsonConvert.SerializeObject(new
			{
				StatusCode = context.Response.StatusCode,
				ErrorMessage = "Hello From Middleware! - Finale"
				
			}));
		}

	}
}
