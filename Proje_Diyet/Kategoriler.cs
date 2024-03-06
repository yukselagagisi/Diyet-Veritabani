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
    public partial class Kategoriler : Form
    {
        public Kategoriler()
        {
            InitializeComponent();
        }

        private void Listele() 
        {
            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString)) 
            {
                using (SQLiteCommand cmd = new SQLiteCommand("Select * from TBLDIYETKATEGORI",conn)) 
                {
                    using(SQLiteDataAdapter da = new SQLiteDataAdapter(cmd)) 
                    {
                        using(DataTable dt = new DataTable()) 
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
            return ((KategoriIDTxt.Text.Length == 0 && 
                    KategoriIsimTxt.Text.Length == 0) || string.IsNullOrWhiteSpace(KategoriIDTxt.Text));
        }

        private void Kategoriler_Load(object sender, EventArgs e)
        {
            Listele();
        }

        private void EkleBtn_Click(object sender, EventArgs e)
        {
            if (BosMu()) return;

            using(SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString)) 
            {
                conn.Open();

                using(SQLiteCommand cmd = new SQLiteCommand("Insert into TBLDIYETKATEGORI (KATEGORIAD) values (@p1)", conn)) 
                {
                    cmd.Parameters.AddWithValue("@p1",KategoriIsimTxt.Text.Length > 0 ? KategoriIsimTxt.Text.ToUpper() : "-1");
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
                MessageBox.Show("Başarıyla ekleme yapıldı");
            }

            Listele();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return; // Headerlara (Column başlıklarına) tıklayınca gelen hatayı kaldırmak için kullanıldı
            KategoriIDTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            KategoriIsimTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
        }

        private void GuncelleBtn_Click(object sender, EventArgs e)
        {
            if (BosMu()) return;

            using(SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString)) 
            {
                conn.Open();

                using(SQLiteCommand cmd = new SQLiteCommand("Update TBLDIYETKATEGORI set KATEGORIAD=@p1 where KATEGORIID=@p2", conn)) 
                {
                    cmd.Parameters.AddWithValue("@p1", KategoriIsimTxt.Text.Length > 0 ? KategoriIsimTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p2", KategoriIDTxt.Text.Length > 0 ? Convert.ToInt32(KategoriIDTxt.Text) : -1);
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
                MessageBox.Show("Güncelleme işlemi başarılı");
            }

            Listele();
        }

        private void Kategoriler_FormClosed(object sender, FormClosedEventArgs e)
        {
            AnaMenu.kategoriler = null;
        }
    }
}
