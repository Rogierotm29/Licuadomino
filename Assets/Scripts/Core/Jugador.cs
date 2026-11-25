using System.Collections.Generic;

namespace Licuadomino.Core
{
    /// <summary>Un jugador y su mano. Solo el host (Master Client) conoce todas las manos.</summary>
    public class Jugador
    {
        readonly List<Ficha> mano = new List<Ficha>();

        public int Id { get; }
        public string Nombre { get; }
        public IReadOnlyList<Ficha> Mano => mano;

        public Jugador(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }

        public bool Tiene(int fichaId)
        {
            foreach (var f in mano)
                if (f.Id == fichaId) return true;
            return false;
        }

        internal void Agregar(Ficha f) => mano.Add(f);
        internal bool Quitar(int fichaId) => mano.RemoveAll(f => f.Id == fichaId) > 0;
        internal void VaciarMano() => mano.Clear();

        public override string ToString() => Nombre;
    }
}
