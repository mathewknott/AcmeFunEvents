namespace AcmeFunEvents.Web.Models.Configuration
{
    /// <summary>
    /// Use this to access the options section of the appsettings json file
    /// </summary>
    public class AppOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public string SiteTitle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Robots { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DebugEmail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Tracking Tracking { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public EfSettings EfSettings { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SmtpConfig Smtp { get; set; }
    }    
}