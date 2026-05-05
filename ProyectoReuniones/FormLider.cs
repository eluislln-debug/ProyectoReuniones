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
       

        // ── Horas fijas disponibles para reuniones ────────────────────
        private readonly List<string> _todasLasHoras = new List<string>
        {
            "08:00", "09:00", "10:00", "11:00",
            "12:00", "13:00", "14:00", "15:00",
            "16:00", "17:00"
        };

        private bool _cargandoHoras = false;

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

            // --- 2. DISEÑO: Lista de Horas (lstHoras) ---
            lstHoras.BorderStyle = BorderStyle.None;
            lstHoras.BackColor = Color.White;
            lstHoras.Font = new Font("Segoe UI", 10F);
            lstHoras.ItemHeight = 30; // Más espacio para cada hora
            lstHoras.ForeColor = Color.FromArgb(71, 85, 105);

            // --- Lista Hora Final (lstHoraFin) igual que lstHoras ---
            lstHoraFin.BorderStyle = BorderStyle.None;
            lstHoraFin.BackColor = Color.White;
            lstHoraFin.Font = new Font("Segoe UI", 10F);
            lstHoraFin.ItemHeight = 30;
            lstHoraFin.ForeColor = Color.FromArgb(71, 85, 105);

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

        public FormLider()
        {
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

        private void FormLider_Load(object sender, EventArgs e)
        {
            // Mostrar nombre del líder y su semillero
            lblBienvenida.Text = "Bienvenido, " + _usuarioActual.NombreUsuario;

            // Buscar el semillero del líder para mostrar su nombre
            var semillero = BD.Instancia.Semilleros
                .Find(s => s.NumeroSemillero == _usuarioActual.NumeroSemillero)
                .FirstOrDefault();

            lblSemillero.Text = semillero != null
                ? "Semillero: " + semillero.NombreSemillero
                : "Semillero: No encontrado";

            // Cargar horas y investigadores con el día de hoy por defecto
            CargarHorasDisponibles(DateTime.Today);
            CargarInvestigadores();
        }

        private void calendario_DateChanged(object sender, DateRangeEventArgs e)
        {
            // Recargar horas disponibles para el nuevo día seleccionado
            CargarHorasDisponibles(e.Start);

            // Limpiar selección de investigadores al cambiar de día
            for (int i = 0; i < lstInvestigadores.Items.Count; i++)
                lstInvestigadores.SetItemChecked(i, false);
        }

        // ── Cargar horas deshabilitando las ya ocupadas ───────────────
        private void CargarHorasDisponibles(DateTime fecha)
        {
            lstHoras.Items.Clear();
            lstHoraFin.Items.Clear();

            // Buscar todas las reuniones del líder en esa fecha
            DateTime inicioDia = fecha.Date;
            DateTime finDia = inicioDia.AddDays(1);

            var reunionesDelDia = BD.Instancia.Reuniones
                .Find(r => r.NumeroLider == _usuarioActual.NumeroUsuario
                        && r.FechaReu >= inicioDia && r.FechaReu < finDia)
                .ToList(); ;

            // Construir los rangos ocupados del líder ese día
            // Cada reunión bloquea desde su hora inicio hasta su hora fin
            List<(TimeSpan inicio, TimeSpan fin)> rangosOcupados = new List<(TimeSpan, TimeSpan)>();

            foreach (var r in reunionesDelDia)
            {
                TimeSpan ini = TimeSpan.Parse(r.HoraReu);
                TimeSpan fin = TimeSpan.Parse(r.HoraFinReu);
                rangosOcupados.Add((ini, fin));
            }

            // Agregar cada hora indicando disponibilidad
            foreach (string hora in _todasLasHoras)
            {
                TimeSpan horaTS = TimeSpan.Parse(hora);

                // Una hora está ocupada si cae dentro de algún rango existente
                bool ocupada = rangosOcupados.Any(r => horaTS >= r.inicio && horaTS < r.fin);

                if (ocupada)
                    lstHoras.Items.Add(hora + "  ✗ Ocupada");
                else
                    lstHoras.Items.Add(hora + "  ✓ Disponible");
            }

            // Limpiar hora fin al recargar
            lstHoraFin.Items.Clear();
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
            // Validar fecha
            DateTime fechaSeleccionada = calendario.SelectionStart.Date;

            if (fechaSeleccionada < DateTime.Today)
            {
                MessageBox.Show(
                    "No puedes programar una reunión en una fecha pasada.\n" +
                    $"La fecha mínima permitida es el {DateTime.Today:dd/MM/yyyy}.",
                    "Fecha no válida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Validar hora de inicio
            if (lstHoras.SelectedIndex < 0 ||
                lstHoras.SelectedItem.ToString().Contains("✗ Ocupada"))
            {
                MessageBox.Show(
                    "Por favor selecciona una hora de inicio disponible.",
                    "Hora no seleccionada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Validar hora final
            if (lstHoraFin.SelectedIndex < 0)
            {
                MessageBox.Show(
                    "Por favor selecciona la hora final de la reunión.",
                    "Hora final no seleccionada",
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

            string horaInicio = _todasLasHoras[lstHoras.SelectedIndex];
            string horaFin = lstHoraFin.SelectedItem.ToString();

            TimeSpan tsInicio = TimeSpan.Parse(horaInicio);
            TimeSpan tsFin = TimeSpan.Parse(horaFin);

            // Recuperar investigadores marcados
            var listaInvestigadores = lstInvestigadores.Tag as List<Usuario>;
            List<int> numerosSeleccionados = new List<int>();

            foreach (int indice in lstInvestigadores.CheckedIndices)
                numerosSeleccionados.Add(listaInvestigadores[indice].NumeroUsuario);

            // Traer todas las reuniones de esa fecha para validar
            DateTime inicioDia = fechaSeleccionada.Date;
            DateTime finDia = inicioDia.AddDays(1);

            var reunionesDelDia = BD.Instancia.Reuniones
                .Find(r => r.FechaReu >= inicioDia && r.FechaReu < finDia)
                .ToList();

            // ── Validar que el nuevo rango no choque con reuniones del líder ──
            bool liderOcupado = reunionesDelDia
                .Where(r => r.NumeroLider == _usuarioActual.NumeroUsuario)
                .Any(r =>
                {
                    TimeSpan rIni = TimeSpan.Parse(r.HoraReu);
                    TimeSpan rFin = TimeSpan.Parse(r.HoraFinReu);

                    // Hay choque si los rangos se solapan
                    return tsInicio < rFin && tsFin > rIni;
                });

            if (liderOcupado)
            {
                MessageBox.Show(
                    $"Ya tienes una reunión en ese rango horario el " +
                    $"{fechaSeleccionada:dd/MM/yyyy}.\n" +
                    "Selecciona un horario que no se solape con tus reuniones existentes.",
                    "Horario ocupado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // ── Validar que ningún investigador tenga choque de horario ──────
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
                        $"{invUsuario?.NombreUsuario} tiene una reunión que se " +
                        $"solapa con el horario seleccionado.\n" +
                        "Selecciona otro horario o deselecciona ese investigador.",
                        "Investigador ocupado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
            }

            // ── Guardar reunión ───────────────────────────────────────────────
            Reunion nuevaReunion = new Reunion
            {
                FechaReu = fechaSeleccionada,
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

            // Recargar horas y limpiar campos
            CargarHorasDisponibles(fechaSeleccionada);
            txtMotivo.Clear();
            lstHoraFin.Items.Clear();

            for (int i = 0; i < lstInvestigadores.Items.Count; i++)
                lstInvestigadores.SetItemChecked(i, false);
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

        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            txtMotivo.Clear();

            for (int i = 0; i < lstInvestigadores.Items.Count; i++)
                lstInvestigadores.SetItemChecked(i, false);

            lstHoras.ClearSelected();

            // Limpiar también la lista de hora final
            lstHoraFin.Items.Clear();

            calendario.SelectionStart = DateTime.Today;
            CargarHorasDisponibles(DateTime.Today);
        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void btnVerReuniones_Click(object sender, EventArgs e)
        {
            Form_Lider_Consultar flc = new Form_Lider_Consultar(_usuarioActual);
            flc.Show();
            this.Hide();
        }

        private void lstHoras_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Si ya estamos cargando horas no hacer nada para evitar bucle
            if (_cargandoHoras) return;

            lstHoraFin.Items.Clear();

            if (lstHoras.SelectedIndex < 0) return;
            if (lstHoras.SelectedItem.ToString().Contains("✗ Ocupada")) return;

            // Activar bandera antes de llenar la lista
            _cargandoHoras = true;

            string horaInicio = _todasLasHoras[lstHoras.SelectedIndex];
            TimeSpan tsInicio = TimeSpan.Parse(horaInicio);

            foreach (string hora in _todasLasHoras)
            {
                TimeSpan tsHora = TimeSpan.Parse(hora);
                if (tsHora > tsInicio)
                    lstHoraFin.Items.Add(hora);
            }

            // Desactivar bandera al terminar
            _cargandoHoras = false;
        }

        private void lstHoraFin_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstHoraFin.Items.Clear();

            // Si no hay selección o la hora está ocupada no hacer nada
            if (lstHoras.SelectedIndex < 0) return;
            if (lstHoras.SelectedItem.ToString().Contains("✗ Ocupada")) return;

            // Hora de inicio seleccionada
            string horaInicio = _todasLasHoras[lstHoras.SelectedIndex];
            TimeSpan tsInicio = TimeSpan.Parse(horaInicio);

            // Solo mostrar horas POSTERIORES a la de inicio hasta las 17:00
            foreach (string hora in _todasLasHoras)
            {
                TimeSpan tsHora = TimeSpan.Parse(hora);
                if (tsHora > tsInicio)
                    lstHoraFin.Items.Add(hora);
            }

            // Agregar 18:00 como opción final máxima si inicio es 17:00
            // En este caso no hay opciones y se puede manejar aparte
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        { }
    }
}
