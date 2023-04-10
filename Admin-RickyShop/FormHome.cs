using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Admin_RickyShop
{
    public partial class FormHome : Form
    {
        public FormHome()
        {
            InitializeComponent();
        }

       private void FormDashbord_Load_1(object sender, EventArgs e)
        {

        }

        private void FormHome_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.lblRelogio.Text = string.Format(@"{0:hh\\:mm\\ss}", DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString());
        }
    }
}
