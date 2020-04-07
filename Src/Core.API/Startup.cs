
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.API.Auth;
using Core.API.AuthStore;
using Core.API.Filters;
using Core.API.Middleware;
using Core.API.Protocol;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using static Core.API.SwaggerHelper.CustomApiVersion;
using Swashbuckle.AspNetCore.Swagger;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private const string ApiName = "MutiTenant Foundation ";

        // This method gets called by the runtime. Use this method to add services to the container.
        // 这个方法被运行时调用。使用此方法将服务添加到容器中。
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers(optinos =>
                {
                    // 全局模型验证
                    optinos.Filters.Add<ValidateModelFilter>();
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    //关闭默认模型验证,因为我们使用了自己的ValidateModelFilter
                    options.SuppressModelStateInvalidFilter = true;
                    // 如果不使用自己的ValidateModelFilter，也可以用下面的代码实现模型验证
                    //options.InvalidModelStateResponseFactory = actionContext =>
                    //{
                    //    //获取验证失败的模型字段 
                    //    var errors = actionContext.ModelState
                    //        .Where(e1 => e1.Value.Errors.Count > 0)
                    //        .Select(e1 => e1.Value.Errors.First().ErrorMessage)
                    //        .ToList();
                    //    var str = string.Join("|", errors);
                    //    return new JsonResult(ResultModel.Error(str, StatusCodes.Status400BadRequest.ToString()));
                    //};
                })
                .AddNewtonsoftJson(options =>
                {
                    // 忽略循环引用
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    // 使用小驼峰
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); // new DefaultContractResolver();
                    // 设置时间格式
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                });

            #region 全局认证参数配置
            services
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.SlidingExpiration = true;
                })
                .AddJwtBearer(o =>
                {
                    // 每个租户可以有不同的配置参数，故在租户里面配置参数 (JwtBearerOptions:o)属性，
                    // 见WithPerTenantOptions<JwtBearerOptions>
                });
            #endregion

            #region 包括鉴权、授权等在内的租户配置
            services.AddMultiTenant()
                .WithConfigurationStore()
                .WithRouteStrategy()
                .WithPerTenantOptions<JwtBearerOptions>((o, tenantInfo) =>
                {

                    o.RequireHttpsMetadata = false;
                    //使用应用密钥得到一个加密密钥字节数组
                    o.Challenge= JwtBearerDefaults.AuthenticationScheme;
                    var key = Encoding.ASCII.GetBytes((string)tenantInfo.Items["Secret"]);
                    //o.Authority = (string)tenantInfo.Items["Authority"];
                    o.Audience = (string)tenantInfo.Items["Audience"];
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,//是否验证超时  当设置exp和nbf时有效 同时启用ClockSkew 
                        //这是缓冲过期时间，总的有效时间等于这个时间加上jwt的过期时间，如果不配置，默认是5分钟
                        ClockSkew = System.TimeSpan.FromSeconds(30)
                    };
                });


            //定义基于角色的授权依据列表，netcore内置授权依据见Microsoft.AspNetCore.Authorization.Infrastructure命名空间下类，
            //为了扩展此处用自定义的PermissionRequirement类
            var permissionRequirement = new PermissionRequirement();
            services.AddSingleton(permissionRequirement);
            services.AddSingleton<UserStore>(); // 注册用户与角色关系数据类，PermissionHandler需要用到
            services.AddSingleton<ApiStore>();  // 注册API与角色关系数据类，PermissionHandler需要用到
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

            // 注册权限验证策略
            services.AddAuthorization(options =>
            {
                options.AddPolicy("default",
                         policy => policy.Requirements.Add(permissionRequirement));
            });
            #endregion

            #region Swagger 接口文档定义
            // 注册Swagger生成器，定义一个或多个Swagger文档
            services.AddSwaggerGen(c =>
            {
                //c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                //根据版本名称倒序 遍历展示
                typeof(ApiVersions).GetEnumNames().OrderBy(e => e).ToList().ForEach(version =>
                {
                    c.SwaggerDoc(version, new OpenApiInfo
                    {
                        Version = version,
                        Title = $"{ApiName} API",
                        Description = "base on .netcore 3.1 ,多租户+全局错误接管+全局模型验证+统一返回格式",
                        TermsOfService = new Uri("https://www.xxx.com/"),//服务条款
                        Contact = new OpenApiContact
                        {
                            Name = "Alonso",
                            Email = string.Empty
                            //Url = new Uri("https://www.xxx.com/alonso"),
                        },
                        License = new OpenApiLicense
                        {
                            Name = "License",
                            Url = new Uri("https://www.xxx.com/license"),
                        }
                    });

                });




                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    //添加注释到SwaggerUI
                    c.IncludeXmlComments(xmlPath);
                }

                #region 为SwaggerUI添加全局token验证
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "在下框中输入请求头中需要添加Jwt授权Token：Bearer Token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme{ Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme,Id = "Bearer" }},
                        new string[] { }
                    }});
                #endregion
            });
            #endregion


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // 这个方法被运行时调用。使用此方法配置HTTP请求管道。
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //启用swagger中间件
            app.UseSwagger();

            //启用中间件服务swagger-ui (HTML, JS, CSS等)，
            //指定Swagger JSON端点。
            app.UseSwaggerUI(c =>
            {
                //c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                //根据版本名称倒序 遍历展示
                typeof(ApiVersions).GetEnumNames().OrderBy(e => e).ToList().ForEach(version =>
                {
                    c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{ApiName} {version}");
                });

            });



            // 全局异常捕获
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            

            // 从当前请求拿到终结点
            app.UseRouting();
            // 启用多租户
            app.UseMultiTenant();
            // 启用Authentication中间件，遍历策略中的身份验证方案获取多张证件，最后合并放入HttpContext.User中
            app.UseAuthentication();
            // 对请求进行权限验证
            app.UseAuthorization();
            // 路由匹配设置
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapControllerRoute("default", "{__tenant__=tenant1}/api/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
