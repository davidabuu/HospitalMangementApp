namespace DotnetAPI.Model
{
//UserLogin
    public partial class UserForLoginInfo
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

    }
}