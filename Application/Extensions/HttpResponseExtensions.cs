using Microsoft.AspNetCore.Http;

namespace Application.Extensions
{
    public static class HttpResponseExtensions
    {
        public static void ClearBody(this HttpResponse response)
        {
            MemoryStream newResponse = new MemoryStream();
            response.Body = newResponse;
        }

        public static async Task<string> CopyAndReplaceBody(this HttpResponse response, Stream newBody)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var bodyContent = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            await response.Body.CopyToAsync(newBody);
            response.Body = newBody;

            return bodyContent;
        }
    }
}
