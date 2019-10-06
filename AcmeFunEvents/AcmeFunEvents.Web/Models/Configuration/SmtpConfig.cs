namespace AcmeFunEvents.Web.Models.Configuration
{
    public class SmtpConfig
    {
        public string Server { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public int Port { get; set; }
        public string ReplyAddress { get; set; }
    }
}
