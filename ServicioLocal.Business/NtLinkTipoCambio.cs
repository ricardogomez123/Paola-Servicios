using System;
using System.IO;
using System.Net;

namespace ServicioLocal.Business
{
    public class NtLInkTipoCambio
    {
        public static string GetTipoCambioUsd()
        {
            try
            {
                DateTime fecha = DateTime.Now;
                string Url = "http://dof.gob.mx/indicadores_detalle.php?cod_tipo_indicador=158&dfecha=" +
                             fecha.ToString("dd") + "%2F" + fecha.ToString("MM") + "%2F" + fecha.ToString("yy") +
                             "&hfecha=" + fecha.ToString("dd") + "%2F" + fecha.ToString("MM") + "%2F" + fecha.ToString("yy");
                var wr = (HttpWebRequest)WebRequest.Create(Url);
                var res = (HttpWebResponse)wr.GetResponse();
                var sr = new StreamReader(res.GetResponseStream());
                while (!sr.EndOfStream)
                {
                    string linea = sr.ReadLine();
                    if (linea.Contains("Celda 1")) // Ya la encontre
                    {
                        sr.ReadLine();
                        string correcta = sr.ReadLine();
                        string resultado = correcta.Substring(correcta.IndexOf(">") + 1, correcta.IndexOf("</td") - (correcta.IndexOf(">") + 1));
                        return resultado;
                    }
                }
            }
            catch (Exception ee)
            {

                return null;
            }
            return null;
        }
    }
}
