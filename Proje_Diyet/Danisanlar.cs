using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proje_Diyet
{
    public partial class Danisanlar : Form
    {
        public Danisanlar()
        {
            InitializeComponent();
        }

        private void Listele()
        {
            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT DANISANID, DANISANISIM, DANISANSOYAD, DANISANSEHIR, DANISANILCE, DANISANTEL, DANISANMAIL, KATEGORIAD FROM TBLDANISANLAR LEFT JOIN TBLDIYETKATEGORI on TBLDANISANLAR.DIYETKATEGORI = TBLDIYETKATEGORI.KATEGORIID", conn)) // Left join yapmamızın sebebi, TBLDIYETKATEGORI sayfasındaki değerlerin, TBLDANISANLAR sayfası boş olduğunda DataGridView içine yazılmasıdır
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

        private bool BosMu()
        {
            return (DanisanIDTxt.Text.Length == 0 &&
            DanisanIsimTxt.Text.Length == 0 &&
            DanisanSoyisimTxt.Text.Length == 0 &&
            DanisanSehirTxt.Text.Length == 0 &&
            DanisanIlceTxt.Text.Length == 0 &&
            DanisanTelNoTxt.Text == "(   )    -" && // MaskedTextBox içerisinde 10 karakterli boş bir telefon numarası template'i taşıyor
            DanisanMailTxt.Text.Length == 0 &&
            DanisanKategoriCmb.Text.Length == 0);
        }

        private void Temizle() 
        {
            DanisanIDTxt.Clear();
            DanisanIsimTxt.Clear();
            DanisanSoyisimTxt.Clear();
            DanisanSehirTxt.Clear();
            DanisanIlceTxt.Clear();
            DanisanTelNoTxt.Clear();
            DanisanMailTxt.Clear();
            DanisanKategoriCmb.ResetText();
            DurumLbl.Text = "Durum: -";
        }

        private void ListeleBtn_Click(object sender, EventArgs e)
        {
            Listele();
        }

        private void Danisanlar_Load(object sender, EventArgs e)
        {
            Listele();

            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString)) // ComboBox'ı doldurmak için kullandığımız alan
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("Select * from TBLDIYETKATEGORI", conn))
                {

                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {

                        while (dr.Read())
                        {
                            DanisanKategoriCmb.Items.Add(dr["KATEGORIAD"]);
                        }
                    }
                }
                conn.Close();
            }

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return; // Headerlara (Column başlıklarına) tıklayınca gelen hatayı kaldırmak için kullanıldı

            DanisanIDTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            DanisanIsimTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            DanisanSoyisimTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            DanisanSehirTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            DanisanIlceTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
            DanisanTelNoTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();
            DanisanMailTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString();
            DanisanKategoriCmb.Text = dataGridView1.Rows[e.RowIndex].Cells[7].Value.ToString();

            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString)) // DURUM KISMININ GETİRİLMESİ
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT DURUM FROM TBLDANISANLAR WHERE DANISANID=@p1", conn)) 
                {
                    cmd.Parameters.AddWithValue("@p1", DanisanIDTxt.Text.Length > 0 ? Convert.ToInt32(DanisanIDTxt.Text) : -1);
                    using (SQLiteDataReader dr = cmd.ExecuteReader()) 
                    {
                        dr.Read();
                        if (dr.HasRows) 
                        {
                            DurumLbl.Text = (dr[0].ToString() == "True" ? "Durum: Aktif" : "Durum: Pasif");
                            dr.Close();
                        }
                    }
                }
                conn.Close();
            }
        }

        private void AraBtn_Click(object sender, EventArgs e)
        {
            if (BosMu()) return;

            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand("Select DANISANID, DANISANISIM, DANISANSOYAD, DANISANSEHIR, DANISANILCE, DANISANTEL, DANISANMAIL, KATEGORIAD from TBLDANISANLAR LEFT JOIN TBLDIYETKATEGORI on TBLDANISANLAR.DIYETKATEGORI = TBLDIYETKATEGORI.KATEGORIID where DANISANID=@p1 or DANISANISIM like '%' || @p2 || '%' or DANISANSOYAD like '%' || @p3 || '%' or DANISANSEHIR like '%' || @p4 || '%' or DANISANILCE like '%' || @p5 || '%' or DANISANTEL = @p6 or DANISANMAIL like '%' || @p7 || '%' or KATEGORIAD like '%' || @p8 || '%'", conn))// Left join yapmamızın sebebi, TBLDIYETKATEGORI sayfasındaki değerlerin, TBLDANISANLAR sayfası boş olduğu zaman arama yaptığımızda, DataGridView içine yazılmasıdır
                {
                    cmd.Parameters.AddWithValue("@p1", DanisanIDTxt.Text.Length > 0 ? Convert.ToInt32(DanisanIDTxt.Text) : -1);
                    cmd.Parameters.AddWithValue("@p2", DanisanIsimTxt.Text.Length > 0 ? DanisanIsimTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p3", DanisanSoyisimTxt.Text.Length > 0 ? DanisanSoyisimTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p4", DanisanSehirTxt.Text.Length > 0 ? DanisanSehirTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p5", DanisanIlceTxt.Text.Length > 0 ? DanisanIlceTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p6", DanisanTelNoTxt.Text.Length > 0 ? DanisanTelNoTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p7", DanisanMailTxt.Text.Length > 0 ? DanisanMailTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p8", DanisanKategoriCmb.Text.Length > 0 ? DanisanKategoriCmb.Text.ToUpper() : "-1");

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

        private void TemizleBtn_Click(object sender, EventArgs e)
        {
            Temizle();
        }

        private void EkleBtn_Click(object sender, EventArgs e)
        {
            if (BosMu()) return;

            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString))
            {
                conn.Open();

                using (SQLiteCommand cmd = DanisanKategoriCmb.Text.Length > 0 ? new SQLiteCommand("INSERT INTO TBLDANISANLAR (DANISANISIM, DANISANSOYAD, DANISANSEHIR, DANISANILCE, DANISANTEL, DANISANMAIL, DIYETKATEGORI) VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7)", conn) : new SQLiteCommand("INSERT INTO TBLDANISANLAR (DANISANISIM, DANISANSOYAD, DANISANSEHIR, DANISANILCE, DANISANTEL, DANISANMAIL) VALUES (@p1, @p2, @p3, @p4, @p5, @p6)",conn))
                {

                    cmd.Parameters.AddWithValue("@p1", DanisanIsimTxt.Text.Length > 0 ? DanisanIsimTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p2", DanisanSoyisimTxt.Text.Length > 0 ? DanisanSoyisimTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p3", DanisanSehirTxt.Text.Length > 0 ? DanisanSehirTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p4", DanisanIlceTxt.Text.Length > 0 ? DanisanIlceTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p5", DanisanTelNoTxt.Text.Length > 0 ? DanisanTelNoTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p6", DanisanMailTxt.Text.Length > 0 ? DanisanMailTxt.Text.ToUpper() : "-1");

                    if (DanisanKategoriCmb.Text.Length > 0) 
                    {
                        cmd.Parameters.AddWithValue("@p7", Convert.ToInt32(DanisanKategoriCmb.SelectedIndex + 1)); // SelectedIndex+1 şeklinde bir kullanım yapmamızın sebebi, veritabanı tarafından gelen ID'lerin 1'den, ComboBox tarafından gelen ID'lerin 0'dan başlamasıdır
                    }
                    

                    cmd.ExecuteNonQuery();
                    conn.Close();
                    MessageBox.Show("Ekleme İşlemi Başarılı");
                }

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
                using (SQLiteCommand cmd = new SQLiteCommand("Delete from TBLDANISANLAR where DANISANID = @p1", conn))
                {

                    cmd.Parameters.AddWithValue("@p1", DanisanIDTxt.Text.Length > 0 ? Convert.ToInt32(DanisanIDTxt.Text) : -1);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    MessageBox.Show("Silme İşlemi Başarılı");
                }
            }
            Listele();
            Temizle();
        }

        private void GuncelleBtn_Click(object sender, EventArgs e)
        {
            if (BosMu()) return;

            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = DanisanKategoriCmb.Text.Length > 0 ? new SQLiteCommand("Update TBLDANISANLAR set DANISANISIM=@p1, DANISANSOYAD=@p2, DANISANSEHIR=@p3, DANISANILCE=@p4, DANISANTEL=@p5, DANISANMAIL=@p6, DIYETKATEGORI=@p7 where DANISANID=@p8", conn) : new SQLiteCommand("Update TBLDANISANLAR set DANISANISIM=@p1, DANISANSOYAD=@p2, DANISANSEHIR=@p3, DANISANILCE=@p4, DANISANTEL=@p5, DANISANMAIL=@p6 where DANISANID=@p8", conn))
                {
                    cmd.Parameters.AddWithValue("@p1", DanisanIsimTxt.Text);
                    cmd.Parameters.AddWithValue("@p2", DanisanSoyisimTxt.Text);
                    cmd.Parameters.AddWithValue("@p3", DanisanSehirTxt.Text);
                    cmd.Parameters.AddWithValue("@p4", DanisanIlceTxt.Text);
                    cmd.Parameters.AddWithValue("@p5", DanisanTelNoTxt.Text);
                    cmd.Parameters.AddWithValue("@p6", DanisanMailTxt.Text);
                    cmd.Parameters.AddWithValue("@p8", Convert.ToInt32(DanisanIDTxt.Text));
                    
                    if (DanisanKategoriCmb.Text.Length > 0) 
                    {
                        cmd.Parameters.AddWithValue("@p7", Convert.ToInt32(DanisanKategoriCmb.SelectedIndex + 1)); // SelectedIndex+1 şeklinde bir kullanım yapmamızın sebebi, veritabanı tarafından gelen ID'lerin 1'den, ComboBox tarafından gelen ID'lerin 0'dan başlamasıdır
                    }

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Güncelleme İşlemi Başarılı");
                }

                conn.Close();              
            }
            Listele();
            Temizle();
        }

        private void DurumBtn_Click(object sender, EventArgs e)
        {
            if (BosMu()) return;

            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString)) 
            {
                conn.Open();

                using (SQLiteCommand cmd = new SQLiteCommand("UPDATE TBLDANISANLAR SET DURUM=@p1 WHERE DANISANID=@p2", conn))
                {
                    cmd.Parameters.AddWithValue("@p1", DurumLbl.Text=="Durum: Aktif" ? 0 : 1);
                    cmd.Parameters.AddWithValue("@p2", DanisanIDTxt.Text.Length > 0 ? Convert.ToInt32(DanisanIDTxt.Text) : -1);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Durum Güncelleme İşlemi Başarılı");
                }

                conn.Close ();
            }
            Listele();
            Temizle();
        }

        private void Danisanlar_FormClosed(object sender, FormClosedEventArgs e)
        {
            AnaMenu.danisanlar = null;
        }
    }
}
