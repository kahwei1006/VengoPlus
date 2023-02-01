using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Lavie.Models
{
    public abstract class DbRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } = 0;
    }
}
