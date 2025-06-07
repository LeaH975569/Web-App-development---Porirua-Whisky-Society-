namespace PWS.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string ErrorMessage = "Something went wrong!";

        public int? StatusCode;
    }
}
