using Core.API.Protocol;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Core.API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }

        }


        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            //var code = HttpStatusCode.InternalServerError; // 500 if unexpected
            var code = StatusCodes.Status500InternalServerError;

            string info;
            switch (context.Response.StatusCode)
            {
                case 401:
                    info = "没有权限";
                    break;
                case 404:
                    info = "未找到服务";
                    break;
                case 403:
                    info = "服务器理解请求客户端的请求，但是拒绝执行此请求";
                    break;
                case 500:
                    info = "服务器内部错误，无法完成请求";
                    break;
                case 502:
                    info = "请求错误";
                    break;
                default:
                    info = ex.Message;
                    break;
            }

            // todo:可记录日志,如通过注入Nlog等三方日志组件记录

            //var result = JsonConvert.SerializeObject(new { Coede= code.ToString(), Message = info });
            var result = JsonConvert.SerializeObject(ResultModel.Error(info, code.ToString()));
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;            
            return context.Response.WriteAsync(result);
        }
    }
}
