using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using ProyectoReuniones.Modelos;

namespace ProyectoReuniones.Conexion
{
    public class ConexionMongoDB
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        // ── Nombre de la base de datos en un solo lugar ──────────────
        private const string NombreDB = "SeedLab";

        public ConexionMongoDB()
        {
            try
            {
                _client = new MongoClient("mongodb://localhost:27017/");
                _database = _client.GetDatabase(NombreDB);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al conectar con MongoDB: " + ex.Message);
            }
        }

        // ── Colecciones tipadas (evita escribir el nombre a mano) ─────
        public IMongoCollection<Usuario> Usuarios =>
            _database.GetCollection<Usuario>("Usuarios");

        public IMongoCollection<Reunion> Reuniones =>
            _database.GetCollection<Reunion>("Reuniones");

        public IMongoCollection<Semillero> Semilleros =>
            _database.GetCollection<Semillero>("Semilleros");

        // ── Método genérico por si necesitas otra colección después ───
        public IMongoCollection<T> GetCollection<T>(string nombre) =>
            _database.GetCollection<T>(nombre);
    }
}
