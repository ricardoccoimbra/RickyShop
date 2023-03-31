using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Admin_RickyShop
{
    public partial class FormPrincipal : Form
    {
        private Form formAtivo;
        public FormPrincipal()
        {
            InitializeComponent();
        }

        public void FormShow(Form form)
        {
            ActiveFormClose();
            formAtivo = form;
            form.TopLevel = false;
            panelForm.Controls.Add(form);
            panelForm.Controls.Add(form);
            form.BringToFront();
            form.Show();
        }

        private void ActiveFormClose()
        {
            if (formAtivo != null)
                formAtivo.Close();
        }

        private void AtiveButton(Button formAtivo)
        {
            foreach (Control item in panelMenu.Controls)
            {
                if(item.Name != "panelHome")
                item.BackColor = Color.FromArgb(162, 219, 59);
            }

            formAtivo.BackColor = Color.FromArgb(104, 159, 9);
        }

        private void iBtnProdutos_Click(object sender, EventArgs e)
        {
            AtiveButton(iBtnProdutos);
            FormShow(new FormProdutos());
        }

        private void iBtnDashbord_Click(object sender, EventArgs e)
        {
            AtiveButton(iBtnDashbord);
            FormShow(new FormDashbord());
        }

        private void iBtnLogs_Click(object sender, EventArgs e)
        {
            AtiveButton(iBtnLogs);
            FormShow(new FormLogs());
        }

        private void iBtnUtilizadores_Click(object sender, EventArgs e)
        {
            AtiveButton(iBtnUtilizadores);
            FormShow(new FormUtilizadores());
        }

        private void iBtnPedidos_Click(object sender, EventArgs e)
        {
            AtiveButton(iBtnPedidos);
            FormShow(new FormPedidos());
        }
    }
}
