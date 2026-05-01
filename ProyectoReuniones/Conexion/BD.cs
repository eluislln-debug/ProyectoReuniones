using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoReuniones.Conexion
{
    // Uso: BD.Instancia.Usuarios  /  BD.Instancia.Reuniones
    public static class BD
    {
        private static ConexionMongoDB _instancia;
        private static readonly object _lock = new object();

        public static ConexionMongoDB Instancia
        {
            get
            {
                // Solo crea la conexion una vez en toda la app
                if (_instancia == null)
                {
                    lock (_lock)
                    {
                        if (_instancia == null)
                            _instancia = new ConexionMongoDB();
                    }
                }
                return _instancia;
            }
        }
    }
}
