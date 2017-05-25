using SQLite;

namespace OnQAndroid.SQLite_Tables
{
    class PastQueue
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        [MaxLength(5)]
        public int studentid { get; set; }
        public string notes { get; set; }
        [MaxLength(1)]
        public int rating { get; set; }
    }
}