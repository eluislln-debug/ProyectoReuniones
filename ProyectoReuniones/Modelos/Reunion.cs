using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProyectoReuniones.Modelos
{
    
    public class Reunion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("fechaReu")]
        public DateTime FechaReu { get; set; }

        [BsonElement("horaReu")]
        public string HoraReu { get; set; }

        [BsonElement("horaFinReu")]
        public string HoraFinReu { get; set; }

        [BsonElement("motivoReu")]
        public string MotivoReu { get; set; }

        [BsonElement("numeroLider")]
        public int NumeroLider { get; set; }

        [BsonElement("numerosInvestigadores")]
        public List<int> NumerosInvestigadores { get; set; }
    }
}
