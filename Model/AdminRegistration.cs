namespace DotnetAPI.Model
{
    public partial class AdminRegistration
    {

        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";

        public string ConfirmPassword { get; set; } = "";

        public string AdminRole { get; set; } = "";


    }
}