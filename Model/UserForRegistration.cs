namespace DotnetAPI.Model
{
    public partial class UserForRegistration
    {

        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        public string EmailAddress { get; set; } = "";

        public string Password { get; set; } = "";

        public string ConfirmPassword { get; set; } = "";
        
        public double Latitude { get; set; }
        
        public double Longitude { get; set; }
        
        public string PhoneNumber { get; set; } = "";

    }
}