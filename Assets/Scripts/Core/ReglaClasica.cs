namespace Licuadomino.Core
{
    public class ReglaClasica : IReglaCompatibilidad
    {
        public string Nombre => "Clásica";

        public ResultadoConexion Evaluar(Fruta libre, Fruta nueva) =>
            libre == nueva
                ? ResultadoConexion.Ok($"{FrutaInfo.Nombre(libre)} con {FrutaInfo.Nombre(nueva)}.")
                : ResultadoConexion.Error($"Tiene que ser {FrutaInfo.Nombre(libre)}.");
    }
}
