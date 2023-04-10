
namespace Admin_RickyShop
{
    partial class FormPrincipal
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPrincipal));
            this.panelMenu = new System.Windows.Forms.Panel();
            this.iBtnExit = new FontAwesome.Sharp.IconButton();
            this.iBtnLogs = new FontAwesome.Sharp.IconButton();
            this.iBtnProdutos = new FontAwesome.Sharp.IconButton();
            this.iBtnUtilizadores = new FontAwesome.Sharp.IconButton();
            this.iBtnPedidos = new FontAwesome.Sharp.IconButton();
            this.iBtnDashbord = new FontAwesome.Sharp.IconButton();
            this.panelHome = new System.Windows.Forms.Panel();
            this.lblHome = new System.Windows.Forms.Label();
            this.panelForm = new System.Windows.Forms.Panel();
            this.panelMenu.SuspendLayout();
            this.panelHome.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMenu
            // 
            this.panelMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(162)))), ((int)(((byte)(219)))), ((int)(((byte)(59)))));
            this.panelMenu.Controls.Add(this.iBtnExit);
            this.panelMenu.Controls.Add(this.iBtnLogs);
            this.panelMenu.Controls.Add(this.iBtnProdutos);
            this.panelMenu.Controls.Add(this.iBtnUtilizadores);
            this.panelMenu.Controls.Add(this.iBtnPedidos);
            this.panelMenu.Controls.Add(this.iBtnDashbord);
            this.panelMenu.Controls.Add(this.panelHome);
            resources.ApplyResources(this.panelMenu, "panelMenu");
            this.panelMenu.Name = "panelMenu";
            // 
            // iBtnExit
            // 
            resources.ApplyResources(this.iBtnExit, "iBtnExit");
            this.iBtnExit.FlatAppearance.BorderSize = 0;
            this.iBtnExit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.iBtnExit.ForeColor = System.Drawing.Color.White;
            this.iBtnExit.IconChar = FontAwesome.Sharp.IconChar.PowerOff;
            this.iBtnExit.IconColor = System.Drawing.Color.White;
            this.iBtnExit.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iBtnExit.IconSize = 40;
            this.iBtnExit.Name = "iBtnExit";
            this.iBtnExit.UseVisualStyleBackColor = true;
            this.iBtnExit.Click += new System.EventHandler(this.iBtnExit_Click);
            // 
            // iBtnLogs
            // 
            resources.ApplyResources(this.iBtnLogs, "iBtnLogs");
            this.iBtnLogs.FlatAppearance.BorderSize = 0;
            this.iBtnLogs.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LawnGreen;
            this.iBtnLogs.ForeColor = System.Drawing.Color.White;
            this.iBtnLogs.IconChar = FontAwesome.Sharp.IconChar.ToggleOff;
            this.iBtnLogs.IconColor = System.Drawing.Color.White;
            this.iBtnLogs.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iBtnLogs.IconSize = 40;
            this.iBtnLogs.Name = "iBtnLogs";
            this.iBtnLogs.UseVisualStyleBackColor = true;
            this.iBtnLogs.Click += new System.EventHandler(this.iBtnLogs_Click);
            // 
            // iBtnProdutos
            // 
            resources.ApplyResources(this.iBtnProdutos, "iBtnProdutos");
            this.iBtnProdutos.FlatAppearance.BorderSize = 0;
            this.iBtnProdutos.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LimeGreen;
            this.iBtnProdutos.ForeColor = System.Drawing.Color.White;
            this.iBtnProdutos.IconChar = FontAwesome.Sharp.IconChar.ProductHunt;
            this.iBtnProdutos.IconColor = System.Drawing.Color.White;
            this.iBtnProdutos.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iBtnProdutos.IconSize = 40;
            this.iBtnProdutos.Name = "iBtnProdutos";
            this.iBtnProdutos.UseVisualStyleBackColor = true;
            this.iBtnProdutos.Click += new System.EventHandler(this.iBtnProdutos_Click);
            // 
            // iBtnUtilizadores
            // 
            resources.ApplyResources(this.iBtnUtilizadores, "iBtnUtilizadores");
            this.iBtnUtilizadores.FlatAppearance.BorderSize = 0;
            this.iBtnUtilizadores.FlatAppearance.MouseOverBackColor = System.Drawing.Color.ForestGreen;
            this.iBtnUtilizadores.ForeColor = System.Drawing.Color.White;
            this.iBtnUtilizadores.IconChar = FontAwesome.Sharp.IconChar.UserAlt;
            this.iBtnUtilizadores.IconColor = System.Drawing.Color.White;
            this.iBtnUtilizadores.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iBtnUtilizadores.IconSize = 40;
            this.iBtnUtilizadores.Name = "iBtnUtilizadores";
            this.iBtnUtilizadores.UseVisualStyleBackColor = true;
            this.iBtnUtilizadores.Click += new System.EventHandler(this.iBtnUtilizadores_Click);
            // 
            // iBtnPedidos
            // 
            resources.ApplyResources(this.iBtnPedidos, "iBtnPedidos");
            this.iBtnPedidos.FlatAppearance.BorderSize = 0;
            this.iBtnPedidos.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Green;
            this.iBtnPedidos.ForeColor = System.Drawing.Color.White;
            this.iBtnPedidos.IconChar = FontAwesome.Sharp.IconChar.ShoppingCart;
            this.iBtnPedidos.IconColor = System.Drawing.Color.White;
            this.iBtnPedidos.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iBtnPedidos.IconSize = 40;
            this.iBtnPedidos.Name = "iBtnPedidos";
            this.iBtnPedidos.UseVisualStyleBackColor = true;
            this.iBtnPedidos.Click += new System.EventHandler(this.iBtnPedidos_Click);
            // 
            // iBtnDashbord
            // 
            resources.ApplyResources(this.iBtnDashbord, "iBtnDashbord");
            this.iBtnDashbord.FlatAppearance.BorderSize = 0;
            this.iBtnDashbord.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGreen;
            this.iBtnDashbord.ForeColor = System.Drawing.Color.White;
            this.iBtnDashbord.IconChar = FontAwesome.Sharp.IconChar.Dashboard;
            this.iBtnDashbord.IconColor = System.Drawing.Color.White;
            this.iBtnDashbord.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iBtnDashbord.IconSize = 40;
            this.iBtnDashbord.Name = "iBtnDashbord";
            this.iBtnDashbord.UseVisualStyleBackColor = true;
            this.iBtnDashbord.Click += new System.EventHandler(this.iBtnDashbord_Click);
            // 
            // panelHome
            // 
            this.panelHome.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(159)))), ((int)(((byte)(9)))));
            this.panelHome.Controls.Add(this.lblHome);
            resources.ApplyResources(this.panelHome, "panelHome");
            this.panelHome.Name = "panelHome";
            // 
            // lblHome
            // 
            this.lblHome.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(162)))), ((int)(((byte)(219)))), ((int)(((byte)(59)))));
            resources.ApplyResources(this.lblHome, "lblHome");
            this.lblHome.Name = "lblHome";
            this.lblHome.Click += new System.EventHandler(this.lblHome_Click);
            // 
            // panelForm
            // 
            this.panelForm.BackColor = System.Drawing.Color.OliveDrab;
            resources.ApplyResources(this.panelForm, "panelForm");
            this.panelForm.ForeColor = System.Drawing.Color.Black;
            this.panelForm.Name = "panelForm";
            this.panelForm.Paint += new System.Windows.Forms.PaintEventHandler(this.panelForm_Paint);
            // 
            // FormPrincipal
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.GreenYellow;
            this.ControlBox = false;
            this.Controls.Add(this.panelForm);
            this.Controls.Add(this.panelMenu);
            this.ForeColor = System.Drawing.Color.White;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPrincipal";
            this.ShowIcon = false;
            this.Load += new System.EventHandler(this.FormPrincipal_Load);
            this.panelMenu.ResumeLayout(false);
            this.panelHome.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Panel panelHome;
        private FontAwesome.Sharp.IconButton iBtnDashbord;
        private FontAwesome.Sharp.IconButton iBtnLogs;
        private FontAwesome.Sharp.IconButton iBtnProdutos;
        private FontAwesome.Sharp.IconButton iBtnUtilizadores;
        private FontAwesome.Sharp.IconButton iBtnPedidos;
        private System.Windows.Forms.Panel panelForm;
        private FontAwesome.Sharp.IconButton iBtnExit;
        private System.Windows.Forms.Label lblHome;
    }
}

