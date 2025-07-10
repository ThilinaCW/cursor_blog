using BlogApi.Core.Entities;
using BlogApi.Core.Enums;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace BlogApi.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(BlogDbContext context)
        {
            // Only seed if there are no users and no blog posts
            if (context.Users.Any() || context.BlogPosts.Any())
            {
                // Database already seeded
                return;
            }

            // Seed Users
            var users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    Email = "admin@lumora.com",
                    PasswordHash = HashPassword("admin123"),
                    FirstName = "Admin",
                    LastName = "User",
                    Bio = "System Administrator",
                    Role = UserRole.Admin,
                    IsActive = true,
                    IsApproved = true,
                    CreatedDate = DateTime.UtcNow,
                    ApprovedDate = DateTime.UtcNow
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "john_doe",
                    Email = "john@example.com",
                    PasswordHash = HashPassword("password123"),
                    FirstName = "John",
                    LastName = "Doe",
                    Bio = "Regular user",
                    Role = UserRole.User,
                    IsActive = true,
                    IsApproved = false,
                    CreatedDate = DateTime.UtcNow
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "jane_smith",
                    Email = "jane@example.com",
                    PasswordHash = HashPassword("password123"),
                    FirstName = "Jane",
                    LastName = "Smith",
                    Bio = "Another regular user",
                   Role = UserRole.Moderator,
                    IsActive = true,
                    IsApproved = false,
                    CreatedDate = DateTime.UtcNow
                }
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();

            // Seed Blog Posts
            var posts = new List<BlogPost>
            {
                new BlogPost
                {
                    Id = Guid.NewGuid(),
                    Title = "The Art of Southern Cooking",
                    Content = "Southern cuisine is more than just food; it's a way of life. From the smoky flavors of barbecue to the comforting warmth of grits, every dish tells a story of tradition and community. This article explores the rich history and techniques behind some of the South's most beloved recipes.",
                    Author = "Sarah Johnson",
                    ImageUrl = "https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?w=800",
                    IsFeatured = true,
                    IsApproved = true,
                    CategoriesJson = JsonSerializer.Serialize(new[] { "Food", "Culture", "Southern" }),
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new BlogPost
                {
                    Id = Guid.NewGuid(),
                    Title = "Music of the Mississippi Delta",
                    Content = "The Mississippi Delta has been the birthplace of some of America's most influential music. From the blues of Robert Johnson to the soul of Otis Redding, this region has shaped the sound of American music for generations. Discover the stories behind the songs and the people who made them.",
                    Author = "Marcus Williams",
                    ImageUrl = "https://images.unsplash.com/photo-1493225457124-a3eb161ffa5f?w=800",
                    IsFeatured = true,
                    IsApproved = true,
                    CategoriesJson = JsonSerializer.Serialize(new[] { "Music", "History", "Blues" }),
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new BlogPost
                {
                    Id = Guid.NewGuid(),
                    Title = "The Changing Face of Southern Cities",
                    Content = "Southern cities are experiencing a renaissance, blending historic charm with modern innovation. From Atlanta's tech boom to Nashville's music scene, these cities are redefining what it means to be Southern in the 21st century.",
                    Author = "Emily Davis",
                    ImageUrl = "https://images.unsplash.com/photo-1449824913935-59a10b8d2000?w=800",
                    IsFeatured = true,
                    IsApproved = true,
                    CategoriesJson = JsonSerializer.Serialize(new[] { "Urban", "Development", "Southern" }),
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new BlogPost
                {
                    Id = Guid.NewGuid(),
                    Title = "Preserving Southern Architecture",
                    Content = "From grand plantation homes to humble shotgun houses, Southern architecture reflects the region's complex history and diverse cultural influences. This article examines efforts to preserve these architectural treasures for future generations.",
                    Author = "Robert Chen",
                    ImageUrl = "https://images.unsplash.com/photo-1564013799919-ab600027ffc6?w=800",
                    IsFeatured = true,
                    IsApproved = true,
                    CategoriesJson = JsonSerializer.Serialize(new[] { "Architecture", "History", "Preservation" }),
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new BlogPost
                {
                    Id = Guid.NewGuid(),
                    Title = "The Literary South",
                    Content = "Southern literature has produced some of America's greatest writers, from William Faulkner to Flannery O'Connor. This piece explores the themes and styles that make Southern writing unique and enduring.",
                    Author = "Jennifer Smith",
                    ImageUrl = "https://images.unsplash.com/photo-1481627834876-b7833e8f5570?w=800",
                    IsFeatured = true,
                    IsApproved = true,
                    CategoriesJson = JsonSerializer.Serialize(new[] { "Literature", "Culture", "Southern" }),
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new BlogPost
                {
                    Id = Guid.NewGuid(),
                    Title = "Southern Hospitality in the Digital Age",
                    Content = "How do we maintain the warmth and personal connection of Southern hospitality in our increasingly digital world? This article explores ways to blend traditional values with modern communication.",
                    Author = "Amanda Wilson",
                    ImageUrl = "https://images.unsplash.com/photo-1516321318423-f06f85e504b3?w=800",
                    IsFeatured = false,
                    IsApproved = false,
                    CategoriesJson = JsonSerializer.Serialize(new[] { "Culture", "Technology", "Southern" }),
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new BlogPost
                {
                    Id = Guid.NewGuid(),
                    Title = "The Future of Southern Agriculture",
                    Content = "Climate change and technological advances are transforming Southern agriculture. From sustainable farming practices to smart irrigation systems, farmers are adapting to meet the challenges of the 21st century.",
                    Author = "David Thompson",
                    ImageUrl = "https://images.unsplash.com/photo-1500937386664-56d1dfef3854?w=800",
                    IsFeatured = false,
                    IsApproved = false,
                    CategoriesJson = JsonSerializer.Serialize(new[] { "Agriculture", "Technology", "Environment" }),
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new BlogPost
                {
                    Id = Guid.NewGuid(),
                    Title = "Southern Festivals and Celebrations",
                    Content = "From Mardi Gras in New Orleans to the Kentucky Derby, Southern festivals bring communities together in celebration of culture, tradition, and shared heritage. Discover the stories behind these beloved events.",
                    Author = "Lisa Rodriguez",
                    ImageUrl = "https://images.unsplash.com/photo-1533174072545-7a4b6ad7a6c3?w=800",
                    IsFeatured = false,
                    CategoriesJson = JsonSerializer.Serialize(new[] { "Festivals", "Culture", "Community" }),
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new BlogPost
                {
                    Id = Guid.NewGuid(),
                    Title = "The Art of Storytelling",
                    Content = "Storytelling is at the heart of Southern culture. Whether around a dinner table or on a front porch, Southerners have perfected the art of weaving tales that entertain, educate, and connect generations.",
                    Author = "Michael Brown",
                    ImageUrl = "https://images.unsplash.com/photo-1513475382585-d06e58bcb0e0?w=800",
                    IsFeatured = false,
                    CategoriesJson = JsonSerializer.Serialize(new[] { "Culture", "Storytelling", "Southern" }),
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new BlogPost
                {
                    Id = Guid.NewGuid(),
                    Title = "Southern Wildlife and Conservation",
                    Content = "The South is home to diverse ecosystems and unique wildlife. This article examines conservation efforts to protect endangered species and preserve the region's natural beauty for future generations.",
                    Author = "Rachel Green",
                    ImageUrl = "https://images.unsplash.com/photo-1549366021-9f761d450615?w=800",
                    IsFeatured = false,
                    CategoriesJson = JsonSerializer.Serialize(new[] { "Wildlife", "Conservation", "Environment" }),
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new BlogPost
                {
                    Id = Guid.NewGuid(),
                    Title = "The Evolution of Southern Fashion",
                    Content = "Southern fashion has evolved from traditional seersucker suits to modern streetwear, while maintaining its distinctive charm and elegance. Explore the trends and designers shaping Southern style today.",
                    Author = "Sophia Martinez",
                    ImageUrl = "https://images.unsplash.com/photo-1445205170230-053b83016050?w=800",
                    IsFeatured = false,
                    CategoriesJson = JsonSerializer.Serialize(new[] { "Fashion", "Style", "Southern" }),
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new BlogPost
                {
                    Id = Guid.NewGuid(),
                    Title = "Southern Food Trucks and Street Food",
                    Content = "Food trucks are bringing innovative takes on Southern classics to the streets. From gourmet biscuits to fusion tacos, discover how entrepreneurs are reimagining traditional Southern cuisine.",
                    Author = "Kevin Lee",
                    ImageUrl = "https://images.unsplash.com/photo-1565299624946-b28f40a0ca4b?w=800",
                    IsFeatured = false,
                    CategoriesJson = JsonSerializer.Serialize(new[] { "Food", "Street Food", "Entrepreneurship" }),
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new BlogPost
                {
                    Id = Guid.NewGuid(),
                    Title = "The Rise of Southern Craft Beer",
                    Content = "Craft breweries are flourishing across the South, creating unique beers that reflect local flavors and traditions. Meet the brewers and discover the stories behind their innovative creations.",
                    Author = "Patrick O'Connor",
                    ImageUrl = "https://images.unsplash.com/photo-1510812431401-41d2bd2722f3?w=800",
                    IsFeatured = false,
                    CategoriesJson = JsonSerializer.Serialize(new[] { "Craft Beer", "Brewing", "Local Business" }),
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new BlogPost
                {
                    Id = Guid.NewGuid(),
                    Title = "Southern Gardens and Landscaping",
                    Content = "Southern gardens are known for their beauty and resilience. From azaleas to magnolias, discover the plants and design principles that make Southern landscapes so distinctive.",
                    Author = "Helen Foster",
                    ImageUrl = "https://images.unsplash.com/photo-1416879595882-3373a0480b5b?w=800",
                    IsFeatured = false,
                    CategoriesJson = JsonSerializer.Serialize(new[] { "Gardening", "Landscaping", "Nature" }),
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new BlogPost
                {
                    Id = Guid.NewGuid(),
                    Title = "The Digital Transformation of Southern Businesses",
                    Content = "Southern businesses are embracing digital technology while maintaining their personal touch. Learn how companies are balancing innovation with tradition in the digital age.",
                    Author = "Alex Johnson",
                    ImageUrl = "https://images.unsplash.com/photo-1460925895917-afdab827c52f?w=800",
                    IsFeatured = false,
                    CategoriesJson = JsonSerializer.Serialize(new[] { "Business", "Technology", "Innovation" }),
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new BlogPost
                {
                    Id = Guid.NewGuid(),
                    Title = "Southern Art and Artists",
                    Content = "From folk art to contemporary installations, Southern artists are creating works that reflect the region's rich cultural heritage and address modern social issues.",
                    Author = "Maria Garcia",
                    ImageUrl = "https://images.unsplash.com/photo-1541961017774-22349e4a1262?w=800",
                    IsFeatured = false,
                    CategoriesJson = JsonSerializer.Serialize(new[] { "Art", "Culture", "Southern" }),
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new BlogPost
                {
                    Id = Guid.NewGuid(),
                    Title = "The Future of Southern Education",
                    Content = "Southern universities and schools are adapting to meet the needs of a changing world. From online learning to innovative programs, discover how education is evolving in the South.",
                    Author = "Dr. James Wilson",
                    ImageUrl = "https://images.unsplash.com/photo-1523050854058-8df90110c9e1?w=800",
                    IsFeatured = false,
                    CategoriesJson = JsonSerializer.Serialize(new[] { "Education", "Innovation", "Southern" }),
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new BlogPost
                {
                    Id = Guid.NewGuid(),
                    Title = "Southern Sports and Recreation",
                    Content = "From college football to outdoor adventures, sports and recreation play a vital role in Southern culture. Explore the traditions and innovations shaping Southern athletics.",
                    Author = "Tom Anderson",
                    ImageUrl = "https://images.unsplash.com/photo-1571019613454-1cb2f99b2d8b?w=800",
                    IsFeatured = false,
                    CategoriesJson = JsonSerializer.Serialize(new[] { "Sports", "Recreation", "Southern" }),
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new BlogPost
                {
                    Id = Guid.NewGuid(),
                    Title = "The Impact of Climate Change on the South",
                    Content = "Climate change is affecting Southern communities in unique ways. This article examines the challenges and opportunities for adaptation and resilience in the region.",
                    Author = "Dr. Sarah Miller",
                    ImageUrl = "https://images.unsplash.com/photo-1569163139394-de4e4f43e8e3?w=800",
                    IsFeatured = false,
                    CategoriesJson = JsonSerializer.Serialize(new[] { "Climate Change", "Environment", "Southern" }),
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new BlogPost
                {
                    Id = Guid.NewGuid(),
                    Title = "Southern Healthcare and Wellness",
                    Content = "Healthcare in the South is evolving to meet diverse community needs. From traditional remedies to modern medicine, discover how Southern communities are promoting health and wellness.",
                    Author = "Dr. Robert Davis",
                    ImageUrl = "https://images.unsplash.com/photo-1559757148-5c350d0d3c56?w=800",
                    IsFeatured = false,
                    CategoriesJson = JsonSerializer.Serialize(new[] { "Healthcare", "Wellness", "Southern" }),
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                },
                new BlogPost
                {
                    Id = Guid.NewGuid(),
                    Title = "The Role of Religion in Southern Culture",
                    Content = "Religion has played a central role in shaping Southern culture and values. This article explores the diverse religious traditions and their influence on Southern life.",
                    Author = "Reverend Thomas Brown",
                    ImageUrl = "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=800",
                    IsFeatured = false,
                    CategoriesJson = JsonSerializer.Serialize(new[] { "Religion", "Culture", "Southern" }),
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                }
            };

            context.BlogPosts.AddRange(posts);
            await context.SaveChangesAsync();
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
} 