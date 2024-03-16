using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Proje_Diyet
{
    public partial class AnaMenu : Form
    {
        public AnaMenu()
        {
            InitializeComponent();
        }

        // PENCEREYİ SÜRÜKLEME KODLARI GİRİŞ

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        // PENCEREYİ SÜRÜKLEME KODLARI ÇIKIŞ

        private void Listele()
        {
            foreach (var series in chart1.Series)
            {
                series.Points.Clear();
            }

            foreach (var series in chart2.Series)
            {
                series.Points.Clear();
            }

            using (SQLiteConnection conn = new SQLiteConnection(Sorgular.connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT DANISANISIM, DANISANSOYAD, MAX(TARIH) AS SON_ODEME_TARIHI, TUTAR FROM TBLHAREKET LEFT JOIN TBLDANISANLAR ON DANISAN = DANISANID WHERE DURUM=1 GROUP BY DANISAN ORDER BY SON_ODEME_TARIHI DESC;", conn))
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

                conn.Open();
                using (SQLiteCommand cmd2 = new SQLiteCommand("select KATEGORIAD, COUNT(*) from TBLDANISANLAR LEFT JOIN TBLDIYETKATEGORI ON DIYETKATEGORI = KATEGORIID GROUP BY KATEGORIAD", conn))
                {
                    using (SQLiteDataReader dr = cmd2.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            chart1.Series["Kategoriler"].Points.AddXY(dr[0], dr[1]);
                        }
                    }
                }

                using (SQLiteCommand cmd3 = new SQLiteCommand("select ETIKETAD, COUNT(*) FROM TBLWP LEFT JOIN TBLETIKET ON ETIKET = ETIKETID GROUP BY ETIKETAD", conn))
                {
                    using (SQLiteDataReader dr2 = cmd3.ExecuteReader())
                    {
                        while (dr2.Read())
                        {
                            chart2.Series["Etiketler"].Points.AddXY(dr2[0], dr2[1]);
                        }
                    }
                }

                conn.Close();
            }
        }

        private void AnaMenu_Load(object sender, EventArgs e)
        {
            Sorgular sorgular = new Sorgular();
            sorgular.CreateTables(); //dependency injection 
            Listele();
        }

        public static Danisanlar danisanlar = null;
        private void DanisanlarBtn_Click(object sender, EventArgs e)
        {
            if (danisanlar == null)
            {
                danisanlar = new Danisanlar();
            }
            danisanlar.Show();
        }

        public static Hareketler hareketler = null;
        private void HareketlerBtn_Click(object sender, EventArgs e)
        {
            if (hareketler == null)
            {
                hareketler = new Hareketler();
            }
            hareketler.Show();
        }

        public static Kategoriler kategoriler = null;
        private void KategorilerBtn_Click(object sender, EventArgs e)
        {
            if (kategoriler == null)
            {
                kategoriler = new Kategoriler();
            }
            kategoriler.Show();
        }

        public static Bilgilendirme bilgilendirme = null;
        private void BilgilendirmePb_Click(object sender, EventArgs e)
        {
            if (bilgilendirme == null)
            {
                bilgilendirme = new Bilgilendirme();
            }
            bilgilendirme.Show();
        }

        public static Dosyalar dosyalar = null;
        private void DosyalarBtn_Click(object sender, EventArgs e)
        {
            if (dosyalar == null)
            {
                dosyalar = new Dosyalar();
            }
            dosyalar.Show();
        }

        public static Whatsapp wp = null;
        private void WPKisilerBtn_Click(object sender, EventArgs e)
        {
            if (wp == null)
            {
                wp = new Whatsapp();
            }
            wp.Show();
        }

        public static Etiketler etiketler = null;
        private void EtiketlerBtn_Click(object sender, EventArgs e)
        {
            if (etiketler == null)
            {
                etiketler = new Etiketler();
            }
            etiketler.Show();
        }

        public static Urunler urunler = null;
        private void UrunlerBtn_Click(object sender, EventArgs e)
        {
            if (urunler == null)
            {
                urunler = new Urunler();
            }
            urunler.Show();
        }

        public static Oneriler oneriler = null;
        private void OnerilerBtn_Click(object sender, EventArgs e)
        {
            if (oneriler == null)
            {
                oneriler = new Oneriler();
            }
            oneriler.Show();
        }

        private void SuruklePnl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void CikisBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void YenileBtn_Click(object sender, EventArgs e)
        {
            Listele();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return; // Headerlara (Column başlıklarına) tıklayınca gelen hatayı kaldırmak için kullanıldı

            TarihDtp.Text = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells[2].Value).AddMonths(1).ToString();
            //TutarTxt.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();

            TutarTxt.Text = $"{dataGridView1.Rows[e.RowIndex].Cells[3].Value:n0}" + " ₺";

        }
    }
}
