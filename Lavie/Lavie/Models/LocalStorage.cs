using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lavie.Models
{
    public class LocalStorage : DbRecord
    {
        [Indexed]
        public string Key { get; set; } = "";
        public string Value { get; set; } = "";
    }
}
