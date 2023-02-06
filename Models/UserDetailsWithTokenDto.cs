namespace test_crud.Models
{
    public class UserDetailsWithTokenDto: UserDetailsDto
    {
        public string AccessToken { get; set; } = string.Empty;
    }
}
