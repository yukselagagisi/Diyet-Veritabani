using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Text;

namespace Proje_Diyet
{
    public class Sorgular
    {
        public static string connectionString = @"Data Source = files\diyet.db;foreign keys=true;Pooling=false;";

        public void CreateTables()
        {
            if (!File.Exists(@"files\diyet.db")) 
            {
                if (!Directory.Exists(@"files\"))
                {
                    DirectoryInfo di = Directory.CreateDirectory(@"files\");
                }

                SQLiteConnection.CreateFile(@"files\diyet.db");

                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    SQLiteCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "CREATE TABLE TBLDIYETKATEGORI (KATEGORIID INTEGER PRIMARY KEY AUTOINCREMENT, KATEGORIAD VARCHAR(20))";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "CREATE TABLE TBLDANISANLAR (DANISANID INTEGER PRIMARY KEY AUTOINCREMENT, DANISANISIM VARCHAR(20), DANISANSOYAD VARCHAR(20), DANISANSEHIR VARCHAR(15), DANISANILCE VARCHAR(15), DANISANTEL VARCHAR(11), DANISANMAIL VARCHAR(30), DIYETKATEGORI INTEGER, DURUM BIT DEFAULT 1 NOT NULL, FOREIGN KEY(DIYETKATEGORI) REFERENCES TBLDIYETKATEGORI(KATEGORIID) ON DELETE CASCADE ON UPDATE CASCADE )";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "CREATE TABLE TBLHAREKET (HAREKETID INTEGER PRIMARY KEY AUTOINCREMENT, DANISAN INTEGER, TUTAR INTEGER, TARIH SMALLDATETIME, FOREIGN KEY(DANISAN) REFERENCES TBLDANISANLAR(DANISANID) ON DELETE CASCADE ON UPDATE CASCADE)";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "CREATE TABLE TBLDOSYALAR (DOSYAID INTEGER PRIMARY KEY AUTOINCREMENT, DANISAN INTEGER, DOSYAAD VARCHAR(20), DOSYAYOLU TEXT, FOREIGN KEY(DANISAN) REFERENCES TBLDANISANLAR(DANISANID) ON DELETE CASCADE ON UPDATE CASCADE)  ";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "CREATE TABLE TBLETIKET (ETIKETID INTEGER PRIMARY KEY AUTOINCREMENT, ETIKETAD VARCHAR(20))";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "CREATE TABLE TBLWP (WPID INTEGER PRIMARY KEY AUTOINCREMENT, WPAD VARCHAR(20), WPSOYAD VARCHAR(20), TELNO VARCHAR(11), ETIKET INTEGER, FOREIGN KEY(ETIKET) REFERENCES TBLETIKET(ETIKETID) ON DELETE CASCADE ON UPDATE CASCADE)";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "CREATE TABLE TBLVERILENURUN (URUNID INTEGER PRIMARY KEY AUTOINCREMENT, WPID INTEGER, URUNAD VARCHAR(20), TARIH SMALLDATETIME, FOREIGN KEY(WPID) REFERENCES TBLWP(WPID) ON DELETE CASCADE ON UPDATE CASCADE )";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "CREATE TABLE TBLONERI (ONERIID INTEGER PRIMARY KEY AUTOINCREMENT, WPID INTEGER, ONERIAD VARCHAR(20), ONERI TEXT, FOREIGN KEY(WPID) REFERENCES TBLWP(WPID) ON DELETE CASCADE ON UPDATE CASCADE )";
                    cmd.ExecuteNonQuery();
                    conn.Close();
                } 
            }
            
        }
    }
}
