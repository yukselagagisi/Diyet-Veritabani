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
    public partial class Hareketler : Form
    {
        public Hareketler()
        {
            InitializeComponent();
        }

        private void Listele()
        {
            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand("Select HAREKETID, DANISANISIM, DANISANSOYAD, TUTAR, TARIH from TBLHAREKET LEFT JOIN TBLDANISANLAR ON DANISAN = DANISANID", conn))
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
            return (
                HarekeIDTxt.Text.Length == 0 &&
                DanisanIsimTxt.Text.Length == 0 &&
                DanisanSoyisimTxt.Text.Length == 0 &&
                TutarTxt.Text.Length == 0 &&
                Convert.ToDateTime(TarihDtp.Text).ToShortDateString() == DateTimePicker.MinimumDateTime.ToShortDateString());
        }

        private void Temizle()
        {
            HarekeIDTxt.Clear();
            DanisanIsimTxt.Clear();
            DanisanSoyisimTxt.Clear();
            TutarTxt.Clear();
            TarihDtp.Value = DateTimePicker.MinimumDateTime;
        }

        private void KazancGuncelle()
        {
            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("Select sum(TUTAR) from TBLHAREKET", conn))
                {
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            dr.Read();
                            ToplamKazancLbl.Text = "Toplam Kazanç: " + (dr[0] == System.DBNull.Value ? "0" : $"{dr[0]:n0}") + " ₺";
                            dr.Close();
                        }
                    }
                }

                using (SQLiteCommand cmd = new SQLiteCommand("Select sum(TUTAR) from TBLHAREKET where TARIH like '%' || @p1 || '%' ", conn))
                {
                    cmd.Parameters.AddWithValue("@p1", DateTime.Now.Month.ToString() + "." + DateTime.Now.Year.ToString());
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            dr.Read();
                            AylıkKazancLbl.Text = "Aylık Kazanç: " + (dr[0] == System.DBNull.Value ? "0" : $"{dr[0]:n0}") + " ₺";
                            dr.Close();
                        }
                    }
                }

                conn.Close();
            }
        }

        private void ListeleBtn_Click(object sender, EventArgs e)
        {
            Listele();
        }

        private void Hareketler_Load(object sender, EventArgs e)
        {
            Listele();
            TarihDtp.Value = DateTimePicker.MinimumDateTime; // Tarihi default olarak minimum time'a ayarlıyoruz çünkü böyle yapmazsak bugünün tarihini alıyor ve arama yaparken yaptığımız aramaya ek olarak bugünün verilerini de getiriyor

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return; // Headerlara (Column başlıklarına) tıklayınca gelen hatayı kaldırmak için kullanıldı

            HarekeIDTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            DanisanIsimTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            DanisanSoyisimTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            TutarTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            TarihDtp.Text = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells[4].Value).ToString();
        }

        private void AraBtn_Click(object sender, EventArgs e)
        {
            if (BosMu()) return;

            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand("Select HAREKETID, DANISANISIM, DANISANSOYAD, TUTAR, TARIH FROM TBLHAREKET LEFT JOIN TBLDANISANLAR ON DANISAN = DANISANID WHERE HAREKETID=@p1 or DANISANISIM like '%' || @p2 || '%' or DANISANSOYAD like '%' || @p3 || '%' or TUTAR=@p4 or TARIH=@p5", conn))
                {
                    cmd.Parameters.AddWithValue("@p1", HarekeIDTxt.Text.Length > 0 ? Convert.ToInt32(HarekeIDTxt.Text) : -1);
                    cmd.Parameters.AddWithValue("@p2", DanisanIsimTxt.Text.Length > 0 ? DanisanIsimTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p3", DanisanSoyisimTxt.Text.Length > 0 ? DanisanSoyisimTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p4", TutarTxt.Text.Length > 0 ? Convert.ToDecimal(TutarTxt.Text) : -1);
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

        private void TemizleBtn_Click(object sender, EventArgs e)
        {
            Temizle();
        }

        private void EkleBtn_Click(object sender, EventArgs e)
        {
            if (BosMu()) return;

            //var tarihParcalari = Convert.ToDateTime(TarihDtp.Text.ToString()).ToShortDateString().Split('.');
            //string yagFormat = string.Format("{0}-{1}-{2}",
            //  tarihParcalari[2], tarihParcalari[1], tarihParcalari[0]);
            //MessageBox.Show(yagFormat);

            //string[] tarihParcalari2 = DateTime.Now.ToShortDateString().Split('.');
            //string yagFormat2 = string.Format("{0}-{1}-{2}",
            //  tarihParcalari2[2], tarihParcalari2[1], tarihParcalari2[0]);


            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString))
            {
                conn.Open();

                using (SQLiteCommand cmd = new SQLiteCommand("Select DANISANID from TBLDANISANLAR where DANISANISIM=@p5 and DANISANSOYAD=@p6", conn))
                {


                    cmd.Parameters.AddWithValue("@p5", DanisanIsimTxt.Text.Length > 0 ? DanisanIsimTxt.Text.ToUpper() : "-1");
                    cmd.Parameters.AddWithValue("@p6", DanisanSoyisimTxt.Text.Length > 0 ? DanisanSoyisimTxt.Text.ToUpper() : "-1");
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {

                        if (dr.HasRows)
                        {
                            dr.Read(); // Bunu kapatmayı unutma
                            using (SQLiteCommand cmd2 = new SQLiteCommand("INSERT INTO TBLHAREKET (DANISAN, TUTAR, TARIH) VALUES (@p2, @p3, @p4)", conn))
                            {

                                cmd2.Parameters.AddWithValue("@p2", dr[0].ToString());
                                cmd2.Parameters.AddWithValue("@p3", TutarTxt.Text.Length > 0 ? Convert.ToDecimal(TutarTxt.Text) : -1);
                                cmd2.Parameters.AddWithValue("@p4", TarihDtp.Text.Length > 0 ? Convert.ToDateTime (TarihDtp.Text).ToShortDateString() : DateTime.Now.ToShortDateString());
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
                Listele();
                Temizle();
                KazancGuncelle();
            }

        }

        private void SilBtn_Click(object sender, EventArgs e)
        {
            if (BosMu()) return;
            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("Delete from TBLHAREKET WHERE HAREKETID = @p1", conn))
                {
                    cmd.Parameters.AddWithValue("@p1", HarekeIDTxt.Text.Length > 0 ? Convert.ToInt32(HarekeIDTxt.Text) : -1);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Silme işlemi başarılı");
                    Temizle();
                }
                conn.Close();
            }
            Listele();
            Temizle();
            KazancGuncelle();
        }

        private void GuncelleBtn_Click(object sender, EventArgs e)
        {
            if (BosMu()) return;

            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString))
            {
                conn.Open();
                
                using (SQLiteCommand cmd = new SQLiteCommand("Update TBLHAREKET SET TUTAR=@p1, TARIH=@p2 where HAREKETID=@p3", conn))
                {

                    cmd.Parameters.AddWithValue("@p1", TutarTxt.Text.Length > 0 ? Convert.ToDecimal(TutarTxt.Text) : -1);
                    cmd.Parameters.AddWithValue("@p2", TarihDtp.Text.Length > 0 ? Convert.ToDateTime(TarihDtp.Text).ToShortDateString() : DateTime.Now.ToShortDateString());
                    cmd.Parameters.AddWithValue("@p3", HarekeIDTxt.Text.Length > 0 ? Convert.ToInt32(HarekeIDTxt.Text) : -1);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Güncelleme işlemi başarılı");
                }

                conn.Close();
                Listele();
                Temizle();
                KazancGuncelle();
            }
        }

        private void KazancGosterBtn_Click(object sender, EventArgs e)
        {
            KazancGuncelle();

            if (!ToplamKazancLbl.Visible)
            {
                ToplamKazancLbl.Visible = true;
                AylıkKazancLbl.Visible = true;

            }
            else
            {
                ToplamKazancLbl.Visible = false;
                AylıkKazancLbl.Visible = false;
            }

        }

        private void Hareketler_FormClosed(object sender, FormClosedEventArgs e)
        {
            AnaMenu.hareketler = null;
        }
    }
}
