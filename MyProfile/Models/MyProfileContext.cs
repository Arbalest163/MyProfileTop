using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MyProfile.Models {
    public class MyProfileContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<TypeInfo> TypesInfo { get; set; }
        public DbSet<UserContactInfo> UsersInfo { get; set; }
        public DbSet<Record> Records { get; set; }
        public DbSet<PictureOnTheWall> PicturesOnTheWall { get; set; }

        public MyProfileContext(DbContextOptions<MyProfileContext> options) : base (options)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();

            if (TypesInfo.Any() == false) { 
                TypesInfo.AddRange(InitData.GetTypeInfos());
                SaveChanges();
            }
        }
    }
}
