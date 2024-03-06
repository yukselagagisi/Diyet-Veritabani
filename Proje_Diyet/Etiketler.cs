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
    public partial class Etiketler : Form
    {
        public Etiketler()
        {
            InitializeComponent();
        }

        private void Listele() 
        {
            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString)) 
            {
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM TBLETIKET", conn)) 
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
            return ((EtiketIDTxt.Text.Length == 0 &&
                EtiketAdTxt.Text.Length == 0) || string.IsNullOrWhiteSpace(EtiketIDTxt.Text));
        }

        private void Etiketler_Load(object sender, EventArgs e)
        {
            Listele();
        }

        private void EkleBtn_Click(object sender, EventArgs e)
        {

            if (BosMu()) return;

            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO TBLETIKET (ETIKETAD) VALUES (@p1)", conn)) 
                {
                    cmd.Parameters.AddWithValue("@p1", EtiketAdTxt.Text.Length > 0 ? EtiketAdTxt.Text.ToUpper() : "-1");
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
                MessageBox.Show("Başarıyla ekleme yapıldı");
            }

            Listele();
        }

        private void GuncelleBtn_Click(object sender, EventArgs e)
        {

            if (BosMu()) return;


            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString)) 
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("UPDATE TBLETIKET SET ETIKETAD=@p1 WHERE ETIKETID=@p2", conn)) 
                {
                    cmd.Parameters.AddWithValue("@p1", EtiketAdTxt.Text.Length > 0 ? EtiketAdTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p2", EtiketIDTxt.Text.Length > 0 ? Convert.ToInt32(EtiketIDTxt.Text) : -1);

                    cmd.ExecuteNonQuery();
                }
                conn.Close();
                MessageBox.Show("Güncelleme işlemi başarılı");
            }

            Listele();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return; // Headerlara (Column başlıklarına) tıklayınca gelen hatayı kaldırmak için kullanıldı
            EtiketIDTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            EtiketAdTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
        }

        private void Etiketler_FormClosed(object sender, FormClosedEventArgs e)
        {
            AnaMenu.etiketler = null;
        }
    }
}
