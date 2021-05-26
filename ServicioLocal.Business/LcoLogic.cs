using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Xml;
using Org.BouncyCastle.Cms;
using ServicioLocalContract;


namespace ServicioLocal.Business
{
    public class LcoLogic : NtLinkBusiness
    {


        private const string LCOFilePath = @"C:\Lco\\";
        readonly string _urlSatLco =
ConfigurationManager.AppSettings["UrlSatLco"];

        public List<LogLco> GetLogLco()
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    return db.LogLco.OrderByDescending(p => p.IdLogLco).Take(5).ToList();
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                return null;
            }
        }

       
        public FoliosCfd SearchFolioSerie(string folio, string serie, string noAprobacion, string anioAprobacion)
        {
            try
            {
                int nFolio = int.Parse(folio);
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var lco =
                        db.FoliosCfd.FirstOrDefault(p => (nFolio >= p.FolioInicial && nFolio <= p.FolioFinal) && p.Serie == serie &&
                                                         p.NoAprobacion == noAprobacion && p.AnoAprobacion == anioAprobacion);
                    return lco;
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                return null;
            }
        }


        public Csd SearchCsdBySerie(string serie, DateTime fecha)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var lco = db.Csd.FirstOrDefault(p => p.no_serie == serie && (fecha >= p.FechaInicial && fecha <= p.FechaFinal) && (p.Estado == "A"));
                    return lco;
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                return null;
            }
        }

      
        public void EjecutarTareaLco()
        {
            try
            {
                var ruta = ConfigurationManager.AppSettings["RutaLco"];
               
                ruta = Path.Combine(ruta, "DescargaLco.exe");
                Process[] pname = Process.GetProcessesByName("DescargaLco");
                if (pname.Length > 0)
                    throw new FaultException("El proceso esta corriendo, por favor espere a que termine");

                Logger.Info("Ejecutando " + ruta);
                if (File.Exists(ruta))
                {
                    ProcessStartInfo psi = new ProcessStartInfo(ruta);
                    psi.Arguments = "Descarga";
                    psi.RedirectStandardOutput = false;
                    psi.WindowStyle = ProcessWindowStyle.Hidden;
                    psi.UseShellExecute = true;
                    Process.Start(psi);
                }
                else
                {
                    throw new FaultException("No se encontró el archivo Lco");
                }
            }
            catch (FaultException fe)
            {
                throw;
            }
            catch (Exception ee)
            {
                Logger.Error(ee, ee);
            }


        }

        //public bool ProcesarArchivoLcoWeb()
        //{
        //    try
        //    {
        //        using (var db = new NtLinkLocalServiceEntities())
        //        {
        //            if (this.ProcesarArchivoLcoSat())
        //            {
        //                db.CommandTimeout = 3600;
        //                db.InsertaLco();
        //                return true;
        //            }
        //            return false;
        //        }
        //    }
        //    catch (Exception ee)
        //    {
        //        Logger.Error(ee.Message);
        //        return false;
        //    }

        //}


        public List<Logger> GetLogsLco()
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var lco = db.Logger.OrderByDescending(p => p.Id).Take(15).ToList();
                    return lco;
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                return null;
            }
        }

        //public bool ProcesarArchivoLcoSat()
        //{
        //    try
        //    {
        //        string strLLCOMasReciente = this.ObtenerLCOMasRecienteFTP();
        //        Logger.Info("Se obtuvo el LCO -> " + strLLCOMasReciente);
        //        string strLArchivoIn = this._urlSatLco + @"/" + strLLCOMasReciente;
        //        const string strLArchivoOut = LCOFilePath + @"\lco_firmado.xml";
        //        Logger.Info("Rutas:\nIn -> " + strLArchivoIn + "\nOut -> " + strLArchivoOut);
        //        Logger.Info("Descargando el archivo IN del SAT");
        //        if (this.PersistLcoFile(strLArchivoIn, strLArchivoOut))
        //        {
        //            if (this.ExcractLcoSignedData())
        //            {
        //                using (var db = new NtLinkLocalServiceEntities())
        //                {
        //                    db.InsertaLco();
        //                    return true;
        //                }
        //            }
        //        }
        //        return false;
        //    }
        //    catch (Exception ee)
        //    {
        //        Logger.Error(ee.Message);
        //        return false;
        //    }
        //}

        #region Helper Methods



        private bool PersistLcoFile(string strPArchivoIn, string strPArchivoOut)
        {
            try
            {
                Logger.Info("Descargando el archivo: " + strPArchivoIn);
                var fwrPeticionFTP =
                    (FtpWebRequest) WebRequest.Create(strPArchivoIn);
                fwrPeticionFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                fwrPeticionFTP.UsePassive = false;
                fwrPeticionFTP.UseBinary = true;
                Logger.Info("Obteniendo el 'Response' del servidor FTP");
                var fwrLRespuestaFTP = (FtpWebResponse) fwrPeticionFTP.GetResponse();
                if (fwrLRespuestaFTP != null)
                {
                    Logger.Info("Obteniendo el 'ResponseStream' del servidor FTP");
                    Stream stmLStreamRespuesta = fwrLRespuestaFTP.GetResponseStream();
                    if (stmLStreamRespuesta != null)
                    {
                        var intLTamañoArchivo = (int) fwrLRespuestaFTP.ContentLength;
                        Logger.Info("Creando el 'BinaryReader' para el FTP");
                        using (var stmLStreamLector = new
                            BinaryReader(stmLStreamRespuesta))
                        {
                            Logger.Info("Creando el 'BinaryWriter'");
                            using (var bwArchivoLCO = new
                                BinaryWriter(File.OpenWrite(strPArchivoOut)))
                            {
                                for (int x = 0; x < intLTamañoArchivo; x += 1024000)
                                {
                                    var r = stmLStreamLector.ReadBytes(1024000);
                                    bwArchivoLCO.Write(r);
                                }
                                Logger.Info("Cerrando los Streams");

                                bwArchivoLCO.Flush();
                                bwArchivoLCO.Close();
                                bwArchivoLCO.Dispose();

                                stmLStreamRespuesta.Close();
                                stmLStreamRespuesta.Dispose();

                                fwrLRespuestaFTP.Close();

                                stmLStreamLector.Close();
                                stmLStreamLector.Dispose();
                            }
                        }
                    }
                }
                Logger.Info("Archivo: " + strPArchivoOut + " guardardo");
            }
            catch (Exception ex)
            {
                Logger.Error(
                    "No se pudo realizar la conexion FTP para descargar el archivo de Certificados \nMensaje: " +
                    (ex.InnerException != null
                         ? ex.InnerException.Message
                         : ex.Message));
                return false;
            }
            return true;
        }

        private bool ExcractLcoSignedData()
        {
            
            Logger.Info("Decodificando el archivo");

            var s = new CmsSignedData(File.ReadAllBytes(LCOFilePath + "lco_firmado.xml"));
            CmsProcessable cp = s.SignedContent;
            byte[] original = (byte[])cp.GetContent(); 
           
            Logger.Info("Cargando el XMLDocument");
            File.WriteAllBytes(LCOFilePath + "lco.xml", original);//, Encoding.ASCII.GetString(sigLFirma.ContentInfo.Content));

            var xmlPDoc = new XmlDocument();


            xmlPDoc.Load(LCOFilePath + "lco.xml");//LoadXml(Encoding.ASCII.GetString(original));//sigLFirma.ContentInfo.Content));

            return this.ParseXmlLco(xmlPDoc);
        }

        private bool ParseXmlLco(XmlDocument xmlLXML)
        {
            using (var smLEscritor = new StreamWriter(LCOFilePath + "lco.xml"))
            {
                try
                {
                    Logger.Info("Extrayendo la informacion del XML");
                    foreach (XmlNode nodoPrincipal in xmlLXML.ChildNodes)
                    {
                        foreach (XmlNode nodoRFCs in nodoPrincipal.ChildNodes)
                        {
                            foreach (XmlAttribute RFC in nodoRFCs.Attributes)
                            {
                                foreach (XmlNode obligaciones in nodoRFCs.ChildNodes)
                                {
                                    string strLLinea = string.Empty;
                                    strLLinea += RFC.Value + "|";
                                    if (obligaciones.Attributes != null)
                                        strLLinea = obligaciones.Attributes.Cast<XmlAttribute>().Aggregate(strLLinea,

                                                                                                           (current, aa)

                                                                                                           =>

                                                                                                           current +

                                                                                                           (aa.Value +

                                                                                                            "|"));

                                    smLEscritor.WriteLine(strLLinea.TrimEnd('|'));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Error al parsear el Xml \nMensaje: " +
                                 (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    throw;
                }
                finally
                {
                    Logger.Info("Cerrando los Streams del archivo .temp");
                    smLEscritor.Flush();
                    smLEscritor.Close();
                }
            }
            Logger.Info("Archivo para BulkInsert creado con exito.");
            return true;
        }

        private string ObtenerLCOMasRecienteFTP()
        {
            Logger.Info("Obtenidno el XML mas reciente de " + _urlSatLco);
            var wrLPeticionFTP = WebRequest.Create(_urlSatLco) as FtpWebRequest;
            Logger.Info("Obteniendo la lista de archivos del SAT");
            wrLPeticionFTP.Method = WebRequestMethods.Ftp.ListDirectory;
            wrLPeticionFTP.UsePassive = false;
            Logger.Info("Obteniendo Response");
            var wrLRespuestaFTP = wrLPeticionFTP.GetResponse();
            Logger.Info("Obteniendo Stream");
            var smLStreamRespuesta = wrLRespuestaFTP.GetResponseStream();
            var smLectorRespuesta = new StreamReader(smLStreamRespuesta);
            var lstLLcos = new List<string>();
            while (!smLectorRespuesta.EndOfStream)
            {
                var strLLinea = smLectorRespuesta.ReadLine();
                if (strLLinea.Contains("LCO"))
                {
                    lstLLcos.Add(strLLinea);
                }
            }
            smLectorRespuesta.Close();
            Logger.Info("Devolvio " + lstLLcos.Count + " archivos, filtrando LCO");

            return lstLLcos.First();
        }

        #endregion
    }
}
