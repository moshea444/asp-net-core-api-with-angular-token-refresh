namespace WebApi.Models
{
    public class RefreshTokenResultDto
    {
        public bool WasSuccessful { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
