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
            txtBusqueda.PlaceholderText = "Ingrese valor de busqueda...";
            lblBienbenido.Text = "Bienvenido, " + _usuarioActual.NombreUsuario;

            // Configurar el ComboBox de filtros
            cbFiltro.Items.Clear();
            cbFiltro.Items.Add("Fecha");
            cbFiltro.Items.Add("Hora");
            cbFiltro.Items.Add("Investigador"); // 👈 nuevo
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
            DataTable dt = new DataTable();
            dt.Columns.Add("Fecha");
            dt.Columns.Add("Hora");
            dt.Columns.Add("Motivo");
            dt.Columns.Add("Líder"); // Cambiado de ID Líder a Líder

            foreach (var r in listaReuniones)
            {
                // Buscamos el nombre del líder en la base de datos usando el NumeroLider de la reunión
                var lider = BD.Instancia.Usuarios
                    .Find(u => u.NumeroUsuario == r.NumeroLider)
                    .FirstOrDefault();

                string nombreLider = lider != null ? lider.NombreUsuario : "No asignado";

                dt.Rows.Add(
                    r.FechaReu.ToString("dd/MM/yyyy"),
                    r.HoraReu,
                    r.MotivoReu,
                    nombreLider
                );
            }

            guna2DataGridView1.DataSource = dt;

            // Validación visual: si no hay resultados, avisar al usuario
            if (listaReuniones.Count == 0)
            {
                MessageBox.Show("No se encontraron reuniones con los criterios seleccionados.",
                                "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void ConfigurarEstructuraGrid()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Fecha");
            dt.Columns.Add("Hora");
            dt.Columns.Add("Motivo");
            dt.Columns.Add("Líder"); 

            guna2DataGridView1.DataSource = dt;

            // Estética del Grid
            guna2DataGridView1.ColumnHeadersVisible = true;
            guna2DataGridView1.ColumnHeadersHeight = 40;
            guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Evitar que el usuario edite las celdas directamente
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.AllowUserToAddRows = false;

            guna2DataGridView1.Refresh();
        }
        private void guna2HtmlLabel3_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string valor = txtBusqueda.Text.Trim();

            if (string.IsNullOrWhiteSpace(valor) || valor == "Ingrese valor de busqueda...")
            {
                MessageBox.Show("Por favor, ingrese un término de búsqueda válido.",
                                "Campo Requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBusqueda.Focus();
                return;
            }

            try
            {
                string filtro = cbFiltro.SelectedItem.ToString();

                // Solo reuniones donde participa el investigador actual
                var query = BD.Instancia.Reuniones
                    .Find(r => r.NumerosInvestigadores.Contains(_usuarioActual.NumeroUsuario));

                var todasMisReuniones = query.ToList();
                List<Reunion> resultados = new List<Reunion>();

                switch (filtro)
                {
                    case "Fecha":
                        if (DateTime.TryParse(valor, out DateTime fechaBusqueda))
                        {
                            resultados = todasMisReuniones
                                .Where(r => r.FechaReu.Date == fechaBusqueda.Date)
                                .ToList();
                        }
                        else
                        {
                            MessageBox.Show("Formato de fecha incorrecto. Use DD/MM/AAAA");
                            return;
                        }
                        break;

                    case "Hora":
                        resultados = todasMisReuniones
                            .Where(r => r.HoraReu != null && r.HoraReu.Contains(valor))
                            .ToList();
                        break;

                    case "Investigador":

                        // 🔥 Buscar usuarios por nombre
                        var usuarios = BD.Instancia.Usuarios
                            .Find(u => u.NombreUsuario.ToLower().Contains(valor.ToLower()))
                            .ToList();

                        List<int> numerosUsuarios = new List<int>();

                        foreach (var u in usuarios)
                        {
                            numerosUsuarios.Add(u.NumeroUsuario);
                        }

                        // 🔥 Filtrar reuniones donde esté ese investigador
                        resultados = todasMisReuniones
                            .Where(r => r.NumerosInvestigadores != null &&
                                        r.NumerosInvestigadores.Any(n => numerosUsuarios.Contains(n)))
                            .ToList();
                        break;

                    default:
                        resultados = todasMisReuniones;
                        break;
                }

                CargarGrid(resultados);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
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
    }
}
