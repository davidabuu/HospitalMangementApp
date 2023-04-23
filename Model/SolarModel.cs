namespace DotnetAPI.Model
{
    public class SolarModel
    {
        public int UserId {get; set;}
        public double Current { get; set; }

        public DateTime GetDate { get; set; }
        public double Voltage { get; set; }
        public double Radiance { get; set; }
        public int Status { get; set; }
        public string EmailAddress {get; set;} = "";
    }
}