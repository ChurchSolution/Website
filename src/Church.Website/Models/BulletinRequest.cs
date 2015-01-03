using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Church.Website.Models
{
    public class BulletinRequest
    {
        public DateTime Date { get; set; }

        public string TextFileContent { get; set; }

        public string PrintFileContent { get; set; }
    }
}