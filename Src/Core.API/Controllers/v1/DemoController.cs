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

namespace Core.API.Controllers.v1
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
        /// 后台写法示例-取得租户
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomRoute(ApiVersions.v1)]
        public ResultModel Tenant() {

            var ti = HttpContext.GetMultiTenantContext()?.TenantInfo;
            return ResultModel.Ok(ti, "取得当前租户实体示例");
        }

        /// <summary>
        /// 后台写法示例-错误拦截示例，前端接收的是统一的JSON格式
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomRoute(ApiVersions.v1)]
        public ResultModel ErrorHandler()
        {
            throw new Exception("我是异常消息");
        }

        /// <summary>
        /// 后台写法示例-匿名方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [CustomRoute(ApiVersions.v1)]
        public ResultModel AllowAnonymous()
        {
            return ResultModel.Ok(null, "访问我不需要登录，更不需要权限");
        }
    }
}