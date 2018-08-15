using log4net;
using log4net.Repository;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace WeiXin.Common
{
    public class CommonFunction
    {
        /// <summary>
        /// 获取微信TOKEN
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetTokenAsync(IConfiguration configuration, IMemoryCache cache)
        {
            string access_token;
            if (!cache.TryGetValue("WeiXinToken", out access_token))
            {
                string url = configuration["GetToken"];
                Models.Token token = await HttpHelper.GetAsync<Models.Token>(url, null);
                var log = LogManager.GetLogger(Startup.loggerRepository.Name, "GetToken");
                log.Error($"获取TOKEN：{Newtonsoft.Json.JsonConvert.SerializeObject(token)}");
                if (token != null && !string.IsNullOrEmpty(token.access_token) && token.access_token != "null")
                {
                    access_token = token.access_token;
                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(token.expires_in > 0 ? token.expires_in : 60));
                    cache.Set("WeiXinToken", token.access_token, cacheEntryOptions);
                }
            }
            return access_token;
        }

        /// <summary>
        /// 将微信接收到的消息处理成类
        /// </summary>
        /// <param name="xml"></param>
        public static Models.xml GetMessage(string xml)
        {
            Models.xml message = new Models.xml();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNode xnMsgType = doc.SelectSingleNode($"/xml/MsgType");
            if(xnMsgType.InnerText != "text")
            {
                return null;
            }

            Type type = typeof(Models.xml);
            var propertries = type.GetProperties();
            foreach (var pi in propertries)//遍历当前类的所有公共属性
            {
                XmlNode xnChiefComplaint = doc.SelectSingleNode($"/xml/{pi.Name}");
                if(xnChiefComplaint != null)
                {
                    pi.SetValue(message, xnChiefComplaint.InnerText);
                }                
            }
            return message;
        }

        public static async Task CreateMenu(IConfiguration configuration, IMemoryCache cache)
        {
            var log = LogManager.GetLogger(Startup.loggerRepository.Name, "GetToken");

            var builder = new ConfigurationBuilder().AddJsonFile("JsonFile/WeiXinMenu.json", optional: false, reloadOnChange: true);
            var config = builder.Build();
            var menus = config.GetSection("button").Get<List<Models.Menu>>();

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(new { button = menus });
            string acc_token = await GetTokenAsync(configuration, cache);
            log.Error($"菜单TOKEN：{acc_token}");
            if (string.IsNullOrEmpty(acc_token))
            {
                acc_token = await GetTokenAsync(configuration, cache);
            }

            string url = configuration["WeiXinMenu"];
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("access_token", acc_token);
            var menujson = await HttpHelper.PostAsync(url, json, dictionary);

            log.Error($"菜单：{menujson}");
        }

        /// <summary>
        /// 回复文本信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string GetTextMesage(Models.xml message)
        {
            return $@"<xml>
<ToUserName><![CDATA[{message.FromUserName}]]></ToUserName>
<FromUserName><![CDATA[{message.ToUserName}]]></FromUserName>
<CreateTime>{message.CreateTime}</CreateTime>
<MsgType><![CDATA[text]]></MsgType>
<Content><![CDATA[{message.Content}]]></Content>
</xml>";
        }

        /// <summary>
        /// 回复图文信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string GetNewsMesage(Models.xml message, Models.AutoReply reply)
        {
            return $@"<xml>
<ToUserName><![CDATA[{message.FromUserName}]]></ToUserName>
<FromUserName><![CDATA[{message.ToUserName}]]></FromUserName>
<CreateTime>{message.CreateTime}</CreateTime>
<MsgType><![CDATA[news]]></MsgType>
<ArticleCount>1</ArticleCount>
<Articles>
    <item>
        <Title><![CDATA[{reply.Title}]]></Title> 
        <Description><![CDATA[{reply.Content}]]></Description>
        <PicUrl><![CDATA[{reply.ImgUrl}]]></PicUrl>
        <Url><![CDATA[{reply.Link}]]></Url>
    </item>
</Articles>
</xml>";
        }

        public static string GetCp(Entity.UserInfo cp, int id)
        {
            return $@"匹配结果：您的cp资料如下
姓名：{cp.Name}
QQ号：{cp.QQ}
微信号：{cp.WX}
自我介绍：{cp.Introduce}

你们的共同编号：{id.ToString("000")}
进群后请把备注改为共同编号

如有任何问题请联系负责人微信：selena1206";
        }
    }
}
