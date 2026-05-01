using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace ProyectoReuniones.Modelos
{
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("numeroUsuario")]
        public int NumeroUsuario { get; set; }

        [BsonElement("nombreUsuario")]
        public string NombreUsuario { get; set; }

        [BsonElement("correoUsuario")]
        public string CorreoUsuario { get; set; }

        [BsonElement("contraUsuario")]
        public int ContraUsuario { get; set; }

        [BsonElement("tipoUsuario")]
        public string TipoUsuario { get; set; }

        [BsonElement("numeroSemillero")]
        public int NumeroSemillero { get; set; }
    }
}
