using SQLite;

namespace OnQAndroid
{
    public class Cfids
    {
        [PrimaryKey, AutoIncrement, Column("_Id")]
        public int id { get; set; }
        [MaxLength(25)]
        public string cfid { get; set; }
        [MaxLength(40)]
        public string name { get; set; }
    }
}