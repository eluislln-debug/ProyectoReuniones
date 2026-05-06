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
using MongoDB.Driver;

namespace ProyectoReuniones
{
    public partial class FormLider : Form
    {
        // Variable que guarda el usuario que inició sesión
        private Usuario _usuarioActual;
       
        private bool _cargandoHoras = false;

        // ── Reunión que se está editando (null = modo registro normal) ──
        private Reunion _reunionEditando = null;

        public FormLider(Usuario usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;

            // --- Configuración General del Form ---
            this.BackColor = Color.FromArgb(237, 242, 247);
            this.BackColor = SystemColors.ActiveCaption;

            // --- Estilizar Cards (Contenedores) ---
            ConfigurarCard(cardFecha);
            ConfigurarCard(cardHoras);
            ConfigurarCard(cardInvestigadores);
            ConfigurarCard(cardMotivo);

            // --- Estilizar Títulos ---
            ConfigurarLabelTitulo(lblTitutloFecha);
            ConfigurarLabelTitulo(lblHorasDisponibles);
            ConfigurarLabelTitulo(lblInvestigador);
            ConfigurarLabelTitulo(lblMotivo);

            // --- 1. DISEÑO: Calendario (Guna2DateTimePicker) ---
            // Si usas el MonthCalendar estándar, lo estilizaremos mínimamente 
            // o puedes cambiarlo por Guna2DateTimePicker en el diseñador
            calendario.BackColor = Color.White;
            calendario.TitleBackColor = Color.FromArgb(59, 130, 246); // Azul moderno

            // --- 3. DISEÑO: Lista de Investigadores (lstInvestigadores) ---
            lstInvestigadores.BorderStyle = BorderStyle.None;
            lstInvestigadores.BackColor = Color.White;
            lstInvestigadores.Font = new Font("Segoe UI", 10F);
            lstInvestigadores.CheckOnClick = true;
            lstInvestigadores.ForeColor = Color.FromArgb(71, 85, 105);

           
            txtMotivo.PlaceholderText = "Describe el objetivo de la reunión...";
            txtMotivo.BorderRadius = 12;
            txtMotivo.FillColor = Color.FromArgb(249, 250, 251);
            txtMotivo.BorderColor = Color.FromArgb(220, 220, 220);
            txtMotivo.FocusedState.BorderColor = Color.FromArgb(59, 130, 246);

   
            lblBienvenida.Font = new Font("Segoe UI Semibold", 16F);
            lblBienvenida.ForeColor = Color.FromArgb(15, 23, 42);
            lblSemillero.Font = new Font("Segoe UI", 10F);
            lblSemillero.ForeColor = Color.FromArgb(100, 116, 139);

            txtMotivo.FillColor = Color.White; // Cambiamos el gris claro por blanco puro
            txtMotivo.ForeColor = Color.FromArgb(15, 23, 42); // Un color de texto casi negro para máximo contraste
            txtMotivo.BorderColor = Color.FromArgb(180, 180, 180); // Un gris un poco más oscuro para definir el borde
            txtMotivo.BorderThickness = 1; // Aseguramos que el borde sea visible
            txtMotivo.Font = new Font("Segoe UI", 11F); // Subimos un poco el tamaño para mejor legibilidad

            // Color del Placeholder (el texto de fondo) para que no parezca deshabilitado
            txtMotivo.PlaceholderForeColor = Color.FromArgb(150, 150, 150);

            // Efecto al hacer clic (esto le da mucha vida)
            txtMotivo.FocusedState.BorderColor = SystemColors.ActiveCaption;
            txtMotivo.FocusedState.FillColor = Color.White;
        }

        // ── Constructor para modo edición ────────────────────────────
        public FormLider(Usuario usuario, Reunion reunionEditar) : this(usuario)
        {
            // Aquí sí existe reunionEditar porque es parámetro de este constructor
            _reunionEditando = reunionEditar;
        }

        // Método auxiliar para no repetir código en los títulos de las secciones
        // Cambiamos 'Label' por 'Guna.UI2.WinForms.Guna2HtmlLabel'
        private void ConfigurarLabelTitulo(Guna.UI2.WinForms.Guna2HtmlLabel lbl)
        {
            lbl.Font = new Font("Segoe UI Semibold", 11F);
            lbl.ForeColor = Color.FromArgb(30, 41, 59);
        }

