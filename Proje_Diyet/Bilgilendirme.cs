using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proje_Diyet
{
    public partial class Bilgilendirme : Form
    {
        public Bilgilendirme()
        {
            InitializeComponent();
        }

        private void Bilgilendirme_FormClosed(object sender, FormClosedEventArgs e)
        {
            AnaMenu.bilgilendirme = null;
        }
    }
}
