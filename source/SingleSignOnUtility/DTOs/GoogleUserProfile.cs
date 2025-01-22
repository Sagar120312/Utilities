namespace SingleSignOnUtility.DTOs
{
    public class UserProfile
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Picture { get; set; }
        public required string Locale { get; set; }
    }
}
