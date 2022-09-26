using MyProfile.Controllers;

namespace MyProfile.Models
{
    public class IndexModel
    {
        public User User { get; set; }
        public Record[] Messages { get; set; }
        public PictureOnTheWall[] Pictures { get; set; }
        public LinkInfo[] linkInfos { get; set; }
    }
}
