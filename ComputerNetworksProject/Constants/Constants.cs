namespace ComputerNetworksProject.Constants
{
    public enum Roles
    {
        Admin,
        User
    }
    public class Constant
    {
        public static readonly int PageSizeCards = 6;
        public static readonly int PageSizeTable = 6;
        public static DateTimeOffset CookieOffset { get => DateTime.Now.AddDays(1); }
        public static DateTimeOffset CartMinLastUpdate { get => DateTime.Now.AddDays(1); }

        public static readonly int ClearCartDelay = 120000;

        public static readonly int SentNotificationDelay = 120000;

        public static readonly string WebUrl = "https://localhost:7047/";
    }
}
