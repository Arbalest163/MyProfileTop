using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyProfile.Models
{
    public class InitData
    {
        public static IEnumerable<TypeInfo> GetTypeInfos()
        {
            yield return new TypeInfo { NameType = "Telegram", isVisible = true };
            yield return new TypeInfo { NameType = "GitHub", isVisible = true };
            yield return new TypeInfo { NameType = "TikTok", isVisible = true };
            yield return new TypeInfo { NameType = "VK", isVisible = true };
            yield return new TypeInfo { NameType = "Facebook", isVisible = true };
            yield return new TypeInfo { NameType = "LinkedIn", isVisible = true };
            yield return new TypeInfo { NameType = "StackOverFlow", isVisible = true };
        }
    }
}
