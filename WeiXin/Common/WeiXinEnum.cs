using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeiXin.Common
{
    public class WeiXinEnum
    {
        public enum MessageType
        {
            /// <summary>
            /// 文本信息
            /// </summary>
            text = 1,
            /// <summary>
            /// 图片信息
            /// </summary>
            image = 2,
            /// <summary>
            /// 语音信息
            /// </summary>
            voice = 3,
            /// <summary>
            /// 视频信息
            /// </summary>
            video = 4,
            /// <summary>
            /// 音乐信息
            /// </summary>
            music = 5,
            /// <summary>
            /// 图文信息
            /// </summary>
            news = 6,
            /// <summary>
            /// cp信息
            /// </summary>
            cp = 7
        }
    }
}
