namespace DotnetAPI.Model
{
    public class UserPowePlantDataModel
    {
        public int UserId { get; set; }

        public decimal Capacity { get; set; }

        public decimal ShortCircuitVoltage { get; set; }
        public decimal InverterCapactity { get; set; }

        public string EmailAddress { get; set; } = "";


    }
}