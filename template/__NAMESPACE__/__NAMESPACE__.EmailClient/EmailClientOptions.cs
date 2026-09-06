namespace __NAMESPACE__.EmailClient
{
    public class EmailClientOptions
    {
        public string? SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string? SmtpFrom { get; set; }
        public string? SmtpFromDisplayName { get; set; }
        public string? SmtpMail { get; set; }
        public string? SmtpPassword { get; set; }
    }
}
