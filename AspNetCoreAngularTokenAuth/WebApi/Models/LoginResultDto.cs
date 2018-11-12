namespace WebApi.Models
{
    public class LoginResultDto
    {
        public bool WasSuccessful { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
