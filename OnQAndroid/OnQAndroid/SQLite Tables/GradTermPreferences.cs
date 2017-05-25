using SQLite;

namespace OnQAndroid.SQLite_Tables
{
    class GradTermPreferences
    {
        [PrimaryKey]
        public int id { get; set; }
        [MaxLength(30)]
        public string gradterm { get; set; }
    }
}