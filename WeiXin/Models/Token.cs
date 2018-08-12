using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeiXin.Models
{
    public class Token
    {
        /// <summary>
        /// 接口返回的微信TOKEN
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// TOKEN有效时间
        /// </summary>
        public int expires_in { get; set; }
    }
}
