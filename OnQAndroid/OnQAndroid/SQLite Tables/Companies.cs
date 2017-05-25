using SQLite;

namespace OnQAndroid
{
    public class Companies
    {
        [PrimaryKey, AutoIncrement, Column("_Id")]
        public int id { get; set; }
        [MaxLength(25)]
        public string name { get; set; }
        [MaxLength(25)]
        public string description { get; set; }
        [MaxLength(50)]
        public string website { get; set; }
        [MaxLength(20)]
        public string rak { get; set; }
    }
}