using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using _225_cie.Models;
using System.Text.Json;

namespace _225_cie.Controllers
{
    public class CourseController : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;
        private const string CourseCacheKey = "CourseListCache";

        // Constructor injecting memory cache and HTTP client factory
        public CourseController(IMemoryCache cache, IHttpClientFactory httpClientFactory)
        {
            _cache = cache;
            _httpClientFactory = httpClientFactory;
        }

        // Helper method returning sample course catalog
        private List<Course> GetSampleCourses()
        {
            return new List<Course>
            {
                new Course { CourseId = 101, CourseName = "Full Stack ASP.NET Core", Duration = "8 Weeks", Fees = 5000, DiscountPercentage = 10 },
                new Course { CourseId = 102, CourseName = "Data Science with Python", Duration = "10 Weeks", Fees = 6500, DiscountPercentage = 15 },
                new Course { CourseId = 103, CourseName = "Cloud & DevOps Essentials", Duration = "6 Weeks", Fees = 4000, DiscountPercentage = 20 }
            };
        }

        // 1 & 4. Displays courses using IMemoryCache with a 5-minute expiration
        public IActionResult Index()
        {
            // Attempt to retrieve cached course list
            if (!_cache.TryGetValue(CourseCacheKey, out List<Course>? courses))
            {
                // Cache miss: fetch courses
                courses = GetSampleCourses();

                // Define 5-minute absolute cache expiration
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                // Save data in cache
                _cache.Set(CourseCacheKey, courses, cacheOptions);
            }

            return View(courses);
        }

        // 1 & 4. Course Details page: uses Query String for CourseId and Response Caching
        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        public IActionResult Details(int courseId) // Receives courseId via query string (?courseId=...)
        {
            var courses = GetSampleCourses();
            var course = courses.FirstOrDefault(c => c.CourseId == courseId);

            if (course == null)
            {
                return NotFound("Course not found.");
            }

            return View(course);
        }

        // 2. Asynchronously retrieves user records from external JSONPlaceholder endpoint
        public async Task<IActionResult> Users()
        {
            var client = _httpClientFactory.CreateClient();

            // Perform async HTTP GET request
            var response = await client.GetAsync("https://jsonplaceholder.typicode.com/users");

            List<ExternalUser> userList = new();

            if (response.IsSuccessStatusCode)
            {
                // Asynchronously read response stream
                var jsonContent = await response.Content.ReadAsStringAsync();

                // Deserialize JSON into model objects
                userList = JsonSerializer.Deserialize<List<ExternalUser>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new();
            }

            return View(userList);
        }

        // 3. State Management: Enrollment Page (GET)
        [HttpGet]
        public IActionResult Enroll(int courseId)
        {
            var courses = GetSampleCourses();
            var course = courses.FirstOrDefault(c => c.CourseId == courseId);

            if (course == null)
            {
                return NotFound();
            }

            // Read student name from cookie if already set
            ViewBag.StudentName = Request.Cookies["StudentName"] ?? string.Empty;

            return View(course);
        }

        // 3. State Management: Form Submission (POST) handling Hidden Field, Cookie, and Session
        [HttpPost]
        public IActionResult Enroll(int courseId, string courseName, string studentName)
        {
            // Store student name in Cookie (valid for 7 days)
            CookieOptions cookieOpts = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                HttpOnly = true
            };
            Response.Cookies.Append("StudentName", studentName, cookieOpts);

            // Store selected course name in Session
            HttpContext.Session.SetString("SelectedCourse", courseName);

            return RedirectToAction("Confirmation");
        }

        // 3. Displays state verification (Values from Cookie and Session)
        public IActionResult Confirmation()
        {
            // Read from Cookie
            ViewBag.StudentName = Request.Cookies["StudentName"] ?? "Not Found";

            // Read from Session
            ViewBag.SelectedCourse = HttpContext.Session.GetString("SelectedCourse") ?? "Not Found";

            return View();
        }
    }
}