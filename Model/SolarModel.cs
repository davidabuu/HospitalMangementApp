namespace DotnetAPI.Model
{
    public class SolarModel
    {
        public int UserId {get; set;}
        public double GetCurrent { get; set; }

        public DateTime GetDate { get; set; }
        public double Voltage { get; set; }
        public double Radiance { get; set; }
        public int GetStatus { get; set; }
    }
}