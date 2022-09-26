using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Models {
    public class UserContactInfo {
        public int Id { get; set; }
        public int User_id { get; set; }
        public int TypeInfo_id { get; set; }
        public string Info { get; set; }
    }

    public class TypeInfo {
        public int Id { get; set; }
        public string NameType { get; set; }
        public bool isVisible { get; set; }
    }
}
