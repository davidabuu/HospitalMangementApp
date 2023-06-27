namespace DotnetAPI.Model
{
    public partial class DoctorsRegistration
    {

        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        public string EmailAddress { get; set; } = "";

        public string Password { get; set; } = "";

        public string ConfirmPassword { get; set; } = "";

        public string Qualification { get; set; } = "";

        public string SpecialistIn { get; set; } = "";

        public string Gender { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public int Verified { get; set; }

    }
}