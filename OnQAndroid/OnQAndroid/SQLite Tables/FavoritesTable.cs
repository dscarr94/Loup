using SQLite;

namespace OnQAndroid
{
    public class FavoritesTable
    {
        [PrimaryKey, Column("_Id")]
        public int id { get; set; }
        [MaxLength(40)]
        public string company { get; set; }
    }
}