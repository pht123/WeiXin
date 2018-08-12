using log4net;
using log4net.Repository;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeiXin.Common
{

    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private ILoggerRepository _loggerRepository = Startup.loggerRepository;
        public void OnException(ExceptionContext context)
        {
            var log = LogManager.GetLogger(_loggerRepository.Name, context.Exception.TargetSite.ReflectedType);
            log.Error(context.Exception.ToString());
        }
    }

}
