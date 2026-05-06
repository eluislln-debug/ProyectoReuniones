using MongoDB.Driver;
using ProyectoReuniones.Conexion;
using ProyectoReuniones.Modelos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Bson;



namespace ProyectoReuniones
{
    public partial class FormInvestigador : Form
    {
        // Variable que guarda el usuario que inició sesión
        private Usuario _usuarioActual;


        public FormInvestigador(Usuario usuario)
        {
            InitializeComponent();
            // Guardamos el usuario para usarlo en toda la ventana
            _usuarioActual = usuario;
            txtBusquedaHora.PlaceholderText = "Ingrese valor de busqueda...";
            lblBienbenido.Text = "Bienvenido, " + _usuarioActual.NombreUsuario;

            // Configurar el ComboBox de filtros
            cbFiltro.Items.Clear();
            cbFiltro.Items.Add("Fecha");
            cbFiltro.Items.Add("Mes");
            cbFiltro.Items.Add("Año");
            cbFiltro.Items.Add("Hora");
            cbFiltro.SelectedIndex = 0;

            ConfigurarEstructuraGrid();
        }

        private void ConsultarTodasLasReuniones()
        {
            try
            {
                // Buscamos reuniones donde el NumeroUsuario del investigador esté en la lista NumerosInvestigadores
                var reuniones = BD.Instancia.Reuniones
                    .Find(r => r.NumerosInvestigadores.Contains(_usuarioActual.NumeroUsuario))
                    .ToList();

                CargarGrid(reuniones);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar reuniones: " + ex.Message);
            }
        }

