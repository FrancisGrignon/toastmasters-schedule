using MembersV6.API.Services;

namespace MembersV6.API.Helpers
{
    public class ApiKeyValidatorsMiddleware
    {
        public const string API_KEY_HEADER_NAME = "x-api-key";

        private readonly RequestDelegate _next;

        private IApiKeysService Service { get; set; }

        public ApiKeyValidatorsMiddleware(RequestDelegate next, IApiKeysService service)
        {
            _next = next;

            Service = service;
        }

        public async Task Invoke(HttpContext context)
        {
            if (false == context.Request.Headers.Keys.Contains(API_KEY_HEADER_NAME))
            {
                context.Response.StatusCode = 400; // Bad Request                

                await context.Response.WriteAsync("User Key is missing");

                return;
            }
            else
            {
                var apiKey = context.Request.Headers[API_KEY_HEADER_NAME];

                if (false == Service.Validate(apiKey))
                {
                    context.Response.StatusCode = 401; // UnAuthorized

                    await context.Response.WriteAsync("Invalid User Key");

                    return;
                }
            }

            await _next.Invoke(context);
        }

    }
}
