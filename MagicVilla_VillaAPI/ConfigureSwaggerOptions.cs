using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MagicVilla_VillaAPI;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
	private readonly IApiVersionDescriptionProvider provider;

	public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
	{
		this.provider = provider;
	}

	public void Configure(SwaggerGenOptions options)
	{
		options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
		{
			Description =
		"JWT Authorization header using the Bearer scheme. \r\n\r\n " +
		"Enter 'Bearer' [space] and then your in the text input below. \r\n\r\n" +
		"Example: \"Bearer 12345abcdef\"",
			Name = "Authorization",
			In = ParameterLocation.Header,
			Scheme = "Bearer"
		});
		options.AddSecurityRequirement(new OpenApiSecurityRequirement()
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				},
				Scheme = "oauth2",
				Name = "Bearer",
				In= ParameterLocation.Header
			},
			new List<string>()
		}
	});

		foreach(var desc in provider.ApiVersionDescriptions)
		{
			options.SwaggerDoc(desc.GroupName, new OpenApiInfo
			{
				Version = desc.ApiVersion.ToString(),
				Title = $"Magic Villa {desc.ApiVersion}",
				Description = "API to manage Villa",
				TermsOfService = new Uri("https://example.com/terms"),
				Contact = new OpenApiContact
				{
					Name = "Farid",
					Url = new Uri("https://github.com/faridibirov")
				},
				License = new OpenApiLicense
				{
					Name = "Example of License",
					Url = new Uri("https://example.com/license")
				},

			});

		}
	}
}
