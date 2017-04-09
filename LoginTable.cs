//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
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
    }    
}