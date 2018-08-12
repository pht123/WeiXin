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
        private readonly IConfiguration _configuration;
        private IMemoryCache _cache;
        public CommonFunction(IConfiguration configuration, IMemoryCache cache)
        {
            _configuration = configuration;
            _cache = cache;
        }
        /// <summary>
        /// 获取微信TOKEN
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetTokenAsync()
        {
            string access_token;
            if (!_cache.TryGetValue("WeiXinToken", out access_token))
            {
                string url = _configuration["GetToken"];
                Models.Token token = await HttpHelper.GetAsync<Models.Token>(url, null);
                if (token != null)
                {
                    access_token = token.access_token;
                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(token.expires_in - 600));
                    _cache.Set("WeiXinToken", token.access_token, cacheEntryOptions);
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

            Type type = typeof(Models.xml);
            var propertries  = type.GetProperties();
            foreach (var pi in propertries)//遍历当前类的所有公共属性
            {
                XmlNode xnChiefComplaint = doc.SelectSingleNode($"/xml/{pi.Name}");
                pi.SetValue(message, xnChiefComplaint.InnerText);
            }
            return message;
        }
    }
}