        private void ConfigurarCard(Guna.UI2.WinForms.Guna2ShadowPanel card)
        {
            card.FillColor = Color.White;
            card.Radius = 15; // Un poco menos redondo para verse más limpio
            card.ShadowColor = Color.FromArgb(200, 210, 230); // Sombra más clara/azulada
            card.ShadowDepth = 8;
            card.ShadowShift = 5;
        }

        // Bandera para evitar validaciones al cargar
        private bool _iniciandoForm = false;

        private void FormLider_Load(object sender, EventArgs e)
        {
            lblBienvenida.Text = "Bienvenido, " + _usuarioActual.NombreUsuario;

            var semillero = BD.Instancia.Semilleros
                .Find(s => s.NumeroSemillero == _usuarioActual.NumeroSemillero)
                .FirstOrDefault();

            lblSemillero.Text = semillero != null
                ? "Semillero: " + semillero.NombreSemillero
                : "Semillero: No encontrado";

            _iniciandoForm = true;
            DateTime ahora = DateTime.Now;
            dtpHoraInicio.Value = ahora;
            dtpHoraFin.Value = ahora.AddMinutes(60);
            _iniciandoForm = false;

            CargarInvestigadores(
                DateTime.Today,
                dtpHoraInicio.Value.ToString("HH:mm"),
                dtpHoraFin.Value.ToString("HH:mm")
            );

            // Si viene con una reunión a editar cargamos sus datos
            if (_reunionEditando != null)
            {
                this.Text = "Editar Reunión";
                btnGuardar.Text = "Guardar cambios";

                _iniciandoForm = true;
                calendario.SelectionStart = _reunionEditando.FechaReu.Date;
                dtpHoraInicio.Value = DateTime.Today.Date.Add(TimeSpan.Parse(_reunionEditando.HoraReu));
                dtpHoraFin.Value = DateTime.Today.Date.Add(TimeSpan.Parse(_reunionEditando.HoraFinReu));
                _iniciandoForm = false;

                txtMotivo.Text = _reunionEditando.MotivoReu;

                // En modo edición cargar TODOS los investigadores del semillero
                // sin filtrar por ocupados para poder ver y desmarcar los actuales
                CargarInvestigadoresModoEdicion();
            }
        }

        // ── Carga investigadores en modo edición ──────────────────────
        // Muestra todos los del semillero y marca los que ya estaban
        private void CargarInvestigadoresModoEdicion()
        {
            lstInvestigadores.Items.Clear();

            // Traer todos los investigadores del semillero sin filtrar
            var todosLosInvestigadores = BD.Instancia.Usuarios
                .Find(u => u.NumeroSemillero == _usuarioActual.NumeroSemillero
                        && u.TipoUsuario == "Investigador")
                .ToList();

            foreach (var inv in todosLosInvestigadores)
            {
                // Marcar los que ya estaban en la reunión
                bool estaba = _reunionEditando.NumerosInvestigadores != null &&
                              _reunionEditando.NumerosInvestigadores.Contains(inv.NumeroUsuario);

                lstInvestigadores.Items.Add(inv.NombreUsuario, estaba);
            }

            // Guardar lista completa en Tag
            lstInvestigadores.Tag = todosLosInvestigadores;
        }
        
        private void CargarInvestigadores(DateTime? fecha = null, string horaInicio = null, string horaFin = null)
        {
            lstInvestigadores.Items.Clear();

            // Traer todos los investigadores del semillero del líder
            var todosLosInvestigadores = BD.Instancia.Usuarios
                .Find(u => u.NumeroSemillero == _usuarioActual.NumeroSemillero
                        && u.TipoUsuario == "Investigador")
                .ToList();

            // Si no hay fecha u horas seleccionadas, mostramos todos
            if (fecha == null || horaInicio == null || horaFin == null)
            {
                foreach (var inv in todosLosInvestigadores)
                    lstInvestigadores.Items.Add(inv.NombreUsuario);

                lstInvestigadores.Tag = todosLosInvestigadores;
                return;
            }

            // Convertir horas seleccionadas a TimeSpan
            TimeSpan tsInicio = TimeSpan.Parse(horaInicio);
            TimeSpan tsFin = TimeSpan.Parse(horaFin);

            // Buscar reuniones del día
            DateTime inicioDia = fecha.Value.Date;
            DateTime finDia = inicioDia.AddDays(1);

            var reunionesDelDia = BD.Instancia.Reuniones
                .Find(r => r.FechaReu >= inicioDia && r.FechaReu < finDia)
                .ToList();

            // Lista de investigadores ocupados
            List<int> investigadoresOcupados = new List<int>();

            foreach (var r in reunionesDelDia)
            {
                if (string.IsNullOrEmpty(r.HoraReu) || string.IsNullOrEmpty(r.HoraFinReu))
                    continue;

                TimeSpan rIni = TimeSpan.Parse(r.HoraReu);
                TimeSpan rFin = TimeSpan.Parse(r.HoraFinReu);

                // Verificar solapamiento
                bool seSolapa = tsInicio < rFin && tsFin > rIni;

                if (seSolapa && r.NumerosInvestigadores != null)
                {
                    foreach (int numInv in r.NumerosInvestigadores)
                        if (!investigadoresOcupados.Contains(numInv))
                            investigadoresOcupados.Add(numInv);
                }
            }

            // Mostrar solo investigadores libres
            var investigadoresLibres = todosLosInvestigadores
                .Where(u => !investigadoresOcupados.Contains(u.NumeroUsuario))
                .ToList();

            foreach (var inv in investigadoresLibres)
                lstInvestigadores.Items.Add(inv.NombreUsuario);

            lstInvestigadores.Tag = investigadoresLibres;
        }


