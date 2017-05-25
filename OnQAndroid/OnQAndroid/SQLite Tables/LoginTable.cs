using SQLite;

namespace OnQAndroid
{
    public class LoginTable
    {
        [PrimaryKey, AutoIncrement, Column("_Id")]
        public int id { get; set; }
        [MaxLength(25)]
        public string name { get; set; }
        [MaxLength(25)]
        public string email { get; set; }
        [MaxLength(15)]
        public string password { get; set; }
        [MaxLength(20)]
        public string type { get; set; }
        [MaxLength(8)]
        public int cfid { get; set; }
    }    
}