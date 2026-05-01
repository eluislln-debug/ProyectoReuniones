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

        public FormLider(Usuario usuario)
        {
            InitializeComponent();
            // Guardamos el usuario para usarlo en toda la ventana
            _usuarioActual = usuario;
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

            // Buscar todas las reuniones del líder en esa fecha
            var reunionesDelDia = BD.Instancia.Reuniones
                .Find(r => r.NumeroLider == _usuarioActual.NumeroUsuario
                        && r.FechaReu.Date == fecha.Date)
                .ToList();

            // Obtener las horas ya ocupadas por el líder ese día
            var horasOcupadas = reunionesDelDia.Select(r => r.HoraReu).ToList();

            // Agregar cada hora indicando si está disponible u ocupada
            foreach (string hora in _todasLasHoras)
            {
                if (horasOcupadas.Contains(hora))
                    lstHoras.Items.Add(hora + "  ✗ Ocupada");
                else
                    lstHoras.Items.Add(hora + "  ✓ Disponible");
            }
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
            // Validar que se seleccionó una hora disponible
            if (lstHoras.SelectedIndex < 0)
            {
                MessageBox.Show(
                    "Por favor selecciona una hora.",
                    "Hora no seleccionada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Verificar que la hora seleccionada no esté ocupada
            string horaSeleccionada = _todasLasHoras[lstHoras.SelectedIndex];
            string itemSeleccionado = lstHoras.SelectedItem.ToString();

            if (itemSeleccionado.Contains("✗ Ocupada"))
            {
                MessageBox.Show(
                    "Esa hora ya está ocupada. Selecciona una hora disponible.",
                    "Hora no disponible",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Validar que se seleccionó al menos un investigador
            if (lstInvestigadores.CheckedIndices.Count == 0)
            {
                MessageBox.Show(
                    "Selecciona al menos un investigador para la reunión.",
                    "Sin investigadores",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Validar que el motivo no esté vacío
            if (string.IsNullOrWhiteSpace(txtMotivo.Text))
            {
                MessageBox.Show(
                    "Por favor escribe el motivo de la reunión.",
                    "Motivo vacío",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Obtener fecha seleccionada
            DateTime fechaSeleccionada = calendario.SelectionStart.Date;

            // ── Validar que la fecha no sea anterior a hoy ────────────────
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

            // Recuperar lista de investigadores del Tag
            var listaInvestigadores = lstInvestigadores.Tag as List<Usuario>;

            // Obtener números de investigadores marcados
            List<int> numerosSeleccionados = new List<int>();
            foreach (int indice in lstInvestigadores.CheckedIndices)
            {
                numerosSeleccionados.Add(listaInvestigadores[indice].NumeroUsuario);
            }

            // ── Traer todas las reuniones de esa fecha y hora ─────────────
            // Se trae la lista y se valida en C# para evitar problemas
            // con la comparacion de listas en el driver de MongoDB
            var reunionesEnEseFecha = BD.Instancia.Reuniones
                .Find(r => r.HoraReu == horaSeleccionada &&
                           r.FechaReu == fechaSeleccionada)
                .ToList();

            // ── Validar que el líder no tenga ya una reunión esa hora ─────
            bool liderOcupado = reunionesEnEseFecha
                .Any(r => r.NumeroLider == _usuarioActual.NumeroUsuario);

            if (liderOcupado)
            {
                MessageBox.Show(
                    $"Ya tienes una reunión programada el " +
                    $"{fechaSeleccionada:dd/MM/yyyy} a las {horaSeleccionada}.",
                    "Hora ocupada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // ── Validar que ningún investigador esté ocupado esa hora ─────
            foreach (int numInv in numerosSeleccionados)
            {
                // Buscar si ese investigador aparece en alguna reunión de esa hora
                bool invOcupado = reunionesEnEseFecha
                    .Any(r => r.NumerosInvestigadores != null &&
                              r.NumerosInvestigadores.Contains(numInv));

                if (invOcupado)
                {
                    var invUsuario = listaInvestigadores
                        .FirstOrDefault(u => u.NumeroUsuario == numInv);

                    MessageBox.Show(
                        $"{invUsuario?.NombreUsuario} ya tiene una reunión " +
                        $"el {fechaSeleccionada:dd/MM/yyyy} a las {horaSeleccionada}.\n" +
                        "Selecciona otra hora o deselecciona ese investigador.",
                        "Investigador ocupado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
            }

            // ── Crear y guardar la reunión ────────────────────────────────
            Reunion nuevaReunion = new Reunion
            {
                FechaReu = fechaSeleccionada,
                HoraReu = horaSeleccionada,
                MotivoReu = txtMotivo.Text.Trim(),
                NumeroLider = _usuarioActual.NumeroUsuario,
                NumerosInvestigadores = numerosSeleccionados
            };

            BD.Instancia.Reuniones.InsertOne(nuevaReunion);

            MessageBox.Show(
                $"Reunión guardada exitosamente.\n" +
                $"Fecha: {fechaSeleccionada:dd/MM/yyyy}\n" +
                $"Hora: {horaSeleccionada}",
                "Reunión programada",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            // Recargar horas para que la recién guardada quede como ocupada
            CargarHorasDisponibles(fechaSeleccionada);

            // Limpiar campos
            txtMotivo.Clear();
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
            // Limpiar el motivo
            txtMotivo.Clear();

            // Desmarcar todos los investigadores
            for (int i = 0; i < lstInvestigadores.Items.Count; i++)
                lstInvestigadores.SetItemChecked(i, false);

            // Deseleccionar la hora
            lstHoras.ClearSelected();

            // Volver el calendario al día de hoy
            calendario.SelectionStart = DateTime.Today;

            // Recargar las horas disponibles para el día de hoy
            CargarHorasDisponibles(DateTime.Today);
        }
    }
}
