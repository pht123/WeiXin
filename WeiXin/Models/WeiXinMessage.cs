using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeiXin.Models
{
    /// <summary>
    /// 微信消息
    /// </summary>
    public class xml
    {
        /// <summary>
        /// 开发者微信号
        /// </summary>
        public string ToUserName { get; set; }
        /// <summary>
        /// 发送方帐号（一个OpenID）
        /// </summary>
        public string FromUserName { get; set; }
        /// <summary>
        /// 消息创建时间 （整型）
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// text
        /// </summary>
        public string MsgType { get; set; }
        /// <summary>
        /// 文本消息内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 消息id，64位整型
        /// </summary>
        public string MsgId { get; set; }
    }

    public class XmlMessage
    {
        public xml xml { get; set; }
    }
}
