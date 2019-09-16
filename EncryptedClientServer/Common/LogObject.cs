using log4net;

namespace Common
{
    public class LogObject
    {
        private ILog _logger;

        protected ILog Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = LoggerFactory.GetLogger(GetType());
                }
                return _logger;
            }
        }
    }
}
