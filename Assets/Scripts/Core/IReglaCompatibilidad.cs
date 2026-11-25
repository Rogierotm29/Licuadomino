namespace Licuadomino.Core
{
    public interface IReglaCompatibilidad
    {
        string Nombre { get; }

        ///<param name="libre"
        /// <param name="nueva">
        ResultadoConexion Evaluar(Fruta libre, Fruta nueva);
    }
}
