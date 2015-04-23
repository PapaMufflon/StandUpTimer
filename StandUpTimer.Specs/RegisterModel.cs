namespace StandUpTimer.Specs
{
    public class RegisterModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public RegisterModel(string email, string password, string confirmPassword)
        {
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
        }

        public RegisterModel() { }
    }
}