using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using System.Net.Http;

namespace MagicVilla_Web.Services;

public class AuthService : IAuthService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private string villaUrl;
	private readonly IBaseService _baseService;

	public AuthService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IBaseService baseService)
    {
        _baseService = baseService;
        _httpClientFactory = httpClientFactory;
        villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
    }

    public async Task<T> LoginAsync<T>(LoginRequestDTO obj)
    {
        return await _baseService.SendAsync<T>(new APIRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = obj,
            Url = villaUrl + "/api/v1/UsersAuth/login"
		}, withBearer:false);
    }

    public async Task<T> RegisterAsync<T>(RegisterationRequestDTO obj)
    {
        return await _baseService.SendAsync<T>(new APIRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = obj,
            Url = villaUrl + "/api/v1/UsersAuth/register"
		}, withBearer:false);
    }
}
