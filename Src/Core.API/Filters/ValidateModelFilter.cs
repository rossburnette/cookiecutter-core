using Core.API.Protocol;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Filters
{
    /// <summary>
    /// 模型验证错误拦截
    /// </summary>
    public class ValidateModelFilter : ActionFilterAttribute
    {
        /// <summary>
        /// Action 调用前执行
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                //获取验证失败的模型字段 
                var errors = context.ModelState
                    .Where(e1 => e1.Value.Errors.Count > 0)
                    .Select(e1 => e1.Value.Errors.First().ErrorMessage)
                    .ToList();
                var str = string.Join("|", errors);
                context.Result = new JsonResult(ResultModel.Error(str, StatusCodes.Status400BadRequest.ToString()));
            }
        }

        /// <summary>
        /// Action 方法调用后，Result 方法调用前执行
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }

}
