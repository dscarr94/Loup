using SQLite;

namespace OnQAndroid.SQLite_Tables
{
    class GPAPreferences
    {
        [PrimaryKey]
        public int id { get; set; }
        [MaxLength(5)]
        public string gpa { get; set; }
    }
}