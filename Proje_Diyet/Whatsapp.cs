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
    public partial class Whatsapp : Form
    {
        public Whatsapp()
        {
            InitializeComponent();
        }

        private void Listele() 
        {
            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT WPID, WPAD, WPSOYAD, TELNO, ETIKETAD FROM TBLWP LEFT JOIN TBLETIKET ON TBLWP.ETIKET=TBLETIKET.ETIKETID  ", conn))
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
            WPIDTxt.Clear();
            AdTxt.Clear();
            SoyadTxt.Clear();
            TelNoTxt.Clear();
            EtiketCmb.ResetText();
        }

        private bool BosMu()
        {
            return (WPIDTxt.Text.Length == 0 &&
            AdTxt.Text.Length == 0 &&
            SoyadTxt.Text.Length == 0 &&
            TelNoTxt.Text == "(   )    -");// MaskedTextBox içerisinde 10 karakterli boş bir telefon numarası template'i taşıyor
        }

        private void ListeleBtn_Click(object sender, EventArgs e)
        {
            Listele();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return; // Headerlara (Column başlıklarına) tıklayınca gelen hatayı kaldırmak için kullanıldı

            WPIDTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            AdTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            SoyadTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            TelNoTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            EtiketCmb.Text = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
        }

        private void TemizleBtn_Click(object sender, EventArgs e)
        {
            Temizle();
        }

        private void Whatsapp_Load(object sender, EventArgs e)
        {
            Listele();

            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString)) 
            {
                conn.Open();

                using (SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM TBLETIKET", conn)) 
                {
                    using (SQLiteDataReader dr = cmd.ExecuteReader()) 
                    {
                        while (dr.Read()) 
                        {
                            EtiketCmb.Items.Add(dr["ETIKETAD"]);
                        }
                    }
                }
                conn.Close();
            }

        }

        private void AraBtn_Click(object sender, EventArgs e)
        {
            if (BosMu()) return;

            using(SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString)) 
            {
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT WPID, WPAD, WPSOYAD, TELNO, TBLETIKET.ETIKETAD FROM TBLWP LEFT JOIN TBLETIKET ON TBLWP.ETIKET = TBLETIKET.ETIKETID WHERE WPID like '%' || @p1 || '%' OR WPAD like '%' || @p2 || '%' OR WPSOYAD like '%' || @p3 || '%' OR TELNO like '%' || @p4 || '%' OR ETIKETAD like '%' || @p5 || '%' OR", conn))
                {
                    cmd.Parameters.AddWithValue("@p1", WPIDTxt.Text.Length > 0 ? Convert.ToInt32(WPIDTxt.Text) : -1);
                    cmd.Parameters.AddWithValue("@p2", AdTxt.Text.Length > 0 ? AdTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p3", SoyadTxt.Text.Length > 0 ? AdTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p4", TelNoTxt.Text.Length > 0 ? AdTxt.Text : "-1");
                    cmd.Parameters.AddWithValue("@p5", EtiketCmb.Text.Length > 0 ? EtiketCmb.Text.ToUpper() : "-1");

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
                using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO TBLWP (WPAD, WPSOYAD, TELNO, ETIKET) VALUES (@p1, @p2, @p3, @p4)", conn)) 
                {
                    cmd.Parameters.AddWithValue("@p1", AdTxt.Text.Length > 0 ? AdTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p2", SoyadTxt.Text.Length > 0 ? SoyadTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p3", TelNoTxt.Text.Length > 0 ? TelNoTxt.Text.ToUpper() : "-1");

                    if (EtiketCmb.Text.Length > 0)
                    {
                        cmd.Parameters.AddWithValue("@p4", Convert.ToInt32(EtiketCmb.SelectedIndex + 1)); // SelectedIndex+1 şeklinde bir kullanım yapmamızın sebebi, veritabanı tarafından gelen ID'lerin 1'den, ComboBox tarafından gelen ID'lerin 0'dan başlamasıdır
                    }

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Ekleme İşlemi Başarılı");
                }
                conn.Close();
                Listele();
            }
        }

        private void SilBtn_Click(object sender, EventArgs e)
        {
            if (BosMu()) return;

            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString)) 
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM TBLWP WHERE WPID=@p1", conn))
                {
                    cmd.Parameters.AddWithValue("@p1", WPIDTxt.Text.Length > 0 ? Convert.ToInt32(WPIDTxt.Text) : -1);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Silme İşlemi Başarılı");
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
                using (SQLiteCommand cmd = new SQLiteCommand("UPDATE TBLWP SET WPAD=@p1, WPSOYAD=@p2, TELNO=@p3, ETIKET=@p5 WHERE WPID=@p4", conn)) 
                {
                    cmd.Parameters.AddWithValue("@p1", AdTxt.Text.Length > 0 ? AdTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p2", SoyadTxt.Text.Length > 0 ? SoyadTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p3", TelNoTxt.Text.Length > 0 ? TelNoTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p4", WPIDTxt.Text.Length > 0 ? Convert.ToInt32(WPIDTxt.Text) : -1);

                    if (EtiketCmb.Text.Length > 0)
                    {
                        cmd.Parameters.AddWithValue("@p5", Convert.ToInt32(EtiketCmb.SelectedIndex + 1)); // SelectedIndex+1 şeklinde bir kullanım yapmamızın sebebi, veritabanı tarafından gelen ID'lerin 1'den, ComboBox tarafından gelen ID'lerin 0'dan başlamasıdır
                    }

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Güncelleme İşlemi Başarılı");
                }
                conn.Close();
            }
            Listele();
            Temizle();
        }

        private void Whatsapp_FormClosed(object sender, FormClosedEventArgs e)
        {
            AnaMenu.wp = null;
        }
    }
}
