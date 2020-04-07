using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.API.Auth;
using Core.API.Protocol;
using Core.API.SwaggerHelper;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using static Core.API.SwaggerHelper.CustomApiVersion;

namespace Core.API.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class AuthController : BaseController
    {
        //private readonly JwtOption options;
        private JwtOption Options
        { 
            get{
                var ti = HttpContext.GetMultiTenantContext()?.TenantInfo;
                var option = new JwtOption();
                if (ti.Items.TryGetValue("Audience", out object audience))
                {
                    option.Audience = audience.ToString();
                }
                if (ti.Items.TryGetValue("Secret", out object secret))
                {
                    option.Secret = (string)secret;
                }
                if (ti.Items.TryGetValue("Issuer", out object issuer))
                {
                    option.Issuer = (string)issuer;
                }
                if (ti.Items.TryGetValue("ExpirationMinutes", out object expirationMinutes))
                {
                    int expi = 2000;
                    int.TryParse(expirationMinutes.ToString(), out expi);
                    option.ExpirationMinutes = expi;
                }
                return option;
            }
        }
        public AuthController()
        {
 
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username"></param>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST
        ///     {
        ///        "id": 1,
        ///        "username": "Item1",
        ///        "isComplete": true
        ///     }
        ///
        /// </remarks>
        /// <returns></returns>
        //[HttpGet("/login")]
        [AllowAnonymous]
        [HttpGet]
        public ResultModel Login([FromQuery]string username)
        {

            //string[] roleType = username == "admin" ? "Administrator" : "Other";

            string[] roles = new string[] { "admin", "test" };

            //new Claim(ClaimTypes.Name, user.uLoginName),
            //        new Claim(JwtRegisteredClaimNames.Jti, tokenModel.Uid.ObjToString()),
            //        new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(_requirement.Expiration.TotalSeconds).ToString()) };

        var claims = new Claim[]
            {
                //new Claim(ClaimTypes.Role, String.Join(',', roles)),
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.Role, "test"),
                new Claim(ClaimTypes.NameIdentifier, "adminstrator"),
                new Claim(ClaimTypes.Name, "admin"),
            };
            var token = new JwtToken(Options).GenerateToken(claims);
            return ResultModel.Ok(token);
        }


    }
}