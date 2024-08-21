
using MagicVilla_VillaAPI;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography.Xml;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>( option =>
{
	option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddResponseCaching();
builder.Services.AddScoped<IVillaRepository, VillaRepository>();
builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddApiVersioning(options=>
{
	options.AssumeDefaultVersionWhenUnspecified = true;
	options.DefaultApiVersion = new ApiVersion(1, 0);
	options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
	options.GroupNameFormat = "'v'VVV";
	options.SubstituteApiVersionInUrl = true;
});

var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");

builder.Services.AddAuthentication(x=>
{
	x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
	.AddJwtBearer(x=> {
		x.RequireHttpsMetadata = false;
		x.SaveToken = true;
		x.TokenValidationParameters = new TokenValidationParameters()
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
			ValidateIssuer = false,
			ValidateAudience = false,
			ClockSkew = TimeSpan.Zero
		};
	
	});

//Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
//	.WriteTo.File("log/villaLogs.txt", rollingInterval:RollingInterval.Day).CreateLogger();

//builder.Host.UseSerilog();

builder.Services.AddControllers(option =>
{
	//option.CacheProfiles.Add("Default30",
	//	new CacheProfile()
	//	{
	//		Duration = 30
	//	});
	//option.ReturnHttpNotAcceptable=true;
}).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options=>
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

	options.SwaggerDoc("v1", new OpenApiInfo
	{
		Version = "v1.0",
		Title = "Magic Villa V1",
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

	options.SwaggerDoc("v2", new OpenApiInfo
	{
		Version = "v2.0",
		Title = "Magic Villa",
		Description = "API to manage Villa V2",
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
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "Magic_VillaV2");
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Magic_VillaV1");
	});
}

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
