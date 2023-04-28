namespace DotnetAPI.Model
{
    public partial class UserForRegistrationInfo
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        public string EmailAddress { get; set; } = "";

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string PhoneNumber { get; set; } = "";
        public bool IsVerified { get; set; }
        public bool UserMonitoringDevice { get; set; }
        public bool UserPowerPlantData { get; set; }

    }
}