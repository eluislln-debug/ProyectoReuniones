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
        }
    }
}
