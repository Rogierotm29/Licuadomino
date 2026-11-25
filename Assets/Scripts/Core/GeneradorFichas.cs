using System;
using System.Collections.Generic;

namespace Licuadomino.Core
{

    public static class GeneradorFichas
    {
        public const int Total = FrutaInfo.Cantidad * (FrutaInfo.Cantidad + 1) / 2; // 36

        static readonly Ficha[] set = Crear();

        public static IReadOnlyList<Ficha> SetCompleto => set;

        public static Ficha PorId(int id)
        {
            if (id < 0 || id >= Total) throw new ArgumentOutOfRangeException(nameof(id));
            return set[id];
        }

        static Ficha[] Crear()
        {
            var lista = new Ficha[Total];
            int id = 0;
            for (int a = 0; a < FrutaInfo.Cantidad; a++)
                for (int b = a; b < FrutaInfo.Cantidad; b++)
                {
                    lista[id] = new Ficha(id, (Fruta)a, (Fruta)b);
                    id++;
                }
            return lista;
        }

        public static List<Ficha> Barajado(Random rng)
        {
            var lista = new List<Ficha>(set);
            for (int i = lista.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                Ficha tmp = lista[i];
                lista[i] = lista[j];
                lista[j] = tmp;
            }
            return lista;
        }
    }
}
