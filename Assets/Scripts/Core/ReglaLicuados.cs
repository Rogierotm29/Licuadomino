namespace Licuadomino.Core
{

    public class ReglaLicuados : IReglaCompatibilidad
    {
        public string Nombre => "Licuados";

        public ResultadoConexion Evaluar(Fruta libre, Fruta nueva)
        {
            Perfil pLibre = FrutaInfo.PerfilDe(libre);
            Perfil pNueva = FrutaInfo.PerfilDe(nueva);
            Perfil necesita = FrutaInfo.Complemento(pLibre);

            string nLibre = FrutaInfo.Nombre(libre);
            string nNueva = FrutaInfo.Nombre(nueva);

            if (pNueva == necesita)
                return ResultadoConexion.Ok(
                    $"¡{nLibre} + {nNueva}: buen licuado! {FrutaInfo.RazonBalance(pLibre)}");

            string sugerencia =
                $"Ocupa una fruta {FrutaInfo.Adjetivo(necesita)} ({FrutaInfo.ListaDePerfil(necesita)}).";

            if (libre == nueva)
                return ResultadoConexion.Error(
                    $"{nLibre} con {nNueva} no es licuado, es puro {nLibre}. {sugerencia}");

            if (pNueva == pLibre)
                return ResultadoConexion.Error(
                    $"{nLibre} y {nNueva} son frutas {FrutaInfo.AdjetivoPlural(pLibre)}: " +
                    $"{FrutaInfo.Exceso(pLibre)}. {sugerencia}");

            return ResultadoConexion.Error(
                $"{nLibre} es una fruta {FrutaInfo.Adjetivo(pLibre)} y {nNueva}, una " +
                $"{FrutaInfo.Adjetivo(pNueva)}: no se balancean. {sugerencia}");
        }
    }
}
