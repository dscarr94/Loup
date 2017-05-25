using SQLite;

namespace OnQAndroid
{
    public class MyAttributes
    {
        [PrimaryKey, Column("_Id")]
        public int id { get; set; }
        [MaxLength(25)]
        public string name { get; set; }
        [MaxLength(25)]
        public string email { get; set; }
        [MaxLength(25)]
        public string password { get; set; }
        [MaxLength(25)]
        public string type { get; set; }
        [MaxLength(25)]
        public string attribute1 { get; set; }
        [MaxLength(25)]
        public string attribute2 { get; set; }
        [MaxLength(25)]
        public string attribute3 { get; set; }
        [MaxLength(30)]
        public string attribute4 { get; set; }
        [MaxLength(8)]
        public int cfid { get; set; }
        [MaxLength(5)]
        public int loginid { get; set; }
        [MaxLength(5)]
        public int typeid { get; set; }
        public bool rememberme { get; set; }
    }
}