        private void calendario_DateChanged(object sender, DateRangeEventArgs e)
        {
            _iniciandoForm = true;

            DateTime ahora = DateTime.Now;
            dtpHoraInicio.Value = e.Start.Date.AddHours(ahora.Hour).AddMinutes(ahora.Minute);
            dtpHoraFin.Value = dtpHoraInicio.Value.AddHours(1);

            _iniciandoForm = false;

            // Validaciones inmediatas al seleccionar fecha
            DateTime fechaSeleccionada = e.Start.Date;

            // Mensaje si es un día pasado
            if (fechaSeleccionada < DateTime.Today)
            {
                MessageBox.Show(
                    "No puedes programar una reunión en un día pasado.",
                    "Advertencia",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            // Mensaje si es domingo
            if (fechaSeleccionada.DayOfWeek == DayOfWeek.Sunday)
            {
                MessageBox.Show(
                    "No se pueden programar reuniones los domingos.",
                    "Advertencia",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            // Limpiar selección de investigadores
            for (int i = 0; i < lstInvestigadores.Items.Count; i++)
                lstInvestigadores.SetItemChecked(i, false);

            // Recargar investigadores para esa fecha y rango
            CargarInvestigadores(
                e.Start.Date,
                dtpHoraInicio.Value.ToString("HH:mm"),
                dtpHoraFin.Value.ToString("HH:mm")
            );

        }

        // ── Cargar investigadores del mismo semillero del líder ────────
        private void CargarInvestigadores()
        {
            lstInvestigadores.Items.Clear();

            // Traer solo los investigadores del semillero del líder
            var investigadores = BD.Instancia.Usuarios
                .Find(u => u.NumeroSemillero == _usuarioActual.NumeroSemillero
                        && u.TipoUsuario == "Investigador")
                .ToList();

            // Agregar cada investigador al CheckedListBox
            // Guardamos el objeto completo como Tag usando un wrapper
            foreach (var inv in investigadores)
            {
                lstInvestigadores.Items.Add(inv.NombreUsuario);
            }

            // Guardamos la lista completa para recuperar números después
            lstInvestigadores.Tag = investigadores;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            DateTime fechaSeleccionada = calendario.SelectionStart.Date;

            // Validar fecha
            if (fechaSeleccionada < DateTime.Today)
            {
                MessageBox.Show(
                    "No puedes programar una reunión en una fecha pasada.",
                    "Fecha no válida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Validar investigadores
            if (lstInvestigadores.CheckedIndices.Count == 0)
            {
                MessageBox.Show(
                    "Selecciona al menos un investigador para la reunión.",
                    "Sin investigadores",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Validar motivo
            if (string.IsNullOrWhiteSpace(txtMotivo.Text))
            {
                MessageBox.Show(
                    "Por favor escribe el motivo de la reunión.",
                    "Motivo vacío",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string horaInicio = dtpHoraInicio.Value.ToString("HH:mm");
            string horaFin = dtpHoraFin.Value.ToString("HH:mm");

            TimeSpan tsInicio = TimeSpan.Parse(horaInicio);
            TimeSpan tsFin = TimeSpan.Parse(horaFin);

            // Validar que hora fin sea posterior a hora inicio
            if (tsFin <= tsInicio)
            {
                MessageBox.Show(
                    "La hora final debe ser posterior a la hora de inicio.",
                    "Hora no válida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Recuperar investigadores seleccionados
            var listaInvestigadores = lstInvestigadores.Tag as List<Usuario>;
            List<int> numerosSeleccionados = new List<int>();

            foreach (int indice in lstInvestigadores.CheckedIndices)
                numerosSeleccionados.Add(listaInvestigadores[indice].NumeroUsuario);

            // Traer reuniones del día EXCEPTO la que se está editando
            DateTime inicioDia = fechaSeleccionada.ToUniversalTime();
            DateTime finDia = fechaSeleccionada.AddDays(1).ToUniversalTime();

            var reunionesDelDia = BD.Instancia.Reuniones
                .Find(r => r.FechaReu >= inicioDia
                        && r.FechaReu < finDia
                        && (_reunionEditando == null || r.Id != _reunionEditando.Id))
                .ToList();

            // Validar solapamiento del líder
            bool liderOcupado = reunionesDelDia
                .Where(r => r.NumeroLider == _usuarioActual.NumeroUsuario)
                .Any(r =>
                {
                    TimeSpan rIni = TimeSpan.Parse(r.HoraReu);
                    TimeSpan rFin = TimeSpan.Parse(r.HoraFinReu);
                    return tsInicio < rFin && tsFin > rIni;
                });

            if (liderOcupado)
            {
                MessageBox.Show(
                    $"Ya tienes una reunión en ese rango horario el {fechaSeleccionada:dd/MM/yyyy}.",
                    "Horario ocupado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Validar solapamiento de investigadores
            foreach (int numInv in numerosSeleccionados)
            {
                bool invOcupado = reunionesDelDia
                    .Where(r => r.NumerosInvestigadores != null &&
                                r.NumerosInvestigadores.Contains(numInv))
                    .Any(r =>
                    {
                        TimeSpan rIni = TimeSpan.Parse(r.HoraReu);
                        TimeSpan rFin = TimeSpan.Parse(r.HoraFinReu);
                        return tsInicio < rFin && tsFin > rIni;
                    });

                if (invOcupado)
                {
                    var invUsuario = listaInvestigadores
                        .FirstOrDefault(u => u.NumeroUsuario == numInv);

                    MessageBox.Show(
                        $"{invUsuario?.NombreUsuario} tiene una reunión que se solapa.",
                        "Investigador ocupado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
            }

            if (_reunionEditando == null)
            {
                // ── Modo registro ─────────────────────────────────────────
                Reunion nuevaReunion = new Reunion
                {
                    FechaReu = fechaSeleccionada.ToUniversalTime(),
                    HoraReu = horaInicio,
                    HoraFinReu = horaFin,
                    MotivoReu = txtMotivo.Text.Trim(),
                    NumeroLider = _usuarioActual.NumeroUsuario,
                    NumerosInvestigadores = numerosSeleccionados
                };

                BD.Instancia.Reuniones.InsertOne(nuevaReunion);

                MessageBox.Show(
                    $"Reunión guardada exitosamente.\n" +
                    $"Fecha: {fechaSeleccionada:dd/MM/yyyy}\n" +
                    $"Horario: {horaInicio} - {horaFin}",
                    "Reunión programada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                btnlimpiar_Click(sender, e);
            }
            else
            {
                // ── Modo edición ──────────────────────────────────────────
                bool fueReprogramada = _reunionEditando.FechaReu.Date != fechaSeleccionada.Date ||
                                       _reunionEditando.HoraReu != horaInicio ||
                                       _reunionEditando.HoraFinReu != horaFin;

                string nuevoEstado = fueReprogramada ? "Reprogramada" : _reunionEditando.EstadoReu;

                var filtroEdicion = Builders<Reunion>.Filter.Eq(r => r.Id, _reunionEditando.Id);
                var actualizacionEdicion = Builders<Reunion>.Update
                    .Set(r => r.FechaReu, fechaSeleccionada.ToUniversalTime())
                    .Set(r => r.HoraReu, horaInicio)
                    .Set(r => r.HoraFinReu, horaFin)
                    .Set(r => r.MotivoReu, txtMotivo.Text.Trim())
                    .Set(r => r.NumerosInvestigadores, numerosSeleccionados)
                    .Set(r => r.EstadoReu, nuevoEstado);

                BD.Instancia.Reuniones.UpdateOne(filtroEdicion, actualizacionEdicion);

                MessageBox.Show(
                    "Reunión actualizada correctamente.",
                    "Éxito",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                this.Close();
            }
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

                // Si está en modo edición solo cerrar este form
                // el Form_Lider_Consultar ya está abierto debajo
                if (_reunionEditando != null)
                {
                    // Cerrar también el form de consultas que está debajo
                    foreach (Form f in Application.OpenForms.Cast<Form>().ToList())
                    {
                        if (f is Form_Lider_Consultar)
                            f.Close();
                    }
                }
                else
                {
                    // Cerrar el form de consultas si está abierto
                    foreach (Form f in Application.OpenForms.Cast<Form>().ToList())
                    {
                        if (f is Form_Lider_Consultar)
                            f.Close();
                    }
                }

                formLogin.Show();
                this.Close();
            }
        }

        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            // Limpiar el motivo
            txtMotivo.Clear();

            // Deseleccionar investigadores
            for (int i = 0; i < lstInvestigadores.Items.Count; i++)
                lstInvestigadores.SetItemChecked(i, false);

            // Reiniciar horas con la hora actual
            _iniciandoForm = true;
            DateTime ahora = DateTime.Now;
            dtpHoraInicio.Value = ahora;
            dtpHoraFin.Value = ahora.AddHours(1);

            // Reiniciar calendario al día de hoy
            calendario.SelectionStart = DateTime.Today;
            _iniciandoForm = false;

            // Recargar investigadores para el nuevo rango
            CargarInvestigadores(
                DateTime.Today,
                dtpHoraInicio.Value.ToString("HH:mm"),
                dtpHoraFin.Value.ToString("HH:mm")
            );
        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void btnVerReuniones_Click(object sender, EventArgs e)
        {
            // Si está en modo edición preguntar si desea descartar cambios
            if (_reunionEditando != null)
            {
                DialogResult confirm = MessageBox.Show(
                    "¿Deseas descartar los cambios y volver a consultas?",
                    "Descartar cambios",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm == DialogResult.No)
                    return;

                // Si dice sí simplemente cerrar este form
                // el Form_Lider_Consultar ya está abierto debajo (ShowDialog)
                this.Close();
                return;
            }

            // Modo normal — abrir consultas
            Form_Lider_Consultar flc = new Form_Lider_Consultar(_usuarioActual);
            flc.Show();
            this.Hide();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        { }

        private void dtpHoraInicio_ValueChanged(object sender, EventArgs e)
        {
            if (_iniciandoForm) return;

            // Solo validar hora si es hoy
            if (calendario.SelectionStart.Date == DateTime.Today)
            {
                // No permitir la hora actual ni horas anteriores
                // Mínimo debe ser hora actual + 1
                if (dtpHoraInicio.Value.Hour <= DateTime.Now.Hour)
                {
                    MessageBox.Show(
                        $"No puedes programar una reunión a las {dtpHoraInicio.Value:HH:mm}.\n" +
                        $"La hora mínima permitida es {DateTime.Now.AddHours(1):HH:mm}.",
                        "Hora no válida",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    _iniciandoForm = true;
                    dtpHoraInicio.Value = dtpHoraInicio.Value.Date.AddHours(DateTime.Now.Hour + 1);
                    dtpHoraFin.Value = dtpHoraInicio.Value.AddHours(1);
                    _iniciandoForm = false;
                    return;
                }
            }

            _iniciandoForm = true;
            dtpHoraFin.Value = dtpHoraInicio.Value.AddHours(1);
            _iniciandoForm = false;

            // En modo edición mantener investigadores marcados
            if (_reunionEditando != null)
                CargarInvestigadoresModoEdicion();
            else
                CargarInvestigadores(
                    calendario.SelectionStart.Date,
                    dtpHoraInicio.Value.ToString("HH:mm"),
                    dtpHoraFin.Value.ToString("HH:mm")
                );
        }

        private void dtpHoraFin_ValueChanged(object sender, EventArgs e)
        {
            if (_iniciandoForm) return;

            if (dtpHoraFin.Value.TimeOfDay <= dtpHoraInicio.Value.TimeOfDay)
            {
                MessageBox.Show(
                    "La hora final debe ser posterior a la hora de inicio.",
                    "Hora no válida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                _iniciandoForm = true;
                dtpHoraFin.Value = dtpHoraInicio.Value.AddHours(1);
                _iniciandoForm = false;
                return;
            }

            // En modo edición mantener investigadores marcados
            if (_reunionEditando != null)
                CargarInvestigadoresModoEdicion();
            else
                CargarInvestigadores(
                    calendario.SelectionStart.Date,
                    dtpHoraInicio.Value.ToString("HH:mm"),
                    dtpHoraFin.Value.ToString("HH:mm")
                );
        }
    }
}
