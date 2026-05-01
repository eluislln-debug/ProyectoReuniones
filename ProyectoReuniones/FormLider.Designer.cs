namespace ProyectoReuniones
{
    partial class FormLider
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2PictureBox1 = new Guna.UI2.WinForms.Guna2PictureBox();
            this.lblBienvenida = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblSemillero = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.grpCalendario = new System.Windows.Forms.GroupBox();
            this.calendario = new System.Windows.Forms.MonthCalendar();
            this.grpHoras = new System.Windows.Forms.GroupBox();
            this.lstHoras = new System.Windows.Forms.ListBox();
            this.grpInvestigadores = new System.Windows.Forms.GroupBox();
            this.lstInvestigadores = new System.Windows.Forms.CheckedListBox();
            this.lblMotivo = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtMotivo = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnGuardar = new Guna.UI2.WinForms.Guna2Button();
            this.btnRegistrar = new Guna.UI2.WinForms.Guna2Button();
            this.btnVerReuniones = new Guna.UI2.WinForms.Guna2Button();
            this.btnCerrarSesion = new Guna.UI2.WinForms.Guna2Button();
            this.btnlimpiar = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.grpCalendario.SuspendLayout();
            this.grpHoras.SuspendLayout();
            this.grpInvestigadores.SuspendLayout();
            this.SuspendLayout();
            // 
            // guna2BorderlessForm1
            // 
            this.guna2BorderlessForm1.ContainerControl = this;
            this.guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.guna2Panel1.Controls.Add(this.btnCerrarSesion);
            this.guna2Panel1.Controls.Add(this.btnVerReuniones);
            this.guna2Panel1.Controls.Add(this.btnRegistrar);
            this.guna2Panel1.Controls.Add(this.guna2PictureBox1);
            this.guna2Panel1.Location = new System.Drawing.Point(0, 0);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.Size = new System.Drawing.Size(211, 678);
            this.guna2Panel1.TabIndex = 0;
            // 
            // guna2PictureBox1
            // 
            this.guna2PictureBox1.Image = global::ProyectoReuniones.Properties.Resources.Proyecto_imagen;
            this.guna2PictureBox1.ImageRotate = 0F;
            this.guna2PictureBox1.Location = new System.Drawing.Point(0, 0);
            this.guna2PictureBox1.Name = "guna2PictureBox1";
            this.guna2PictureBox1.Size = new System.Drawing.Size(211, 151);
            this.guna2PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.guna2PictureBox1.TabIndex = 0;
            this.guna2PictureBox1.TabStop = false;
            // 
            // lblBienvenida
            // 
            this.lblBienvenida.AutoSize = false;
            this.lblBienvenida.BackColor = System.Drawing.Color.Transparent;
            this.lblBienvenida.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBienvenida.Location = new System.Drawing.Point(217, 12);
            this.lblBienvenida.Name = "lblBienvenida";
            this.lblBienvenida.Size = new System.Drawing.Size(523, 36);
            this.lblBienvenida.TabIndex = 1;
            this.lblBienvenida.Text = "Bienvenido,";
            // 
            // lblSemillero
            // 
            this.lblSemillero.BackColor = System.Drawing.Color.Transparent;
            this.lblSemillero.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSemillero.Location = new System.Drawing.Point(2, 13);
            this.lblSemillero.Name = "lblSemillero";
            this.lblSemillero.Size = new System.Drawing.Size(111, 28);
            this.lblSemillero.TabIndex = 2;
            this.lblSemillero.Text = "Semillero: ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnlimpiar);
            this.groupBox1.Controls.Add(this.btnGuardar);
            this.groupBox1.Controls.Add(this.txtMotivo);
            this.groupBox1.Controls.Add(this.lblMotivo);
            this.groupBox1.Controls.Add(this.grpInvestigadores);
            this.groupBox1.Controls.Add(this.grpHoras);
            this.groupBox1.Controls.Add(this.grpCalendario);
            this.groupBox1.Controls.Add(this.lblSemillero);
            this.groupBox1.Location = new System.Drawing.Point(217, 54);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(862, 610);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // grpCalendario
            // 
            this.grpCalendario.Controls.Add(this.calendario);
            this.grpCalendario.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpCalendario.Location = new System.Drawing.Point(6, 51);
            this.grpCalendario.Name = "grpCalendario";
            this.grpCalendario.Size = new System.Drawing.Size(270, 288);
            this.grpCalendario.TabIndex = 3;
            this.grpCalendario.TabStop = false;
            this.grpCalendario.Text = "Selecciona una fecha";
            // 
            // calendario
            // 
            this.calendario.Location = new System.Drawing.Point(6, 18);
            this.calendario.Name = "calendario";
            this.calendario.TabIndex = 0;
            this.calendario.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.calendario_DateChanged);
            // 
            // grpHoras
            // 
            this.grpHoras.Controls.Add(this.lstHoras);
            this.grpHoras.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpHoras.Location = new System.Drawing.Point(282, 51);
            this.grpHoras.Name = "grpHoras";
            this.grpHoras.Size = new System.Drawing.Size(270, 288);
            this.grpHoras.TabIndex = 4;
            this.grpHoras.TabStop = false;
            this.grpHoras.Text = "Horas disponibles";
            // 
            // lstHoras
            // 
            this.lstHoras.FormattingEnabled = true;
            this.lstHoras.ItemHeight = 20;
            this.lstHoras.Location = new System.Drawing.Point(6, 34);
            this.lstHoras.Name = "lstHoras";
            this.lstHoras.Size = new System.Drawing.Size(256, 244);
            this.lstHoras.TabIndex = 0;
            // 
            // grpInvestigadores
            // 
            this.grpInvestigadores.Controls.Add(this.lstInvestigadores);
            this.grpInvestigadores.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpInvestigadores.Location = new System.Drawing.Point(558, 51);
            this.grpInvestigadores.Name = "grpInvestigadores";
            this.grpInvestigadores.Size = new System.Drawing.Size(298, 288);
            this.grpInvestigadores.TabIndex = 5;
            this.grpInvestigadores.TabStop = false;
            this.grpInvestigadores.Text = "Seleccionar investigadores";
            // 
            // lstInvestigadores
            // 
            this.lstInvestigadores.FormattingEnabled = true;
            this.lstInvestigadores.Location = new System.Drawing.Point(6, 25);
            this.lstInvestigadores.Name = "lstInvestigadores";
            this.lstInvestigadores.Size = new System.Drawing.Size(286, 257);
            this.lstInvestigadores.TabIndex = 0;
            // 
            // lblMotivo
            // 
            this.lblMotivo.BackColor = System.Drawing.Color.Transparent;
            this.lblMotivo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMotivo.Location = new System.Drawing.Point(12, 345);
            this.lblMotivo.Name = "lblMotivo";
            this.lblMotivo.Size = new System.Drawing.Size(169, 22);
            this.lblMotivo.TabIndex = 6;
            this.lblMotivo.Text = "Motivo de la reunión:";
            // 
            // txtMotivo
            // 
            this.txtMotivo.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtMotivo.DefaultText = "";
            this.txtMotivo.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtMotivo.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtMotivo.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtMotivo.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtMotivo.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtMotivo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtMotivo.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtMotivo.Location = new System.Drawing.Point(12, 374);
            this.txtMotivo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtMotivo.Multiline = true;
            this.txtMotivo.Name = "txtMotivo";
            this.txtMotivo.PlaceholderText = "";
            this.txtMotivo.SelectedText = "";
            this.txtMotivo.Size = new System.Drawing.Size(356, 215);
            this.txtMotivo.TabIndex = 7;
            // 
            // btnGuardar
            // 
            this.btnGuardar.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnGuardar.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnGuardar.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnGuardar.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnGuardar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnGuardar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(425, 544);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(180, 45);
            this.btnGuardar.TabIndex = 8;
            this.btnGuardar.Text = "Guardar Reunión";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnRegistrar
            // 
            this.btnRegistrar.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnRegistrar.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnRegistrar.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnRegistrar.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnRegistrar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnRegistrar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnRegistrar.ForeColor = System.Drawing.Color.White;
            this.btnRegistrar.Location = new System.Drawing.Point(0, 215);
            this.btnRegistrar.Name = "btnRegistrar";
            this.btnRegistrar.Size = new System.Drawing.Size(211, 45);
            this.btnRegistrar.TabIndex = 9;
            this.btnRegistrar.Text = "Registrar Reunión";
            // 
            // btnVerReuniones
            // 
            this.btnVerReuniones.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnVerReuniones.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnVerReuniones.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnVerReuniones.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnVerReuniones.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnVerReuniones.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnVerReuniones.ForeColor = System.Drawing.Color.White;
            this.btnVerReuniones.Location = new System.Drawing.Point(0, 376);
            this.btnVerReuniones.Name = "btnVerReuniones";
            this.btnVerReuniones.Size = new System.Drawing.Size(211, 45);
            this.btnVerReuniones.TabIndex = 10;
            this.btnVerReuniones.Text = "Ver mis reuniones";
            // 
            // btnCerrarSesion
            // 
            this.btnCerrarSesion.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnCerrarSesion.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnCerrarSesion.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnCerrarSesion.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnCerrarSesion.FillColor = System.Drawing.Color.Red;
            this.btnCerrarSesion.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCerrarSesion.ForeColor = System.Drawing.Color.White;
            this.btnCerrarSesion.Location = new System.Drawing.Point(0, 570);
            this.btnCerrarSesion.Name = "btnCerrarSesion";
            this.btnCerrarSesion.Size = new System.Drawing.Size(211, 45);
            this.btnCerrarSesion.TabIndex = 11;
            this.btnCerrarSesion.Text = "Cerrar Sesion";
            this.btnCerrarSesion.Click += new System.EventHandler(this.btnCerrarSesion_Click);
            // 
            // btnlimpiar
            // 
            this.btnlimpiar.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnlimpiar.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnlimpiar.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnlimpiar.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnlimpiar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnlimpiar.ForeColor = System.Drawing.Color.White;
            this.btnlimpiar.Location = new System.Drawing.Point(648, 544);
            this.btnlimpiar.Name = "btnlimpiar";
            this.btnlimpiar.Size = new System.Drawing.Size(180, 45);
            this.btnlimpiar.TabIndex = 9;
            this.btnlimpiar.Text = "Limpiar campos";
            this.btnlimpiar.Click += new System.EventHandler(this.btnlimpiar_Click);
            // 
            // FormLider
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1091, 675);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblBienvenida);
            this.Controls.Add(this.guna2Panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLider";
            this.Text = "FormLider";
            this.Load += new System.EventHandler(this.FormLider_Load);
            this.guna2Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpCalendario.ResumeLayout(false);
            this.grpHoras.ResumeLayout(false);
            this.grpInvestigadores.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox grpCalendario;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblSemillero;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblBienvenida;
        private Guna.UI2.WinForms.Guna2PictureBox guna2PictureBox1;
        private System.Windows.Forms.GroupBox grpInvestigadores;
        private System.Windows.Forms.CheckedListBox lstInvestigadores;
        private System.Windows.Forms.GroupBox grpHoras;
        private System.Windows.Forms.ListBox lstHoras;
        private System.Windows.Forms.MonthCalendar calendario;
        private Guna.UI2.WinForms.Guna2TextBox txtMotivo;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblMotivo;
        private Guna.UI2.WinForms.Guna2Button btnGuardar;
        private Guna.UI2.WinForms.Guna2Button btnCerrarSesion;
        private Guna.UI2.WinForms.Guna2Button btnVerReuniones;
        private Guna.UI2.WinForms.Guna2Button btnRegistrar;
        private Guna.UI2.WinForms.Guna2Button btnlimpiar;
    }
}