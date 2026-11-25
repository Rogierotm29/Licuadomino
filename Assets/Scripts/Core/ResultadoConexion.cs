namespace Licuadomino.Core
{

    public readonly struct ResultadoConexion
    {
        public readonly bool Valida;
        public readonly string Mensaje;

        ResultadoConexion(bool valida, string mensaje)
        {
            Valida = valida;
            Mensaje = mensaje;
        }

        public static ResultadoConexion Ok(string mensaje) => new ResultadoConexion(true, mensaje);
        public static ResultadoConexion Error(string mensaje) => new ResultadoConexion(false, mensaje);

        public override string ToString() => (Valida ? "OK: " : "ERROR: ") + Mensaje;
    }
}
