namespace SAT.CFDI.Cliente.Procesamiento
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class AccesoDisco
    {
        #region Métodos Públicos
        public static IList RecuperaListaArchivos(string directorioRaiz)
        {
            IList listaArchivos = Directory.GetFiles(directorioRaiz).ToList();
            return listaArchivos;
        }

        public static Stream RecuperaArchivo(string rutaAbsoluta)
        {
            return File.OpenRead(rutaAbsoluta);
        }

        public static void MoverArchivo(string rutaAbsoluta, string rutaDestino)
        {
            var nombreArchivo = Path.GetFileName(rutaAbsoluta);
            File.Move(rutaAbsoluta, string.Format("{0}\\{1}", rutaDestino, nombreArchivo));
        }

        public static void GuardarArchivoLog(string rutaAbsoluta, List<string> contenidoArchivo)
        {
            File.WriteAllLines(rutaAbsoluta, contenidoArchivo.ToArray());
        }

        public static void GuardarArchivoTexto(string rutaAbsoluta, string contenidoArchivo)
        {
            File.WriteAllText(rutaAbsoluta, contenidoArchivo);
        }
        #endregion
    }
}
