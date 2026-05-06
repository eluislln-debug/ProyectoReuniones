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
            txtBusquedaHora.Text = "";
            txtBusquedaHora.PlaceholderText = "Ingrese valor de busqueda...";

            cbFiltro.Items.Clear();
            cbFiltro.Items.Add("Fecha");
            cbFiltro.Items.Add("Mes");
            cbFiltro.Items.Add("Año");
            cbFiltro.Items.Add("Hora");
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
            dt.Columns.Add("Estado");      // ← nueva columna
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
                string nombresInvestigadores = "Ninguno";

                if (r.NumerosInvestigadores != null && r.NumerosInvestigadores.Count > 0)
                {
                    var investigadores = BD.Instancia.Usuarios
                        .Find(u => r.NumerosInvestigadores.Contains(u.NumeroUsuario))
                        .ToList();

                    nombresInvestigadores = string.Join(", ", investigadores.Select(u => u.NombreUsuario));
                }

                // Calcular estado automáticamente
                string estado = CalcularEstado(r);

                dt.Rows.Add(
                    r.Id.ToString(),
                    r.FechaReu.ToLocalTime().ToString("dd/MM/yyyy"),
                    r.HoraReu,
                    r.HoraFinReu,
                    estado,
                    r.MotivoReu,
                    nombresInvestigadores
                    
                );
            }

            // Colorear filas según estado
            foreach (DataGridViewRow fila in guna2DataGridView1.Rows)
            {
                string estado = fila.Cells["Estado"].Value?.ToString();

                switch (estado)
                {
                    case "Pendiente":
                        fila.DefaultCellStyle.BackColor = Color.FromArgb(219, 234, 254); // Azul claro
                        fila.DefaultCellStyle.ForeColor = Color.FromArgb(30, 64, 175);
                        break;
                    case "En curso":
                        fila.DefaultCellStyle.BackColor = Color.FromArgb(220, 252, 231); // Verde claro
                        fila.DefaultCellStyle.ForeColor = Color.FromArgb(22, 101, 52);
                        break;
                    case "Finalizada":
                        fila.DefaultCellStyle.BackColor = Color.FromArgb(229, 231, 235); // Gris claro
                        fila.DefaultCellStyle.ForeColor = Color.FromArgb(75, 85, 99);
                        break;
                    case "Reprogramada":
                        fila.DefaultCellStyle.BackColor = Color.FromArgb(254, 243, 199); // Amarillo claro
                        fila.DefaultCellStyle.ForeColor = Color.FromArgb(146, 64, 14);
                        break;
                    case "Cancelada":
                        fila.DefaultCellStyle.BackColor = Color.FromArgb(254, 226, 226); // Rojo claro
                        fila.DefaultCellStyle.ForeColor = Color.FromArgb(153, 27, 27);
                        break;
                }
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
            try
            {
                string filtro = cbFiltro.SelectedItem.ToString();

                // Traer reuniones del líder actual
                var queryBase = BD.Instancia.Reuniones
                    .Find(r => r.NumeroLider == _usuarioActual.NumeroUsuario)
                    .ToList();

                List<Reunion> resultados = new List<Reunion>();

                switch (filtro)
                {
                    case "Fecha":
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
                        if (string.IsNullOrWhiteSpace(txtBusquedaHora.Text))
                        {
                            MessageBox.Show("Ingresa una hora para buscar.",
                                "Campo vacío", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Validar formato HH:mm
                        if (!TimeSpan.TryParse(txtBusquedaHora.Text.Trim(), out TimeSpan horaValida))
                        {
                            MessageBox.Show(
                                "Formato de hora incorrecto.\nUsa el formato HH:mm, ejemplo: 08:00 o 14:30.",
                                "Formato inválido",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
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

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            ConsultarTodasLasReuniones();
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(idReunionSeleccionada))
    {
        MessageBox.Show("Por favor selecciona una reunión.", "Sin selección",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
    }

    // Buscar la reunión completa
    var filtro = Builders<Reunion>.Filter.Eq(r => r.Id, idReunionSeleccionada);
    Reunion reunion = BD.Instancia.Reuniones.Find(filtro).FirstOrDefault();

    if (reunion == null)
    {
        MessageBox.Show("No se encontró la reunión.", "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
    }

    // Calcular estado actual
    string estadoActual = CalcularEstado(reunion);

    // No permitir cancelar reuniones finalizadas o ya canceladas
    if (estadoActual == "Finalizada")
    {
        MessageBox.Show(
            "No puedes cancelar una reunión que ya finalizó.",
            "Cancelación no permitida",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
        return;
    }

    if (estadoActual == "Cancelada")
    {
        MessageBox.Show(
            "Esta reunión ya está cancelada.",
            "Cancelación no permitida",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
        return;
    }

    // Para pendientes y reprogramadas validar 1 hora de diferencia
    if (estadoActual == "Pendiente" || estadoActual == "Reprogramada")
    {
        DateTime fechaReu       = reunion.FechaReu.ToLocalTime().Date;
        TimeSpan horaInicio     = TimeSpan.Parse(reunion.HoraReu);
        DateTime inicioCompleto = fechaReu.Add(horaInicio);
        TimeSpan diferencia     = inicioCompleto - DateTime.Now;

        if (diferencia.TotalHours < 1)
        {
            MessageBox.Show(
                $"No puedes cancelar esta reunión.\n" +
                $"Debe haber al menos 1 hora de diferencia con la hora de inicio.\n" +
                $"La reunión comienza a las {reunion.HoraReu}.",
                "Cancelación no permitida",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }
    }

    // En curso tampoco se puede cancelar
    if (estadoActual == "En curso")
    {
        MessageBox.Show(
            "No puedes cancelar una reunión que está en curso.",
            "Cancelación no permitida",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
        return;
    }

    if (MessageBox.Show("¿Deseas cancelar esta reunión?", "Confirmar",
        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
    {
        try
        {
            var filtroCancelar = Builders<Reunion>.Filter.Eq(r => r.Id, idReunionSeleccionada);
            var actualizacion  = Builders<Reunion>.Update.Set(r => r.EstadoReu, "Cancelada");

            BD.Instancia.Reuniones.UpdateOne(filtroCancelar, actualizacion);

            MessageBox.Show("Reunión cancelada correctamente.", "Éxito",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            idReunionSeleccionada = "";
            ConsultarTodasLasReuniones();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(idReunionSeleccionada))
            {
                MessageBox.Show(
                    "Por favor selecciona una reunión para editar.",
                    "Sin selección",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var filtro = Builders<Reunion>.Filter.Eq(r => r.Id, idReunionSeleccionada);
            Reunion reunionEditar = BD.Instancia.Reuniones.Find(filtro).FirstOrDefault();

            if (reunionEditar == null)
            {
                MessageBox.Show("No se encontró la reunión.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Calcular estado actual de la reunión
            string estadoActual = CalcularEstado(reunionEditar);

            // No permitir editar reuniones finalizadas, canceladas o en curso
            if (estadoActual == "Finalizada")
            {
                MessageBox.Show(
                    "No puedes editar una reunión finalizada.",
                    "Edición no permitida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (estadoActual == "Cancelada")
            {
                MessageBox.Show(
                    "No puedes editar una reunión cancelada.",
                    "Edición no permitida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (estadoActual == "En curso")
            {
                MessageBox.Show(
                    "No puedes editar una reunión que está en curso.",
                    "Edición no permitida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Para reuniones pendientes validar que falte al menos 1 hora para el inicio
            if (estadoActual == "Pendiente" || estadoActual == "Reprogramada")
            {
                DateTime fechaReu = reunionEditar.FechaReu.ToLocalTime().Date;
                TimeSpan horaInicio = TimeSpan.Parse(reunionEditar.HoraReu);
                DateTime inicioCompleto = fechaReu.Add(horaInicio);

                // Diferencia entre ahora y la hora de inicio
                TimeSpan diferencia = inicioCompleto - DateTime.Now;

                if (diferencia.TotalHours < 1)
                {
                    MessageBox.Show(
                        $"No puedes editar esta reunión.\n" +
                        $"Debe haber al menos 1 hora de diferencia con la hora de inicio.\n" +
                        $"La reunión comienza a las {reunionEditar.HoraReu}.",
                        "Edición no permitida",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
            }

            // Todo válido — abrir FormLider en modo edición
            FormLider formEditar = new FormLider(_usuarioActual, reunionEditar);
            formEditar.ShowDialog();

            ConsultarTodasLasReuniones();
        }

        // ── Calcula el estado automático de una reunión ───────────────
        private string CalcularEstado(Reunion r)
        {
            // Si está cancelada respetar ese estado
            if (r.EstadoReu == "Cancelada") return "Cancelada";

            // Si está reprogramada respetar ese estado
            if (r.EstadoReu == "Reprogramada") return "Reprogramada";

            DateTime ahora = DateTime.Now;
            DateTime fechaReu = r.FechaReu.ToLocalTime().Date;

            TimeSpan horaInicio = TimeSpan.Parse(r.HoraReu);
            TimeSpan horaFin = TimeSpan.Parse(r.HoraFinReu);

            DateTime inicioCompleto = fechaReu.Add(horaInicio);
            DateTime finCompleto = fechaReu.Add(horaFin);

            if (ahora < inicioCompleto)
                return "Pendiente";
            else if (ahora >= inicioCompleto && ahora < finCompleto)
                return "En curso";
            else
                return "Finalizada";
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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
