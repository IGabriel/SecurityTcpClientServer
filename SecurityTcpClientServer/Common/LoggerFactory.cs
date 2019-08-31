using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;

namespace Common
{
    public static class LoggerFactory
    {
        private const string Log4netConfigFile = "log4net.config";

        public static void Initialize(string log4netConfigFile = Log4netConfigFile)
        {
            var assembly = Assembly.GetEntryAssembly();
            string configPath = Path.Combine(Path.GetDirectoryName(assembly.Location), log4netConfigFile);

            var repository = LogManager.GetRepository(assembly);
            XmlConfigurator.Configure(repository, new FileInfo(configPath));
        }

        public static ILog GetLogger(Type classType)
        {
            return LogManager.GetLogger(classType);
        }
    }
}