using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProyectoReuniones.Modelos
{
    public class Semillero
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("numeroSemillero")]
        public int NumeroSemillero { get; set; }

        [BsonElement("nombreSemillero")]
        public string NombreSemillero { get; set; }

        [BsonElement("descripcion")]
        public string Descripcion { get; set; }
    }
}
