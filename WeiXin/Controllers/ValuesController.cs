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

namespace WeiXin.Controllers
{
    [Route("")]
    public class ValuesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggerRepository _loggerRepository = Startup.loggerRepository;
        private ILog log;
        public ValuesController(IConfiguration configuration)
        {
            _configuration = configuration;
            log = LogManager.GetLogger(_loggerRepository.Name, typeof(ValuesController));
        }
        // GET api/values/5
        [HttpGet]
        public string Get(string signature, string timestamp, string nonce, string echostr)
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
            log.Error(xml);

            Models.xml message = Common.CommonFunction.GetMessage(xml);

            log.Error(Newtonsoft.Json.JsonConvert.SerializeObject(message));

            message.Content = $"http://www.baidu.com/";

            return $"<xml><ToUserName><![CDATA[{message.FromUserName}]]></ToUserName><FromUserName><![CDATA[{message.ToUserName}]]></FromUserName><CreateTime>{message.CreateTime}</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[{message.Content}]]></Content></xml>";
        }
    }
}
