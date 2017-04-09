using SQLite;

namespace OnQAndroid
{
    public class StudentTable
    {
        [PrimaryKey, AutoIncrement, Column("_Id")]
        public int id { get; set; }
        [MaxLength(25)]
        public string name { get; set; }
        [MaxLength(25)]
        public string email { get; set; }
        [MaxLength(15)]
        public string password { get; set; }
        [MaxLength(15)]
        public string gradterm { get; set; }
        [MaxLength(30)]
        public string major { get; set; }
        [MaxLength(5)]
        public string gpa { get; set; }
    }
}