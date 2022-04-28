using System;
using System.Data;
using http.dump.service.models;
using Microsoft.Extensions.Primitives;

using Microsoft.AspNetCore.Http.Extensions;
using http.dump.service.repositories;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace http.dump.service.middleware
{

    public class RequestDumpMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestDumpMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        private Dictionary<string, string[]> GetHeaders(HttpContext context)
        {
            var retval = new Dictionary<string, string[]>();
            foreach (var h in context.Request.Headers)
            {
                retval.Add(h.Key, h.Value.ToArray());
            }
            return retval;
        }
        private object? GetContent(HttpContext context)
        {
            var request = context.Request;
            request.Body.Seek(0, SeekOrigin.Begin);
            using (StreamReader stream = new StreamReader(request.Body))
            {
                string body = stream.ReadToEndAsync().Result;
                try
                {
                    if ((request.ContentType != null) && (request.ContentType.Contains("application/json")))
                    {
                        return JsonSerializer.Deserialize<object>(body);
                    }
                }
                catch
                {

                }
                return body;

            }
        }

        public DumpModel GetDump(HttpContext context)
        {
            return new DumpModel()
            {
                DateTime = DateTime.Now,

                Method = context.Request.Method,
                Url = context.Request.GetDisplayUrl(),
                Query = context.Request.QueryString.ToString(),
                Header = GetHeaders(context),
                Body = GetContent(context),
                ContentType = context.Request.ContentType,
                ContentLength = context.Request.ContentLength,
            };
        }
        public async Task InvokeAsync(HttpContext context, DumpRepository repo)
        {
            context.Request.EnableBuffering();
            if (
                context.Request.Path.ToString().Contains("$api") ||
                context.Request.Path.ToString().Contains("/swagger") ||
                context.Request.Path.ToString().Contains("/favicon.ico")
            )
            {
                await _next(context);
            }
            else
            {

                var id = repo.Create(GetDump(context));
                var validUntil = DateTime.Now.Add(new TimeSpan(0, DumpRepository.TTL_IN_MINUTES, 0));
                var url = context.Request.Scheme + "://" + context.Request.Headers.Host[0] + "/debug/$api/result/" + id;
                var result = new { validUntil, url };
                context.Response.ContentType = "application/json";
                //https://www.youtube.com/watch?v=ysgS4P4uHdo
                var json = JsonSerializer.Serialize(result);
                await context.Response.WriteAsync(json);
            }


        }
    }
}
