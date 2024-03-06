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
    public partial class Oneriler : Form
    {
        public Oneriler()
        {
            InitializeComponent();
        }

        private void Listele()
        {
            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT ONERIID, WPAD, WPSOYAD, ONERIAD, ONERI FROM TBLONERI LEFT JOIN TBLWP ON TBLONERI.WPID = TBLWP.WPID", conn))
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
            return (OneriIDTxt.Text.Length == 0 &&
                    IsımTxt.Text.Length == 0 &&
                    SoyadTxt.Text.Length == 0 &&
                    OneriAdTxt.Text.Length == 0 &&
                    OneriTxt.Text.Length == 0);
        }

        private void Temizle() 
        {
            OneriIDTxt.Clear();
            IsımTxt.Clear();
            SoyadTxt.Clear();
            OneriAdTxt.Clear();
            OneriTxt.Clear();
        }

        private void Oneriler_Load(object sender, EventArgs e)
        {
            Listele();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return; // Headerlara (Column başlıklarına) tıklayınca gelen hatayı kaldırmak için kullanıldı
            OneriIDTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            IsımTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            SoyadTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            OneriAdTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            OneriTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
        }

        private void TemizleBtn_Click(object sender, EventArgs e)
        {
            Temizle();
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
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT ONERIID, WPAD, WPSOYAD, ONERIAD, ONERI FROM TBLONERI LEFT JOIN TBLWP ON TBLONERI.WPID = TBLWP.WPID WHERE ONERIID like '%' || @p1 || '%' OR WPAD like '%' || @p2 || '%' OR WPSOYAD like '%' || @p3 || '%' OR ONERIAD like '%' || @p4 || '%'", conn))
                {
                    cmd.Parameters.AddWithValue("@p1", OneriIDTxt.Text.Length > 0 ? Convert.ToInt32(OneriIDTxt.Text) : -1);
                    cmd.Parameters.AddWithValue("@p2", IsımTxt.Text.Length > 0 ? IsımTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p3", SoyadTxt.Text.Length > 0 ? SoyadTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p4", OneriAdTxt.Text.Length > 0 ? OneriAdTxt.Text.ToUpper() : "-1");

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
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT WPID FROM TBLWP WHERE WPAD=@p1 AND WPSOYAD=@p2", conn)) 
                {
                    cmd.Parameters.AddWithValue("@p1", IsımTxt.Text.Length > 0 ? IsımTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p2", SoyadTxt.Text.Length > 0 ? SoyadTxt.Text.ToUpper() : "-1");

                    using (SQLiteDataReader dr  = cmd.ExecuteReader()) 
                    {
                        if (dr.HasRows) 
                        {
                            dr.Read(); // Bunu kapatmayı unutma

                            using (SQLiteCommand cmd2 = new SQLiteCommand("INSERT INTO TBLONERI (WPID, ONERIAD, ONERI) VALUES (@p3, @p4, @p5)", conn)) 
                            {
                                cmd2.Parameters.AddWithValue("@p3", dr[0].ToString());
                                cmd2.Parameters.AddWithValue("@p4", OneriAdTxt.Text.Length > 0 ? OneriAdTxt.Text.ToUpper() : "-1");
                                cmd2.Parameters.AddWithValue("@p5", OneriTxt.Text.Length > 0 ? OneriTxt.Text.ToUpper() : "-1");

                                cmd2.ExecuteReader();
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
                using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM TBLONERI WHERE ONERIID=@p1", conn)) 
                {
                    cmd.Parameters.AddWithValue("@p1", OneriIDTxt.Text.Length > 0 ? Convert.ToInt32(OneriIDTxt.Text) : -1);
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
            if (BosMu()) return;

            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString)) 
            {
                conn.Open();

                using (SQLiteCommand cmd = new SQLiteCommand("UPDATE TBLONERI SET ONERIAD=@p1, ONERI=@p2 WHERE ONERIID=@p3", conn))
                {
                    cmd.Parameters.AddWithValue("@p1", OneriAdTxt.Text.Length > 0 ? OneriAdTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p2", OneriTxt.Text.Length > 0 ? OneriTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p3", OneriIDTxt.Text.Length > 0 ? Convert.ToInt32(OneriIDTxt.Text) : -1);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Güncelleme işlemi başarılı");
                }

                conn.Close();
            }

            Listele();
            Temizle();
        }

        private void Oneriler_FormClosed(object sender, FormClosedEventArgs e)
        {
            AnaMenu.oneriler = null;
        }
    }
}
