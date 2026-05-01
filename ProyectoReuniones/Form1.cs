using MongoDB.Driver;
using ProyectoReuniones.Conexion;
using ProyectoReuniones.Modelos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoReuniones
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void txtContrasena_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir solo dígitos y la tecla backspace para borrar
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true; // Bloquea cualquier otro carácter
            }
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            string correo = txtCorreo.Text.Trim();
            string contrasena = txtContrasena.Text.Trim();

            // Validar que los campos no estén vacíos
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena))
            {
                MessageBox.Show(
                    "Por favor completa todos los campos.",
                    "Campos vacíos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // Convertir contraseña a entero (siempre será numérica por el KeyPress)
            int contraInt = int.Parse(contrasena);

            try
            {
                // Buscar el usuario en MongoDB por correo y contraseña
                var filtro = Builders<Usuario>.Filter.And(
                    Builders<Usuario>.Filter.Eq(u => u.CorreoUsuario, correo),
                    Builders<Usuario>.Filter.Eq(u => u.ContraUsuario, contraInt)
                );

                Usuario usuarioEncontrado = BD.Instancia.Usuarios
                    .Find(filtro)
                    .FirstOrDefault();

                // Si no existe el usuario mostrar error
                if (usuarioEncontrado == null)
                {
                    MessageBox.Show(
                        "Correo o contraseña incorrectos.",
                        "Acceso denegado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                    // Limpiar solo la contraseña para que reintente
                    txtContrasena.Clear();
                    txtContrasena.Focus();
                    return;
                }

                // Redirigir según el tipo de usuario
                if (usuarioEncontrado.TipoUsuario == "Lider")
                {
                    FormLider formLider = new FormLider(usuarioEncontrado);
                    this.Hide();
                    formLider.Show();
                }
                else if (usuarioEncontrado.TipoUsuario == "Investigador")
                {
                    FormInvestigador formInvestigador = new FormInvestigador(usuarioEncontrado);
                    this.Hide();
                    formInvestigador.Show();
                }
            }
            catch (Exception ex)
            {
                // Error de conexión u otro problema inesperado
                MessageBox.Show(
                    "Error de conexión: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
