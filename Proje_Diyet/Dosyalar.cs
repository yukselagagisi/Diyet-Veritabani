using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Proje_Diyet
{
    public partial class Dosyalar : Form
    {
        public Dosyalar()
        {
            InitializeComponent();
        }

        private void Temizle()
        {
            DosyaIDTxt.Clear();
            DanisanIsimTxt.Clear();
            DanisanSoyisimTxt.Clear();
            DosyaAdTxt.Clear();
            DosyaTxt.Clear();
        }

        private void Listele()
        {
            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand("Select DOSYAID, DANISANISIM, DANISANSOYAD, DOSYAAD, DOSYAYOLU from TBLDOSYALAR LEFT JOIN TBLDANISANLAR ON DANISAN = DANISANID ", conn))
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
            return (DosyaIDTxt.Text.Length == 0 &&
                   DanisanIsimTxt.Text.Length == 0 &&
                   DanisanSoyisimTxt.Text.Length == 0 &&
                   DosyaAdTxt.Text.Length == 0 &&
                   DosyaTxt.Text.Length == 0);
        }

        private void TemizleBtn_Click(object sender, EventArgs e)
        {
            Temizle();
        }

        private void ListeleBtn_Click(object sender, EventArgs e)
        {
            Listele();
        }

        private void Dosyalar_Load(object sender, EventArgs e)
        {
            Listele();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return; // Headerlara (Column başlıklarına) tıklayınca gelen hatayı kaldırmak için kullanıldı

            DosyaIDTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            DanisanIsimTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            DanisanSoyisimTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            DosyaAdTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            DosyaTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
        }

        private void DosyaAraBtn_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                DosyaTxt.Text = openFileDialog1.FileName;
            }
        }

        private void DosyaAcBtn_Click(object sender, EventArgs e)
        {
            if (DosyaTxt.Text.Length > 0 && System.IO.File.Exists(DosyaTxt.Text))
            {
                Process.Start("explorer.exe", DosyaTxt.Text);
            }
            else
            {
                MessageBox.Show("Dosya konumu girilmedi veya hatalı girildi.");
            }
        }

        private void AraBtn_Click(object sender, EventArgs e)
        {
            if (BosMu()) return;

            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT DOSYAID, DANISANISIM, DANISANSOYAD, DOSYAAD, DOSYAYOLU FROM TBLDOSYALAR LEFT JOIN TBLDANISANLAR ON DANISAN=DANISANID WHERE DOSYAID=@p1 OR DANISANISIM LIKE '%' || @p2 || '%' OR DANISANSOYAD LIKE '%' || @p3 || '%' OR DOSYAAD LIKE '%' || @p4 || '%'", conn))
                {
                    cmd.Parameters.AddWithValue("@p1", DosyaIDTxt.Text.Length > 0 ? Convert.ToInt32(DosyaIDTxt.Text) : -1);
                    cmd.Parameters.AddWithValue("@p2", DanisanIsimTxt.Text.Length > 0 ? DanisanIsimTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p3", DanisanSoyisimTxt.Text.Length > 0 ? DanisanSoyisimTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p4", DosyaAdTxt.Text.Length > 0 ? DosyaAdTxt.Text.ToUpper() : "-1");

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

                using (SQLiteCommand cmd = new SQLiteCommand("SELECT DANISANID FROM TBLDANISANLAR WHERE DANISANISIM=@p1 AND DANISANSOYAD =@p2", conn))
                {
                    cmd.Parameters.AddWithValue("@p1", DanisanIsimTxt.Text.Length > 0 ? DanisanIsimTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p2", DanisanSoyisimTxt.Text.Length > 0 ? DanisanSoyisimTxt.Text.ToUpper() : "-1");

                    SQLiteDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        string dosyaAdi = Path.GetFileName(DosyaTxt.Text);
                        string projeKonumu = System.IO.Directory.GetCurrentDirectory();

                        if (!File.Exists(projeKonumu + "\\files\\" + dosyaAdi))
                        {
                            dr.Read(); // Bunu kapatmayı unutma

                            using (SQLiteCommand cmd2 = new SQLiteCommand("INSERT INTO TBLDOSYALAR (DANISAN, DOSYAAD, DOSYAYOLU) VALUES (@p3, @p4, @p5) ", conn))
                            {
                                cmd2.Parameters.AddWithValue("@p3", dr[0].ToString());
                                cmd2.Parameters.AddWithValue("@p4", DosyaAdTxt.Text.Length > 0 ? DosyaAdTxt.Text.ToUpper() : "-1");
                                //cmd2.Parameters.AddWithValue("@p5", DosyaTxt.Text.Length > 0 ? DosyaTxt.Text : "-1");

                                System.IO.File.Copy(DosyaTxt.Text, projeKonumu + "\\files\\" + dosyaAdi);
                                cmd2.Parameters.AddWithValue("@p5", projeKonumu + "\\files\\" + dosyaAdi);

                                cmd2.ExecuteNonQuery();
                                MessageBox.Show("Başarıyla ekleme yapıldı");
                            }
                            dr.Close();
                        }
                        else 
                        {
                            MessageBox.Show("Girdiğiniz dosya zaten var!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Girilen İsim veya Soyisim Bulunamadı!");
                    }
                }
                conn.Close();
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
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT DANISANID FROM TBLDANISANLAR WHERE DANISANISIM=@p1 AND DANISANSOYAD=@p2", conn))
                {
                    cmd.Parameters.AddWithValue("@p1", DanisanIsimTxt.Text.Length > 0 ? DanisanIsimTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p2", DanisanSoyisimTxt.Text.Length > 0 ? DanisanSoyisimTxt.Text.ToUpper() : "-1");
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            dr.Read(); // Bunu kapatmayı unutma

                            using (SQLiteCommand cmd2 = new SQLiteCommand("UPDATE TBLDOSYALAR SET DANISAN=@p3, DOSYAAD=@p4, DOSYAYOLU=@p5 where DOSYAID = @p6", conn))
                            {
                                cmd2.Parameters.AddWithValue("@p3", dr[0].ToString());
                                cmd2.Parameters.AddWithValue("@p4", DosyaAdTxt.Text.Length > 0 ? DosyaAdTxt.Text.ToUpper() : "-1");
                                cmd2.Parameters.AddWithValue("@p5", DosyaTxt.Text.Length > 0 ? DosyaTxt.Text : "-1");
                                cmd2.Parameters.AddWithValue("@p6", DosyaIDTxt.Text.Length > 0 ? Convert.ToInt32(DosyaIDTxt.Text) : -1);
                                cmd2.ExecuteNonQuery();
                                dr.Close();
                                MessageBox.Show("Güncelleme işlemi başarılı");
                            }

                        }
                        else
                        {
                            MessageBox.Show("Girilen İsim veya Soyisim Bulunamadı");
                        }
                    }
                }
                conn.Close();
                Listele();
                Temizle();
            }
        }

        private void SilBtn_Click(object sender, EventArgs e)
        {
            if (BosMu()) return;
            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM TBLDOSYALAR WHERE DOSYAID=@p1", conn))
                {
                    cmd.Parameters.AddWithValue("@p1", DosyaIDTxt.Text.Length > 0 ? Convert.ToInt32(DosyaIDTxt.Text) : -1);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Silme işlemi başarılı");
                }
                conn.Close();
            }
            Listele();
            Temizle();
        }

        private void Dosyalar_FormClosed(object sender, FormClosedEventArgs e)
        {
            AnaMenu.dosyalar = null;
        }
    }
}
