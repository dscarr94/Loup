using SQLite;

namespace OnQAndroid.SQLite_Tables
{
    class Queue
    {
        [PrimaryKey]
        public int id { get; set; }
        [MaxLength(5)]
        public int studentid { get; set; }
    }
}