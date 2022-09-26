using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Models {
    public class MyProfileContext : DbContext {
        public MyProfileContext(DbContextOptions<MyProfileContext> context)
            : base (context) {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();

            if (TypesInfo.Any() == false) { 
                TypesInfo.Add(new TypeInfo { NameType = "Telegram", isVisible = true });
                TypesInfo.Add(new TypeInfo { NameType = "GitHub", isVisible = true });
                TypesInfo.Add(new TypeInfo { NameType = "TikTok", isVisible = true });
                TypesInfo.Add(new TypeInfo { NameType = "VK", isVisible = true });
                TypesInfo.Add(new TypeInfo { NameType = "Facebook", isVisible = true });
                TypesInfo.Add(new TypeInfo { NameType = "LinkedIn", isVisible = true });
                TypesInfo.Add(new TypeInfo { NameType = "StackOverFlow", isVisible = true });
                SaveChanges();
            }
        }
        public DbSet<User> Users { get; set; }
        public DbSet<TypeInfo> TypesInfo { get; set; } 
        public DbSet<UserContactInfo> UsersInfo { get; set; }
        public DbSet<Record> Records { get; set; }
        public DbSet<PictureOnTheWall> PicturesOnTheWall { get; set; }
    }
}
