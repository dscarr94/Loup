using SQLite;

namespace OnQAndroid.SQLite_Tables
{
    class MyPastQueue
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        [MaxLength(30)]
        public string company { get; set; }
    }
}