        private void CargarGrid(List<Reunion> listaReuniones)
        {
            DataTable dt = (DataTable)guna2DataGridView1.DataSource;
            dt.Rows.Clear();

            foreach (var r in listaReuniones)
            {
                // Buscar nombre del líder
                var lider = BD.Instancia.Usuarios
                    .Find(u => u.NumeroUsuario == r.NumeroLider)
                    .FirstOrDefault();

                string nombreLider = lider != null ? lider.NombreUsuario : "No asignado";

                // Calcular estado automáticamente
                string estado = CalcularEstado(r);

                dt.Rows.Add(
                    r.FechaReu.ToLocalTime().ToString("dd/MM/yyyy"),
                    r.HoraReu,
                    r.HoraFinReu,
                    estado,
                    r.MotivoReu,
                    nombreLider
                );
            }

            // Colorear filas según estado
            foreach (DataGridViewRow fila in guna2DataGridView1.Rows)
            {
                string estado = fila.Cells["Estado"].Value?.ToString();

                switch (estado)
                {
                    case "Pendiente":
                        fila.DefaultCellStyle.BackColor = Color.FromArgb(219, 234, 254);
                        fila.DefaultCellStyle.ForeColor = Color.FromArgb(30, 64, 175);
                        break;
                    case "En curso":
                        fila.DefaultCellStyle.BackColor = Color.FromArgb(220, 252, 231);
                        fila.DefaultCellStyle.ForeColor = Color.FromArgb(22, 101, 52);
                        break;
                    case "Finalizada":
                        fila.DefaultCellStyle.BackColor = Color.FromArgb(229, 231, 235);
                        fila.DefaultCellStyle.ForeColor = Color.FromArgb(75, 85, 99);
                        break;
                    case "Reprogramada":
                        fila.DefaultCellStyle.BackColor = Color.FromArgb(254, 243, 199);
                        fila.DefaultCellStyle.ForeColor = Color.FromArgb(146, 64, 14);
                        break;
                    case "Cancelada":
                        fila.DefaultCellStyle.BackColor = Color.FromArgb(254, 226, 226);
                        fila.DefaultCellStyle.ForeColor = Color.FromArgb(153, 27, 27);
                        break;
                }
            }

            if (listaReuniones.Count == 0)
            {
                MessageBox.Show("No se encontraron reuniones.",
                    "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ── Calcula el estado automático de una reunión ───────────────
        private string CalcularEstado(Reunion r)
        {
            if (r.EstadoReu == "Cancelada") return "Cancelada";
            if (r.EstadoReu == "Reprogramada") return "Reprogramada";

            DateTime ahora = DateTime.Now;
            DateTime fechaReu = r.FechaReu.ToLocalTime().Date;

            TimeSpan horaInicio = TimeSpan.Parse(r.HoraReu);
            TimeSpan horaFin = TimeSpan.Parse(r.HoraFinReu);

            DateTime inicioCompleto = fechaReu.Add(horaInicio);
            DateTime finCompleto = fechaReu.Add(horaFin);

            if (ahora < inicioCompleto) return "Pendiente";
            if (ahora < finCompleto) return "En curso";
            return "Finalizada";
        }

        private void ConfigurarEstructuraGrid()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Fecha");
            dt.Columns.Add("Hora Inicio");
            dt.Columns.Add("Hora Fin");
            dt.Columns.Add("Estado");
            dt.Columns.Add("Motivo");
            dt.Columns.Add("Líder");

            guna2DataGridView1.DataSource = dt;

            guna2DataGridView1.ColumnHeadersVisible = true;
            guna2DataGridView1.ColumnHeadersHeight = 40;
            guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.AllowUserToAddRows = false;
            guna2DataGridView1.Refresh();
        }
        private void guna2HtmlLabel3_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            try
            {
                string filtro = cbFiltro.SelectedItem.ToString();

                // Para investigador buscar donde aparece en la lista de participantes
                var queryBase = BD.Instancia.Reuniones
                    .Find(r => r.NumerosInvestigadores.Contains(_usuarioActual.NumeroUsuario))
                    .ToList();

                List<Reunion> resultados = new List<Reunion>();

                switch (filtro)
                {
                    case "Fecha":
                        // Buscar por fecha seleccionada en el calendario
                        DateTime fechaBusqueda = calBusqueda.SelectionStart.Date;
                        resultados = queryBase
                            .Where(r => r.FechaReu.ToLocalTime().Date == fechaBusqueda)
                            .ToList();
                        break;

                    case "Mes":
                        int mesBusqueda = int.Parse(cbValorFiltro.SelectedItem.ToString().Split('-')[0].Trim());
                        resultados = queryBase
                            .Where(r => r.FechaReu.ToLocalTime().Month == mesBusqueda)
                            .ToList();
                        break;

                    case "Año":
                        int anioBusqueda = int.Parse(cbValorFiltro.SelectedItem.ToString());
                        resultados = queryBase
                            .Where(r => r.FechaReu.ToLocalTime().Year == anioBusqueda)
                            .ToList();
                        break;

                    case "Hora":
                        // Validar que ingresó algo
                        if (string.IsNullOrWhiteSpace(txtBusquedaHora.Text))
                        {
                            MessageBox.Show("Ingresa una hora para buscar.",
                                "Campo vacío", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        string horaBusqueda = txtBusquedaHora.Text.Trim();
                        resultados = queryBase
                            .Where(r => r.HoraReu != null && r.HoraReu.Contains(horaBusqueda))
                            .ToList();
                        break;
                }

                CargarGrid(resultados);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            ConsultarTodasLasReuniones();
        }

        private void guna2DataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show(
                "¿Desea cerrar sesión?",
                "Cerrar sesión",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (r == DialogResult.Yes)
            {
                Form1 login = new Form1(); 
                login.Show();

                this.Hide(); 
            }
        }

        private void cbFiltro_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Ocultar todos los controles de búsqueda
            calBusqueda.Visible = false;
            txtBusquedaHora.Visible = false;
            cbValorFiltro.Visible = false;

            string filtro = cbFiltro.SelectedItem.ToString();

            switch (filtro)
            {
                case "Fecha":
                    // Mostrar calendario
                    calBusqueda.Visible = true;
                    calBusqueda.MinDate = DateTime.Today;
                    calBusqueda.SelectionStart = DateTime.Today;
                    break;

                case "Mes":
                    // Mostrar combobox con meses desde el actual
                    cbValorFiltro.Visible = true;
                    cbValorFiltro.Items.Clear();

                    for (int mes = DateTime.Today.Month; mes <= 12; mes++)
                    {
                        // Nombre del mes en español
                        string nombreMes = new DateTime(DateTime.Today.Year, mes, 1)
                            .ToString("MMMM", new System.Globalization.CultureInfo("es-CO"));
                        cbValorFiltro.Items.Add($"{mes} - {nombreMes}");
                    }
                    cbValorFiltro.SelectedIndex = 0;
                    break;

                case "Año":
                    // Mostrar combobox con años desde el actual
                    cbValorFiltro.Visible = true;
                    cbValorFiltro.Items.Clear();

                    for (int anio = DateTime.Today.Year; anio <= DateTime.Today.Year + 5; anio++)
                        cbValorFiltro.Items.Add(anio.ToString());

                    cbValorFiltro.SelectedIndex = 0;
                    break;

                case "Hora":
                    // Mostrar textbox para escribir la hora
                    txtBusquedaHora.Visible = true;
                    txtBusquedaHora.Text = "";
                    txtBusquedaHora.PlaceholderText = "Ej: 08:00";
                    break;
            }
        }
    }
}
