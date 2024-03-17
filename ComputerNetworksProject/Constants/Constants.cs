namespace ComputerNetworksProject.Constants
{
    public enum Roles
    {
        Admin,
        User
    }
    public class Constant
    {
        public static readonly int PageSizeCards = 12;

        public static readonly int PageSizeTable = 24;

        public static readonly int CookieOffset = 60; // in minutes

        public static readonly int ClearCartDelay = 3600000; //one hour in milis

        public static readonly int SentNotificationDelay = 3600000; //one hour in milis

        public static readonly string WebUrl = "https://localhost:7047/";
    }
}
