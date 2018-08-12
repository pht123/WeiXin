using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeiXin.Models
{
    public class AutoReply
    {
        public string Key { get; set; }
        public string Content { get; set; }
        public string ImgUrl { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public Common.WeiXinEnum.MessageType Type { get; set; }
    }
}
