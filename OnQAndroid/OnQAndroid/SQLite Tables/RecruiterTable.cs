using SQLite;

namespace OnQAndroid
{
    public class RecruiterTable
    {
        [PrimaryKey, AutoIncrement, Column("_Id")]
        public int id { get; set; }
        [MaxLength(25)]
        public string name { get; set; }
        [MaxLength(25)]
        public string email { get; set; }
        [MaxLength(15)]
        public string password { get; set; }
        [MaxLength(30)]
        public string company { get; set; }
    }
}