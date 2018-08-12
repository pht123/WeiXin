using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using log4net;
using log4net.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace WeiXin.Controllers
{
    [Route("")]
    public class ValuesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggerRepository _loggerRepository = Startup.loggerRepository;
        private ILog log;
        private DataBase.AccountContext _context;
        private IMemoryCache _cache;
        public ValuesController(IConfiguration configuration, DataBase.AccountContext context, IMemoryCache cache)
        {
            _configuration = configuration;
            log = LogManager.GetLogger(_loggerRepository.Name, typeof(ValuesController));
            _context = context;
            _cache = cache;
        }
        // GET api/values/5
        [HttpGet]
        public async Task<string> Get(string signature, string timestamp, string nonce, string echostr)
        {
            string token = _configuration["WeiXinToken"];
            //将token、timestamp、nonce三个参数进行字典序排序
            string[] arr = { token, timestamp, nonce };
            Array.Sort(arr);

            //将三个参数字符串拼接成一个字符串进行sha1加密
            string str = string.Join("", arr);

            return echostr;
        }

        [Produces("text/plain")]
        // POST api/values
        [HttpPost]
        public async Task<string> Post()
        {
            var reader = new StreamReader(Request.Body);
            string xml = await reader.ReadToEndAsync();
            log.Error($"用户发送的信息{xml}");
            Models.xml message = Common.CommonFunction.GetMessage(xml);

            var builder = new ConfigurationBuilder().AddJsonFile("JsonFile/AutoMessage.json", optional: false, reloadOnChange: true);
            var config = builder.Build();
            var replys = config.GetSection("AutoReply").Get<List<Models.AutoReply>>();
            if (replys != null && replys.Count > 0)
            {
                foreach (var reply in replys)
                {
                    if (reply.Type == Common.WeiXinEnum.MessageType.cp)
                    {
                        if (message.Content.StartsWith(reply.Key))
                        {
                            string telphone = message.Content.Substring(reply.Key.Length).Trim();
                            var cp = _context.Map.Where(t => t.ManTelphone == telphone).FirstOrDefault();
                            string cp_telphone = "";
                            if (cp == null)
                            {
                                cp = _context.Map.Where(t => t.WomenTelphone == telphone).FirstOrDefault();
                                if (cp == null)
                                {
                                    message.Content = "找不到您的匹配信息哦，小编正在努力帮您寻找心仪的对象...";
                                    return Common.CommonFunction.GetTextMesage(message);
                                }
                                cp_telphone = cp.ManTelphone;
                            }
                            else
                            {
                                cp_telphone = cp.WomenTelphone;
                            }                            
                            var cp_user = _context.UserInfo.Where(t => t.Telphone == cp_telphone).FirstOrDefault();
                            if (cp_user == null)
                            {
                                message.Content = "CP丢失了，小编正在努力匹配中...";
                                log.Error($"CP信息丢失；电话号码：{telphone}");
                                return Common.CommonFunction.GetTextMesage(message);                               
                            }
                            else
                            {
                                reply.Content = Common.CommonFunction.GetCp(cp_user, cp.ID);
                                return Common.CommonFunction.GetNewsMesage(message, reply);
                            }
                        }
                    }
                    if (message.Content == reply.Key)
                    {
                        switch (reply.Type)
                        {
                            case Common.WeiXinEnum.MessageType.text:
                                message.Content = reply.Content;
                                return Common.CommonFunction.GetTextMesage(message);
                            case Common.WeiXinEnum.MessageType.news:
                                reply.Content = reply.Content;
                                return Common.CommonFunction.GetNewsMesage(message, reply);
                            default:
                                message.Content = reply.Content;
                                return Common.CommonFunction.GetTextMesage(message);
                        }
                    }
                }
            }
            message.Content = _configuration["DefalutReply"];
            return Common.CommonFunction.GetTextMesage(message);
        }
    }
}
