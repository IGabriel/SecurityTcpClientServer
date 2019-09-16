using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;

namespace Common
{
    public class LoggerFactory
    {
        private const string Log4netConfigFile = "log4net.config";

        public static void Initialize(string log4netConfigFile = Log4netConfigFile)
        {
            var repository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(repository, new FileInfo(log4netConfigFile));
        }

        public static ILog GetLogger(Type classType)
        {
            return LogManager.GetLogger(classType);
        }
    }
}