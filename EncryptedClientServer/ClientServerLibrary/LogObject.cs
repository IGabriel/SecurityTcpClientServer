namespace ClientServerLibrary
{
    public class LogObject
    {
        private ILogger _logger;

        protected ILogger Logger
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