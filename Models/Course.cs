namespace _225_cie.Models
{
    // Model class to hold course-related data
    public class Course
    {
        // Unique identifier for the course
        public int CourseId { get; set; }

        // Name of the course
        public string CourseName { get; set; } = string.Empty;

        // Duration of the course (e.g., "8 Weeks")
        public string Duration { get; set; } = string.Empty;

        // Original registration fees
        public decimal Fees { get; set; }

        // Discount percentage applicable to the course
        public decimal DiscountPercentage { get; set; }
    }

    // Static class defining extension methods for Course
    public static class CourseExtensions
    {
        // Extension method to compute the final payable fee after deducting discount
        public static decimal CalculateFinalFees(this Course course)
        {
            // Calculate the discount amount
            decimal discount = course.Fees * (course.DiscountPercentage / 100m);

            // Subtract discount from the original fee
            return course.Fees - discount;
        }
    }
}