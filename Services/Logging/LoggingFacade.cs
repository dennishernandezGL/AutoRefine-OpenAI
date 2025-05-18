using Services.Logging.MixPanel;

namespace Services.Logging
{
    public class LoggingFacade
    {
        private static LoggingFacade _instance = new LoggingFacade();
        private readonly IConfiguration _configuration;

        protected LoggingFacade() { }
        public LoggingFacade(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static LoggingFacade GetInstance(LoggingConnections connectionType, IConfiguration configuration)
        {
            switch (connectionType)
            {
                case LoggingConnections.MixPanel:
                    _instance = new MixPanelConnectivity(configuration); 
                    return _instance;
                default:
                    throw new NotImplementedException($"Logging connection type '{connectionType}' is not implemented.");
            }
        }

        public virtual void LogInfo(string message, object obj, Context context)
        {
            throw new NotImplementedException("LogInfo method must be implemented by a derived class.");
        }

        public virtual void LogWarning(string message, object obj, Context context)
        {
            throw new NotImplementedException("LogWarning method must be implemented by a derived class.");
        }

        public virtual void LogError(ExceptionDetails exception, object obj, Context context)
        {
            throw new NotImplementedException("LogError method must be implemented by a derived class.");
        }

        public virtual Task<IEnumerable<dynamic>> RetrieveLogsAsync(DateTime startDate, DateTime endDate, string eventName)
        {
            throw new NotImplementedException("RetrieveLogsAsync method must be implemented by a derived class.");
        }

        public virtual Task<IEnumerable<dynamic>> RetrieveLogsByContextAsync(Func<Context, bool> contextComparison, DateTime startDate, DateTime endDate, string eventName)
        {
            throw new NotImplementedException("RetrieveLogsByContextAsync method must be implemented by a derived class.");
        }
    }
}