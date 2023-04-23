namespace DotnetAPI.Model
{
    public class UserPowePlantDataModel
    {
        public int UserId { get; set; }

        public double Capacity { get; set; }

        public double ShortCircuitVoltage { get; set; }
        public double InverterCapactity { get; set; }

        public string EmailAddress { get; set; } = "";


    }
}