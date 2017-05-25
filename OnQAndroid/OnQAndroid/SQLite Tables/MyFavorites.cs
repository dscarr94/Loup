using SQLite;

namespace OnQAndroid.SQLite_Tables
{
    class MyFavorites
    {
        [PrimaryKey, Column("_Id")]
        public int id { get; set; }
        [MaxLength(5)]
        public bool isFavorite { get; set; }
    }
}