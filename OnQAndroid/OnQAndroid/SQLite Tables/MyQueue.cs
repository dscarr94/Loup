using SQLite;

namespace OnQAndroid.SQLite_Tables
{
    class MyQueue
    {
        [PrimaryKey]
        public int id { get; set; }
        [MaxLength(40)]
        public string company { get; set; }
        [MaxLength(10)]
        public int position { get; set; }
    }
}