using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class HeaderHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HeaderHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        var userId = _httpContextAccessor.HttpContext?.Session.GetString("Id");

        if (
            !string.IsNullOrEmpty(userId)
            && request.RequestUri?.Host == "localhost"
            && request.RequestUri?.Port == 44365
        )
        {
            request.Headers.Add("X-User-Id", userId);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
