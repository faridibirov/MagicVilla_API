using MagicVilla_Utility;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services;

public class TokenProvider : ITokenProvider
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public TokenProvider(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	public void ClearToken()
	{
		_httpContextAccessor.HttpContext?.Response.Cookies.Delete(SD.AccessToken);
	}

	public TokenDTO GetToken()
	{
		try
		{
			bool hasAccessToken = _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(SD.AccessToken, out string accessToken);

			TokenDTO tokenDTO = new()
			{
				AccessToken = accessToken
			};

			return hasAccessToken ? tokenDTO : null;

		}

		catch(Exception ex )
		{
			return null;
		}
	}

	public void SetToken(TokenDTO tokenDTO)
	{
		_httpContextAccessor.HttpContext?.Response.Cookies.Append(SD.AccessToken, tokenDTO.AccessToken);
	}
}
