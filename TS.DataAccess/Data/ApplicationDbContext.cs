using TS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace TS.DataAccess.Data
{
    public class ApplicationDbContext: IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
                
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Reinstatements", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Overbanding", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Antiskid", DisplayOrder = 3 }
                );

            modelBuilder.Entity<Product>().HasData(
                new Product 
                {
                    Id = 1,
                    Title = "Bitucrete",
                    Author = "",
                    Description = "Available in a variety of grades, Bitucrete can be used for footway repairs, patching, reinstatements and temporary surfacing.",
                    ISBN = "SWD9999001",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
                    CategoryId = 1,
                    ImageUrl = "/Pictures/bitucrete.jpg"
                },
                new Product
                {
                    Id = 2,
                    Title = "Colpatch",
                    Author = "",
                    Description = "Colpatch is a BBA HAPAS approved, cold applied storage grade macadam that is ideal for instant, high quality repairs to bituminous and concrete surfaces",
                    ISBN = "CAW777777701",
                    ListPrice = 40,
                    Price = 30,
                    Price50 = 25,
                    Price100 = 20,
                    CategoryId = 1,
                    ImageUrl = "/Pictures/colpatch.jpg"
                },
                new Product
                {
                    Id = 3,
                    Title = "Bitukold",
                    Author = "",
                    Description = "Bitukold is a cold hand applied, thixotropic bitumen emulsion designed for sealing vertical joints",
                    ISBN = "RITO5555501",
                    ListPrice = 55,
                    Price = 50,
                    Price50 = 40,
                    Price100 = 35,
                    CategoryId = 1,
                    ImageUrl = "/Pictures/bitukold.jpg"
                },
                new Product
                {
                    Id = 4,
                    Title = "Colstrips",
                    Author = "",
                    Description = "Colstrips are HAPAS approved skid resistant overbanding and crack sealing thermoplastic strips, designed to seal and repair static cracks up to 5mm wide and 20mm deep in nonporous bituminous and concrete highways",
                    ISBN = "WS3333333301",
                    ListPrice = 70,
                    Price = 65,
                    Price50 = 60,
                    Price100 = 55,
                    CategoryId = 2,
                    ImageUrl = "/Pictures/colstrips.jpg"
                },
                new Product
                {
                    Id = 5,
                    Title = "Gripstrip",
                    Author = "",
                    Description = "Gripstrip is an anti-skid, self-adhesive, overbanding joint and crack sealing tape. Gripstrip consists of an extruded bituminous compound modified with elastomers and fast bonding resins",
                    ISBN = "SOTJ1111111101",
                    ListPrice = 30,
                    Price = 27,
                    Price50 = 25,
                    Price100 = 20,
                    CategoryId = 2,
                    ImageUrl = "/Pictures/gripstrip.jpg"
                },
                new Product
                {
                    Id = 6,
                    Title = "Colasgrip",
                    Author = "",
                    Description = "Colasgrip is a high performance high friction and coloured surfacing. It comprises of a two component thermosetting pigmentable epoxy resin binder dressed with natural or pigmented coloured aggregates, normally calcined bauxite or granite",
                    ISBN = "FOT000000001",
                    ListPrice = 25,
                    Price = 23,
                    Price50 = 22,
                    Price100 = 20,
                    CategoryId = 3,
                    ImageUrl = "/Pictures/colasgrip.jpg"
                },
                new Product
                {
                    Id = 7,
                    Title = "Jointgrip ",
                    Author = "",
                    Description = "Jointgrip OB is an anti-skid overbanding joint sealant. It is a proprietary formulation of blended paving grade bitumen incorporating special polymers and abrasive aggregates to provide a compound of toughness, flexibility, adhesiveness and resistance to flow.",
                    ISBN = "FOT000000001",
                    ListPrice = 25,
                    Price = 23,
                    Price50 = 22,
                    Price100 = 20,
                    CategoryId = 2,
                    ImageUrl = "/Pictures/jointgrip.jpg"
                },
                new Product
                {
                    Id = 8,
                    Title = "Tac-Pads",
                    Author = "",
                    Description = "Tac-Pads are a pressure sensitive self-adhesive tile, made from Methacrylate resin. Requiring no heat.",
                    ISBN = "FOT000000001",
                    ListPrice = 25,
                    Price = 23,
                    Price50 = 22,
                    Price100 = 20,
                    CategoryId = 3,
                    ImageUrl = "/Pictures/tacpad1-1.jpg"
                }
                );
        }
    }
}
