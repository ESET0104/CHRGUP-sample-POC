namespace esyasoft.mobility.CHRGUP.service.api.DTOs.Auth
{
    public class LoginRequestDto
    {
        public string Identifier { get; set; } = null!; // email or username
        public string Password { get; set; } = null!;
    }
}
