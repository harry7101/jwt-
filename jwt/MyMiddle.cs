using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace jwt
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class IPMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        public IPMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<IPMiddleware>();
        }

      

        public async Task InvokeAsync(HttpContext context)
        {

            _logger.LogInformation($"Client Ip:{context.Connection.RemoteIpAddress.ToString()}");
            // Call the next delegate/middleware in the pipeline
            context.Request.EnableBuffering();//启用倒带功能，就可以让 Request.Body 可以再次读取
            string body = "";
            using (var reader = new StreamReader(
                    context.Request.Body,
                    encoding: Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: 1024 * 1024,
                    leaveOpen: true))
            {
                body = await reader.ReadToEndAsync();
            }
      
             context.Request.Body.Position = 0;// 重置读取位置


            body = Regex.Unescape(body); //处理返回的字符比如unicode转为中文
            if (!body.Contains("sa"))
            {
                await context.Response.WriteAsync("不是sa不能登入");
                return;
            }
          //  
            await _next(context);
        
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class IPMiddlewareExtensions
    {
        public static IApplicationBuilder UseIP(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IPMiddleware>();
        }
    }
}