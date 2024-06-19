namespace ComputerNetworksProject.Models
{
    public class ErrorViewModel
    {
        //view model for 404
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}