namespace SingleSignOnUtility.DTOs
{
    public class FacebookUserProfile
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Locale { get; set; }
        public required string ProfilePictureUrl { get; set; }
    }
}
