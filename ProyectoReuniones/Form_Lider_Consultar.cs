using MongoDB.Bson;
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
using MongoDB.Bson.Serialization.Attributes;

namespace ProyectoReuniones
{
   
    public partial class Form_Lider_Consultar : Form
    {
        private Usuario _usuarioActual;
        private string idReunionSeleccionada = "";
        private string motivoReunionSeleccionada = "";
       
       
        public Form_Lider_Consultar(Usuario usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;
            lblBienbenido.Text = "Bienvenido, " + _usuarioActual.NombreUsuario;
            // 2. Configuración de controles
            txtBusqueda.Text = "";
            txtBusqueda.PlaceholderText = "Ingrese valor de busqueda...";

            cbFiltro.Items.Clear();
            cbFiltro.Items.Add("Fecha");
            cbFiltro.Items.Add("Hora");
            cbFiltro.Items.Add("Investigador");
            cbFiltro.SelectedIndex = 0;

            // 3. Preparar el Grid
            ConfigurarEstructuraGrid();
        }

        private void ConfigurarEstructuraGrid()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID_Internal"); // Columna oculta para MongoDB
            dt.Columns.Add("Fecha");
            dt.Columns.Add("Hora Inicio");
            dt.Columns.Add("Hora Fin");    // ← columna nueva
            dt.Columns.Add("Motivo");
            dt.Columns.Add("Participantes");

            guna2DataGridView1.DataSource = dt;

            // Ajustes visuales
            guna2DataGridView1.Columns["ID_Internal"].Visible = false;
            guna2DataGridView1.ColumnHeadersVisible = true;
            guna2DataGridView1.ColumnHeadersHeight = 40;
            guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            guna2DataGridView1.MultiSelect = false;
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.Refresh();
        }

        private void CargarGrid(List<Reunion> listaReuniones)
        {
            DataTable dt = (DataTable)guna2DataGridView1.DataSource;
            dt.Rows.Clear();

            foreach (var r in listaReuniones)
            {
                // Buscar los nombres de los investigadores en la colección Usuarios
                string nombresInvestigadores = "Ninguno";

                if (r.NumerosInvestigadores != null && r.NumerosInvestigadores.Count > 0)
                {
                    // Traer los usuarios cuyos números estén en la lista de investigadores
                    var investigadores = BD.Instancia.Usuarios
                        .Find(u => r.NumerosInvestigadores.Contains(u.NumeroUsuario))
                        .ToList();

                    // Unir los nombres separados por coma
                    nombresInvestigadores = string.Join(", ", investigadores.Select(u => u.NombreUsuario));
                }

                dt.Rows.Add(
                    r.Id.ToString(),
                    r.FechaReu.ToString("dd/MM/yyyy"),
                    r.HoraReu,
                    r.HoraFinReu,
                    r.MotivoReu,
                    nombresInvestigadores  // Ahora muestra los nombres reales
                );
            }

            if (listaReuniones.Count == 0)
            {
                MessageBox.Show("No se encontraron registros.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ConsultarTodasLasReuniones()
        {
            try
            {
                // El Líder solo ve las reuniones donde su NumeroUsuario coincide con NumeroLider
                var reuniones = BD.Instancia.Reuniones
                    .Find(r => r.NumeroLider == _usuarioActual.NumeroUsuario)
                    .ToList();

                CargarGrid(reuniones);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar: " + ex.Message);
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string valor = txtBusqueda.Text.Trim();
            if (string.IsNullOrWhiteSpace(valor))
            {
                MessageBox.Show("Ingrese un término de búsqueda.");
                return;
            }

            try
            {
                string filtro = cbFiltro.SelectedItem.ToString();

                var queryBase = BD.Instancia.Reuniones
                    .Find(r => r.NumeroLider == _usuarioActual.NumeroUsuario)
                    .ToList();

                List<Reunion> resultados = new List<Reunion>();

                switch (filtro)
                {
                    case "Fecha":
                        if (DateTime.TryParse(valor, out DateTime f))
                            resultados = queryBase
                                .Where(r => r.FechaReu.Date == f.Date)
                                .ToList();
                        break;

                    case "Hora":
                        resultados = queryBase
                            .Where(r => r.HoraReu != null && r.HoraReu.Contains(valor))
                            .ToList();
                        break;

                    case "Investigador":

                        // Buscar usuarios que coincidan con el nombre
                        var investigadores = BD.Instancia.Usuarios
                            .Find(u => u.NombreUsuario.ToLower().Contains(valor.ToLower()))
                            .ToList();

                        var idsInvestigadores = investigadores
                            .Select(u => u.NumeroUsuario)
                            .ToList();

                        // Filtrar reuniones donde participen esos investigadores
                        resultados = queryBase
                            .Where(r => r.NumerosInvestigadores != null &&
                                        r.NumerosInvestigadores.Any(id => idsInvestigadores.Contains(id)))
                            .ToList();

                        break;
                }

                CargarGrid(resultados);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            ConsultarTodasLasReuniones();
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            // 1. Validamos selección
            if (string.IsNullOrEmpty(idReunionSeleccionada))
            {
                MessageBox.Show("Por favor, selecciona una reunión.");
                return;
            }

            if (MessageBox.Show($"¿Deseas cancelar la reunión?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    // EL CAMBIO ESTÁ AQUÍ: 
                    // Como tu clase 'Reunion' usa 'string Id', el filtro debe comparar contra el string.
                    // No necesitas hacer el .Parse() a ObjectId para el filtro de LINQ.

                    var filtro = Builders<Reunion>.Filter.Eq(r => r.Id, idReunionSeleccionada);

                    // Ejecutamos la eliminación
                    var resultado = BD.Instancia.Reuniones.DeleteOne(filtro);

                    if (resultado.DeletedCount > 0)
                    {
                        MessageBox.Show("Reunión cancelada correctamente.", "Éxito");
                        idReunionSeleccionada = "";
                        ConsultarTodasLasReuniones();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar: " + ex.Message);
                }
            }
        }

        private void guna2DataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            
                if (e.RowIndex >= 0)
                {
                    // Obtenemos los datos de la fila seleccionada
                    // "ID_Internal" es la columna que definimos como oculta en ConfigurarEstructuraGrid
                    idReunionSeleccionada = guna2DataGridView1.Rows[e.RowIndex].Cells["ID_Internal"].Value.ToString();
                    motivoReunionSeleccionada = guna2DataGridView1.Rows[e.RowIndex].Cells["Motivo"].Value.ToString();

                    // Opcional: Podrías cambiar el color del botón de cancelar para indicar que ya se puede usar
                    btnCancelar.FillColor = System.Drawing.Color.FromArgb(255, 82, 82); // Un rojo suave
                }
            
        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnVerReuniones_Click(object sender, EventArgs e)
        {

        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            FormLider fl = new FormLider(_usuarioActual);   
            fl.Show();
            this.Hide();
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show(
                "¿Estás seguro que deseas cerrar sesión?",
                "Cerrar sesión",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                Form1 formLogin = new Form1();
                this.Hide();
                formLogin.Show();
            }
        }
    }
}
