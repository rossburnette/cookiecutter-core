using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.API.Protocol;
using Core.API.SwaggerHelper;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Core.API.SwaggerHelper.CustomApiVersion;

namespace Core.API.Controllers.v2
{
    //[Route("api/[controller]")]
    //[ApiController]
    /// <summary>
    /// 用于示例的控制器，正式使用可删除
    /// </summary>
    [Authorize("default")]
    public class DemoController : BaseController
    {
      
        /// <summary>
        /// 后台写法示例-匿名方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [CustomRoute(ApiVersions.v2)]
        public ResultModel AllowAnonymous()
        {
            return ResultModel.Ok(null, "访问我不需要登录，更不需要权限");
        }
    }
}