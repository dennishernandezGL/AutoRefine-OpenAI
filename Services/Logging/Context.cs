namespace Services.Logging
{
    public class Context
    {
        public string ComponentName { get; set; }
        public string Environment { get; set; }
        public string InstanceIdentifier { get; set; }
        public string LoggerUser { get; set; }
        public string Timestamp { get; set; } = DateTime.UtcNow.ToString("o");
    }
}