namespace _225_cie.Models
{
    // Class representing user data fetched from external API
    public class ExternalUser
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Company Company { get; set; } = new Company();
    }

    // Nested class representing user's company information
    public class Company
    {
        public string Name { get; set; } = string.Empty;
    }
}