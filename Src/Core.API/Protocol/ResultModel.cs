using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Protocol
{
    public class ResultModel
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="data">返回数据</param>
        /// <param name="message">辅助信息</param>
        /// <returns></returns>
        public static ResultModel Ok(object data, string message = "")
        {
            return new ResultModel { Data = data, Message = message, IsSuccess = true, Code = "200" };
        }

        /// <summary>
        /// 出错，前端可统一接管的错误（无需到指定模块中处理）
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="code">状态码</param>
        /// <returns></returns>
        public static ResultModel Error(string message, string code = "200")
        {
            return new ResultModel { Data = null, Message = message, IsSuccess = false, Code = code };
        }
    }
}
