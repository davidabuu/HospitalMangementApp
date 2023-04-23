namespace DotnetAPI.Model
{
    public class UserMonitoringDataModel
    {

        public int UserId { get; set; }

        public string MacAddress { get; set; } = "";


        public string IpAddress { get; set; } = "";
        public int Port { get; set; }

        public string EmailAddress { get; set; } = "";


    }
}