namespace SAT.CFDI.Cliente.Procesamiento
{
    using System;
    using System.Text;

    public static class AdministradorLogs
    {
        #region Campos

        private static StringBuilder logBuilder = null;

        #endregion

        #region Propiedades
        public static void RegistraEntrada(string entradaLog)
        {
            if(logBuilder == null)
            {
                logBuilder = new StringBuilder();
            }

            logBuilder.AppendLine(string.Format("Entrada al log: {0}", DateTime.Now.ToLongDateString()));
            logBuilder.AppendLine(entradaLog);
            logBuilder.AppendLine();
        }

        public static void GenerarLog (string rutaAbsoluta)
        {
            AccesoDisco.GuardarArchivoTexto(rutaAbsoluta, logBuilder.ToString());            
        }
        #endregion
    }
}
