using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace jwt.Controllers
{
    /// <summary>
    /// 这个叫你懂的基类
    /// </summary>
    public class BaseController : ControllerBase
    {

        /// <summary>
        /// 用户id
        /// 理论上，只有通过验证的Controller方法（带[Authorize]标签）
        /// 才能调用这个方法获得用户id，否则都只返回0
        /// </summary>
        public string UserId
        {
            get
            {
                // 这里从请求的头里获得 jwt 的token信息
                var headerCode = Request.Headers["Authorization"];
                if (String.IsNullOrEmpty(headerCode))
                {
                    
                    return "";
                }
                var code = headerCode.ToString().Replace("Bearer ", "");

                // 这里从 jwt token 里解析出用户id，默认认为已经通过权限验证了
                var read = new JwtSecurityTokenHandler().ReadJwtToken(code);
                var claim = read.Claims.FirstOrDefault(o => o.Type == ClaimTypes.Name);
             

                return claim.Value;
            }
        }
    }
}