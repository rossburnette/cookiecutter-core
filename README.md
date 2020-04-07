
README
===========================
基于 netcore3.x Finbuckle.MultiTenant 多组户后端API项目脚手架，

## 模板目录结构
* API 项目
    * Auth 鉴权、授权
        * JwtToken.cs  用于生成JWT Token
        * PermissionHandler.cs 权限验证处理类
        * PermissionRequirement.cs 权限依据实体类
    * AuthStore 授权需要用的数据存储
        * ApiStore.cs Api与角色的关系数据存储，示例数据，
        * UserStore.cs 登录用户与角色的关系数据存储，示例数据，
    * Controllers 控制器，由于引入了Swagger做为借口文档，加入多版本支持，建议在对应版本文件夹下建立控制器
        * v1 
        * v2
        * AuthController.cs 用户认证相关接口
        * BaseController.cs 控制器基础类库
        * DemoController.cs 后端写法的一些示例，产品环境建议删除
    * Filters 用于编写自定义的过滤器文件夹
        * ValidateModelFilter.cs 全局模型验证过滤器   
    * Middleware 用于编写自定义的中间件文件夹
         * ErrorHandlingMiddleware.cs 全局错误接管中间件
    * Protocol 规约文件夹，主要用于前端后端的数据传输协议
         * ResultModel.cs  
    * SwaggerHelper
         * CustomApiVersion.cs 定义接口版本枚举
         * CustomRouteAttribute.cs 自定义路由特性类，用于action特性

