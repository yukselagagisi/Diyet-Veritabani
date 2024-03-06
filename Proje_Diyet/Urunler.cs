using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proje_Diyet
{
    public partial class Urunler : Form
    {
        public Urunler()
        {
            InitializeComponent();
        }

        private void Listele() 
        {
            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString)) 
            {
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT URUNID, WPAD, WPSOYAD, URUNAD, TARIH FROM TBLVERILENURUN LEFT JOIN TBLWP ON TBLVERILENURUN.WPID = TBLWP.WPID", conn)) 
                {
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd)) 
                    {
                        using (DataTable dt = new DataTable()) 
                        {
                            da.Fill(dt);
                            dataGridView1.DataSource = dt;
                        }
                    }
                }
            }
        }

        private void Temizle() 
        {
            UrunIDTxt.Clear();
            IsımTxt.Clear();
            SoyadTxt.Clear();
            UrunAdTxt.Clear();
            TarihDtp.Value = DateTimePicker.MinimumDateTime;
        }

        private bool BosMu() 
        {
            return (UrunIDTxt.Text.Length == 0 && 
                    IsımTxt.Text.Length == 0 &&
                    SoyadTxt.Text.Length == 0 &&
                    UrunAdTxt.Text.Length == 0);
        }

        private void Urunler_Load(object sender, EventArgs e)
        {
            Listele();

            TarihDtp.Value = DateTimePicker.MinimumDateTime; // Tarihi default olarak minimum time'a ayarlıyoruz çünkü böyle yapmazsak bugünün tarihini alıyor ve arama yaparken yaptığımız aramaya ek olarak bugünün verilerini de getiriyor
        }

        private void TemizleBtn_Click(object sender, EventArgs e)
        {
            Temizle();  
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return; // Headerlara (Column başlıklarına) tıklayınca gelen hatayı kaldırmak için kullanıldı

            UrunIDTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            IsımTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            SoyadTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            UrunAdTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            TarihDtp.Text = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells[4].Value).ToString();
        }

        private void ListeleBtn_Click(object sender, EventArgs e)
        {
            Listele();
        }

        private void AraBtn_Click(object sender, EventArgs e)
        {
            if (BosMu()) return;

            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString)) 
            {

                using (SQLiteCommand cmd = new SQLiteCommand("SELECT URUNID, WPAD, WPSOYAD, URUNAD FROM TBLVERILENURUN INNER JOIN TBLWP ON TBLVERILENURUN.WPID = TBLWP.WPID WHERE URUNID like '%' || @p1 || '%' OR WPAD like '%' || @p2 || '%' OR WPSOYAD like '%' || @p3 || '%' OR URUNAD like '%' || @p4 || '%' or TARIH=@p5", conn)) 
                {
                    cmd.Parameters.AddWithValue("@p1", UrunIDTxt.Text.Length > 0 ? Convert.ToInt32(UrunIDTxt.Text) : -1);
                    cmd.Parameters.AddWithValue("@p2", IsımTxt.Text.Length > 0 ? IsımTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p3", SoyadTxt.Text.Length > 0 ? SoyadTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p4", UrunAdTxt.Text.Length > 0 ? UrunAdTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p5", TarihDtp.Text.Length > 0 ? Convert.ToDateTime(TarihDtp.Text).ToShortDateString() : DateTime.Now.ToShortDateString());

                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd)) 
                    {
                        using (DataTable dt = new DataTable()) 
                        {
                            da.Fill(dt);
                            dataGridView1.DataSource = dt;
                        }
                    }

                }
            }
        }

        private void EkleBtn_Click(object sender, EventArgs e)
        {
            if (BosMu()) return;

            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString))
            {
                conn.Open();
                using(SQLiteCommand cmd = new SQLiteCommand("SELECT WPID FROM TBLWP WHERE WPAD=@p1 AND WPSOYAD=@p2", conn)) 
                {
                    cmd.Parameters.AddWithValue("@p1", IsımTxt.Text.Length > 0 ? IsımTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p2", SoyadTxt.Text.Length > 0 ? SoyadTxt.Text.ToUpper() : "-1");

                    using (SQLiteDataReader dr = cmd.ExecuteReader()) 
                    {
                        if (dr.HasRows) 
                        {
                            dr.Read(); // Bunu kapatmayı unutma

                            using (SQLiteCommand cmd2 = new SQLiteCommand("INSERT INTO TBLVERILENURUN (WPID, URUNAD, TARIH) VALUES (@p2, @p3,@p4)",conn)) 
                            {
                                cmd2.Parameters.AddWithValue("@p2", dr[0].ToString());
                                cmd2.Parameters.AddWithValue("@p3", UrunAdTxt.Text.Length > 0 ? UrunAdTxt.Text.ToUpper() : "-1");
                                cmd2.Parameters.AddWithValue("@p4", TarihDtp.Text.Length > 0 ? Convert.ToDateTime(TarihDtp.Text).ToShortDateString() : DateTime.Now.ToShortDateString());

                                cmd2.ExecuteNonQuery();
                                dr.Close();

                                MessageBox.Show("Başarıyla ekleme yapıldı");
                            }

                        }
                        else 
                        {
                            MessageBox.Show("Girilen İsim veya Soyisim Bulunamadı!");
                        }
                    }
                }
                conn.Close();
            }

            Listele();
            Temizle();
        }

        private void SilBtn_Click(object sender, EventArgs e)
        {
            if (BosMu()) return;

            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString)) 
            {
                conn.Open();

                using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM TBLVERILENURUN WHERE URUNID=@p1", conn)) 
                {
                    cmd.Parameters.AddWithValue("@p1", UrunIDTxt.Text.Length > 0 ? Convert.ToInt32(UrunIDTxt.Text) : -1);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Silme işlemi başarılı");
                }

                conn.Close();
            }

            Listele();
            Temizle();
        }

        private void GuncelleBtn_Click(object sender, EventArgs e)
        {
            if (!BosMu()) return;

            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString)) 
            {
                conn.Open();

                using (SQLiteCommand cmd = new SQLiteCommand("UPDATE TBLVERILENURUN SET URUNAD=@p1, TARIH=@p2 WHERE URUNID=@p3",conn)) 
                {
                    cmd.Parameters.AddWithValue("@p1", UrunAdTxt.Text.Length > 0 ? UrunAdTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p2", TarihDtp.Text.Length > 0 ? Convert.ToDateTime(TarihDtp.Text).ToShortDateString() : DateTime.Now.ToShortDateString());
                    cmd.Parameters.AddWithValue("@p3", UrunIDTxt.Text.Length > 0 ? Convert.ToInt32(UrunIDTxt.Text) : -1);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Güncelleme işlemi başarılı");
                }

                conn.Close();
            }

            Listele();
            Temizle();
        }

        private void Urunler_FormClosed(object sender, FormClosedEventArgs e)
        {
            AnaMenu.urunler = null;
        }
    }
}
