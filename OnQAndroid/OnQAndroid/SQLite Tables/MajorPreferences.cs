using SQLite;

namespace OnQAndroid.SQLite_Tables
{
    class MajorPreferences
    {
        [PrimaryKey]
        public int id { get; set; }
        [MaxLength(40)]
        public string major { get; set; }
    }
}