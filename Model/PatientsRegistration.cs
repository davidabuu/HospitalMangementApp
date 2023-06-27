namespace DotnetAPI.Model
{
    public partial class PatientsRegistration
    {

        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        public string EmailAddress { get; set; } = "";

        public string Password { get; set; } = "";

        public string ConfirmPassword { get; set; } = "";

        public string PatientsAddress { get; set; } = "";

        public string PatientsState { get; set; } = "";

        public string Gender { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public int Age { get; set; }

    }
}