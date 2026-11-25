using System.Collections.Generic;

namespace Licuadomino.Core
{
   
    public enum Fruta { Platano, Coco, Limon, Naranja, Mango, Fresa, Sandia, Melon }


    public enum Perfil { Cremosa, Acida, Dulce, Refrescante }


    public static class FrutaInfo
    {
        public const int Cantidad = 8;

        static readonly string[] nombres =
            { "Plátano", "Coco", "Limón", "Naranja", "Mango", "Fresa", "Sandía", "Melón" };

        static readonly Perfil[] perfiles =
        {
            Perfil.Cremosa, Perfil.Cremosa,
            Perfil.Acida, Perfil.Acida,
            Perfil.Dulce, Perfil.Dulce,
            Perfil.Refrescante, Perfil.Refrescante
        };

        public static string Nombre(Fruta f) => nombres[(int)f];
        public static Perfil PerfilDe(Fruta f) => perfiles[(int)f];

        static readonly Perfil[] complementos = { Perfil.Acida, Perfil.Cremosa, Perfil.Refrescante, Perfil.Dulce };
        static readonly string[] adjetivos = { "cremosa", "ácida", "dulce", "refrescante" };
        static readonly string[] adjetivosPlural = { "cremosas", "ácidas", "dulces", "refrescantes" };
        static readonly string[] excesos = { "queda muy espeso", "queda muy agrio", "queda empalagoso", "queda aguado" };

        public static Perfil Complemento(Perfil p) => complementos[(int)p];

        public static string Adjetivo(Perfil p) => adjetivos[(int)p];
        public static string AdjetivoPlural(Perfil p) => adjetivosPlural[(int)p];

        public static string Exceso(Perfil p) => excesos[(int)p];

        public static string RazonBalance(Perfil p) =>
            p == Perfil.Cremosa || p == Perfil.Acida
                ? "Lo ácido corta lo cremoso."
                : "Lo refrescante aligera lo dulce.";

        public static IEnumerable<Fruta> DePerfil(Perfil p)
        {
            for (int i = 0; i < Cantidad; i++)
                if (perfiles[i] == p) yield return (Fruta)i;
        }

        public static string ListaDePerfil(Perfil p) => string.Join(" o ", NombresDePerfil(p));

        static IEnumerable<string> NombresDePerfil(Perfil p)
        {
            foreach (var f in DePerfil(p)) yield return Nombre(f);
        }
    }
}
