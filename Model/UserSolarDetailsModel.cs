namespace DotnetAPI.Model
{
    public class UserSolarModel
    {
        public int UserId { get; set; }
        public double Current { get; set; }

        public DateTime GetDate { get; set; }
        public double Voltage { get; set; }
        public double Radiance { get; set; }
        public int Status { get; set; }
        public string EmailAddress { get; set; } = "";

        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";


        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string PhoneNumber { get; set; } = "";
        public bool IsVerified { get; set; }
    }
}