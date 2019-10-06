namespace AcmeFunEvents.Web.Models
{
    public class ApiResponse 
    {
        public ApiResponse()
        {
            HttpErrorMessage = "";
            HttpFriendlyErrorMessage = "";
        }

        public object Object { get; set; }

        public int HttpStatus { get; set; }
        
        public string HttpErrorMessage { get; set; }

        public string HttpFriendlyErrorMessage { get; set; }
    }
}