namespace Obrazovashka.AuthService.Results
{
    public class LoginResult
    {
        public bool? Success { get; set; }
        public string? Token { get; set; }
        public string? Message { get; set; }
    }
}
