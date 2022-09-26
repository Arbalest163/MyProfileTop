using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Models {
    public class Record {
        public int Id {get; set; }
        public int User_id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
