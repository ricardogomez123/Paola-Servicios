using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ServicioLocalContract;
using I_RFC_SAT;


namespace ServicioLocal.Business
{
    public class NtLinkEmpresa : NtLinkBusiness
    {

        private void CreaRutas(string rfc)
        {
            string path = Path.Combine(ConfigurationManager.AppSettings["Resources"], rfc);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!Directory.Exists(Path.Combine(path, "Certs")))
                Directory.CreateDirectory(Path.Combine(path, "Certs"));
            if (!Directory.Exists(Path.Combine(path, "Facturas")))
                Directory.CreateDirectory(Path.Combine(path, "Facturas"));
        }
        //--------------------------------------------
        public bool SaveConfirmacion(ConfirmacionTimbreWs33 ConfirmacionTimbre)
        {
            try
            {
                    using (var db = new NtLinkLocalServiceEntities())
                    {
                        var confirma = db.ConfirmacionTimbreWs33.Where(p => p.Folio == ConfirmacionTimbre.Folio && p.RfcEmisor == ConfirmacionTimbre.RfcEmisor && p.RfcReceptor == ConfirmacionTimbre.RfcReceptor).FirstOrDefault();
                        if (confirma == null)
                        {
                            db.ConfirmacionTimbreWs33.AddObject(ConfirmacionTimbre);

                            db.SaveChanges();
                        }
                    }
                

                return true;
            }
            catch (Exception ee)
            {
                return false;
            }
        }

        public ConfirmacionTimbreWs33 GetConfirmacion(string RFCEmpresa,string RFCCliente,string Folio)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var confirma = db.ConfirmacionTimbreWs33.Where(p => p.Folio == Folio && p.RfcEmisor==RFCEmpresa && p.RfcReceptor==RFCCliente).FirstOrDefault();
                              
                    return confirma;

                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
               
                return null;
            }
        }
        //------------------------------------------------------------
        public bool GetConfirmacionExiste(string confirmacion)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var confirma = db.ConfirmacionTimbreWs33.Where(p => p.Confirmacion ==confirmacion).FirstOrDefault();
                    if (confirma != null)
                        if (confirma.procesado != null)
                            return (bool)confirma.procesado;
                        else
                            return false;
                    else
                        return false;
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);

                return false;
            }
        }
        //------------------------------------------------------------

        
        public List<empresa> GetList(string perfil, int idEmpresa, long idSistema)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    if (perfil.Equals("Administrador"))
                    {
                        var res =
                            db.empresa.Where(p => p.idSistema == idSistema).Select(
                                p =>
                                new
                                    {
                                        RFC = p.RFC,
                                        idSistema = p.idSistema,
                                        IdEmpresa = p.IdEmpresa,
                                        RazonSocial = p.RazonSocial,
                                        TimbresConsumidos = p.TimbresConsumidos,
                                        Linea = p.Linea,
                                        Baja = p.Baja,
                                        Bloqueado = p.Bloqueado
                                    }).OrderBy(p => p.RFC).ToList();
                        return
                            res.Select(
                                p =>
                                new empresa()
                                {
                                    RFC = p.RFC,
                                    idSistema = p.idSistema,
                                    IdEmpresa = p.IdEmpresa,
                                    RazonSocial = p.RazonSocial,
                                    TimbresConsumidos = p.TimbresConsumidos,
                                    Linea = p.Linea,
                                    Baja = p.Baja,
                                    Bloqueado = p.Bloqueado
                                }).ToList();
                    }
                    
                    else
                    {
                        var res =
                            db.empresa.Where(p => p.IdEmpresa == idEmpresa).Select(
                                p =>
                                new
                                    {
                                        RFC = p.RFC,
                                        idSistema = p.idSistema,
                                        IdEmpresa = p.IdEmpresa,
                                        RazonSocial = p.RazonSocial,
                                        TimbresConsumidos = p.TimbresConsumidos,
                                        Linea = p.Linea,
                                        Baja = p.Baja,
                                        Bloqueado = p.Bloqueado
                                    }).OrderBy(p => p.RFC).ToList();
                        return res.Select(
                                p =>
                                new empresa()
                                {
                                    RFC = p.RFC,
                                    idSistema = p.idSistema,
                                    IdEmpresa = p.IdEmpresa,
                                    RazonSocial = p.RazonSocial,
                                    TimbresConsumidos = p.TimbresConsumidos,
                                    Linea = p.Linea,
                                    Baja = p.Baja,
                                    Bloqueado = p.Bloqueado
                                }).ToList();
                    }
                        
                }

            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                return null;
            }

        }

        public List<empresa> GetListForLine(string Linea)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    if (Linea == null)
                    {
                        Linea = "A";
                    }
                    if (Linea != null)
                    {
                        var res =
                            db.empresa.Where(p => p.Linea == Linea).Select(
                                p =>
                                new
                                    {
                                        p.RFC,
                                        p.idSistema, 
                                        p.IdEmpresa,
                                        p.RazonSocial,
                                        p.TimbresConsumidos,
                                        Linea,
                                        p.Baja,
                                        p.Bloqueado
                                    }).OrderBy(p => p.RFC).ToList();
                        return
                            res.Select(
                                p =>
                                new empresa()
                                    {
                                        RFC = p.RFC,
                                        idSistema = p.idSistema,
                                        IdEmpresa = p.IdEmpresa,
                                        RazonSocial = p.RazonSocial,
                                        TimbresConsumidos = p.TimbresConsumidos,
                                        Linea = p.Linea,
                                        Baja = p.Baja,
                                        Bloqueado = p.Bloqueado
                                    }).ToList();
                    }
                    else
                    {
                        return db.empresa.Select(p => new empresa() { RFC = p.RFC, idSistema = p.idSistema, IdEmpresa = p.IdEmpresa, RazonSocial = p.RazonSocial, TimbresConsumidos = p.TimbresConsumidos, Linea = p.Linea, Baja = p.Baja, Bloqueado = p.Bloqueado }).OrderBy(p => p.RFC).ToList();
                    }
                }

            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                return null;
            }

        }
        /*
        public empresa GetByUID(string uid)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var fac = db.TimbreWs33.FirstOrDefault(p => p.Uuid == uid);
                    if (fac != null)
                    {
                        var empr = db.empresa.FirstOrDefault(p => p.IdEmpresa == fac.IdEmpresa);

                        return empr;
                    }
                    return null;
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return null;
            }
        }
        //-------------------------------------------------------------------------------------
       */

        public empresa GetById(int idEmpresa)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var empr = db.empresa.Where(p => p.IdEmpresa == idEmpresa).FirstOrDefault();
                    if (empr != null)
                    {
                        string path = Path.Combine(ConfigurationManager.AppSettings["Resources"], empr.RFC);
                        string cer = Path.Combine(path, "Certs", "csd.cer");
                        if (File.Exists(cer))
                        {
                            X509Certificate2 cert = new X509Certificate2(cer);
                            empr.VencimientoCert = cert.GetExpirationDateString();
                        }

                    }
                    return empr;

                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                return null;
            }
        }


        public empresa GetByRfc(string rfc)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var empr = db.empresa.FirstOrDefault(p => p.RFC == rfc);
                    if (empr != null)
                    {
                        string path = Path.Combine(ConfigurationManager.AppSettings["Resources"], empr.RFC);
                        string cer = Path.Combine(path, "Certs", "csd.cer");
                        if (File.Exists(cer))
                        {
                            X509Certificate2 cert = new X509Certificate2(cer);
                            empr.VencimientoCert = cert.GetExpirationDateString();
                        }

                    }
                    return empr;
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                if (ee.InnerException !=null)
                    Logger.Error(ee.InnerException);
                return null;
            }
        }
        //-------------------------------------------------------------------------------------
        public bool GetByRfcMultiple(string rfc)///para verificar si existen mas de una empresa con el RFC
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var empr = db.empresa.Where(p => p.RFC == rfc).ToList();
                    if (empr != null)
                    {
                        if (empr.Count > 1)
                            return true;
                        else
                            return false;
                    }
                    return false;
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return false;
            }
        }

        /*
        public empresa GetByRfcMultipleUsuario(string usuario)//obtiene la empresa por medio del usuario
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    Sistemas sist = db.Sistemas.FirstOrDefault(c => c.Email == usuario);
                    var empresa = db.empresa.FirstOrDefault(e => e.idSistema == sist.IdSistema);
                    return empresa;
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return null;
            }
        }*/

        //-------------------------------------------------------------------------------------
        private bool Validar(empresa e)
        {
            //TODO: Validar los campos requeridos y generar excepcion
            {
                if (string.IsNullOrEmpty(e.RazonSocial))
                {

                    throw new FaultException("La Razón Social no puede ir vacía");
                }
                if (string.IsNullOrEmpty(e.RFC))
                {
                    throw new FaultException("El RFC no puede ir vacío");

                }
                if (string.IsNullOrEmpty(e.Email))
                {
                    throw new FaultException("El campo Email es Obligatorio");
                }
                if (string.IsNullOrEmpty(e.Direccion))
                {
                    throw new FaultException("El campo Calle es Obligatorio");
                }
                if (string.IsNullOrEmpty(e.Ciudad))
                {
                    throw new FaultException("El campo Municipio es Obligatorio");
                }
                if (string.IsNullOrEmpty(e.Estado))
                {
                    throw new FaultException("El campo Estado es Obligatorio");
                }
                if (string.IsNullOrEmpty(e.RegimenFiscal))
                {
                    throw new FaultException("El campo Regimen Fiscal es Obligatorio");
                }
                if (string.IsNullOrEmpty(e.CP))
                {
                    throw new FaultException("El campo Codigo Postal es Obligatorio");
                }
                Regex regex =
                    new Regex(
                        @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*([,;]\s*\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)*");
                if (!regex.IsMatch(e.Email))
                {
                    throw new FaultException("El campo Email esta mal formado");
                }
                Regex reg = new Regex("^[A-Z,Ñ,&amp;]{3,4}[0-9]{2}[0-1][0-9][0-3][0-9][A-Z,0-9]{2}[0-9,A]$");
                if (!reg.IsMatch(e.RFC))
                {
                    throw new FaultException("El RFC es inválido");
                }

                //LcoLogic lco = new LcoLogic();
                Operaciones_IRFC lco = new Operaciones_IRFC();
                var rfcLco = lco.SearchLCOByRFC(e.RFC);
                if (rfcLco == null)
                {
                    throw new FaultException("El Rfc no se encuentra en la lista de contribuyentes con obligaciones");
                }
            }
            return true;
        }

        public bool Save(empresa e, byte[] cert, byte[] llave, string passwordLlave, byte[] logo)
        {

            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                    {
                    if (Validar(e))
                    {
                        CreaRutas(e.RFC);
                        string path = Path.Combine(ConfigurationManager.AppSettings["Resources"], e.RFC);
                        if (logo != null)
                        {
                            File.WriteAllBytes(Path.Combine(path, "Logo.png"), logo);
                        }
                        if (!ValidaRfcEmisor(e.RFC, cert))
                        {
                            throw new FaultException("El rfc del emisor no corresponde con el certificado");
                        }
                        string pathCer = Path.Combine(path, "Certs", "csd.cer");
                        string pathKey = Path.Combine(path, "Certs", "csd.key");
                        File.WriteAllBytes(pathCer, cert);
                        File.WriteAllBytes(pathKey, llave);
                        //CertUtil.CreaP12(pathKey, pathCer, passwordLlave, Path.ChangeExtension(pathCer, ".p12"));
                        if (e.IdEmpresa == 0)
                        {
                            if (db.empresa.Any(l => l.RFC.Equals(e.RFC) && l.idSistema == e.idSistema))
                            {
                                throw new FaultException("El RFC ya ha sido dato de alta");
                            }
                            db.empresa.AddObject(e);
                        }
                        else
                        {
                            db.empresa.Where(p => p.IdEmpresa == e.IdEmpresa).FirstOrDefault();
                            db.empresa.ApplyCurrentValues(e);
                        }
                        db.SaveChanges();
                        return true;
                    }
                    Logger.Error("Fallo de validación");
                    return false;
                }
            }
            catch (ApplicationException ae)
            {
                throw new FaultException(ae.Message);
            }
            catch (FaultException fe)
            {
                throw;
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                return false;
            }
        }


        public bool ValidaRfcEmisor(string rfc, byte[] certificado)
        {
            try
            {
                X509Certificate2 cer = new X509Certificate2(certificado);
                if (certificado == null)
                    return false;
                var name = cer.SubjectName.Name;
                name = name.Replace("\"", "");
                string strLRfc =
                    name.Substring(name.LastIndexOf("2.5.4.45=") + 9, 13).Trim();
                return strLRfc == rfc;
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                return false;
            }
        }


        public long ObtenerNumeroTimbres(int idEmpresa)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    var res = db.TimbradoEmpresaMensual.Where(p => p.IdEmpresa == idEmpresa).Sum(p=>p.Timbres);
                    return res;
                    
                }

            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return 0;
            }


        }


        public bool Save(empresa e, byte[] logo)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    if (Validar(e))
                    {
                        if (e.IdEmpresa == 0)
                        {
                            if (db.empresa.Any(l => l.RFC.Equals(e.RFC) && l.idSistema == e.idSistema))
                            {
                                throw new FaultException("El RFC ya ha sido dato de alta");
                            }
                            db.empresa.AddObject(e);
                        }
                        else
                        {
                            db.empresa.FirstOrDefault(p => p.IdEmpresa == e.IdEmpresa);
                            db.empresa.ApplyCurrentValues(e);
                        }
                        db.SaveChanges();
                        CreaRutas(e.RFC);
                        
                        string path = Path.Combine(ConfigurationManager.AppSettings["Resources"], e.RFC);
                        if (logo != null)
                        {
                            File.WriteAllBytes(Path.Combine(path, "Logo.png"), logo);
                        }
                        if(e.Baja!=true)
                        throw new FaultException("El RFC ha sido dato de alta");//para que retorne algo si fue exitoso
                        else
                         throw new FaultException("El RFC ha sido dato de Baja");//para que retorne algo si fue exitoso
                      
                        return true;

                    }
                    return false;
                }
            }
            catch (ApplicationException ae)
            {
                throw new FaultException(ae.Message);
            }
            catch (FaultException fe)
            {
                throw;
            }
            catch (Exception ee)
            {
                Logger.Error(ee.Message);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException.Message);
                return false;
            }
        }

        public bool TieneConfiguradoCertificado(int idEmpresa)
        {
            using (var db = new NtLinkLocalServiceEntities())
            {
                empresa emp = db.empresa.Single(l => l.IdEmpresa == idEmpresa);

                string path = Path.Combine(ConfigurationManager.AppSettings["Resources"], emp.RFC);
                string pathCer = Path.Combine(path, "Certs", "csd.cer");
                string pathKey = Path.Combine(path, "Certs", "csd.key");

                if (File.Exists(pathCer) && File.Exists(pathKey))
                {
                    return true;
                }
                return false;
            }
        }

        public List<empresaPantalla> GetPantallas(int idEmpresa)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    return db.empresaPantalla.Where(l => l.idEmpresa == idEmpresa).ToList();
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return null;
            }

        }

        public bool SavePantallas(List<empresaPantalla> pantallas)
        {
            try
            {
                if (pantallas != null && pantallas.Count >0 )
                {
                    int idEmpresa = pantallas.First().idEmpresa;
                    using (var db = new NtLinkLocalServiceEntities())
                    {
                        List<empresaPantalla> currPantallas =
                            db.empresaPantalla.Where(l => l.idEmpresa == idEmpresa).ToList();

                        foreach (var pantalla in currPantallas)
                        {
                            db.empresaPantalla.DeleteObject(pantalla);
                        }

                        foreach (var pantalla in pantallas)
                        {
                            db.empresaPantalla.AddObject(pantalla);
                        }

                        db.SaveChanges();
                    }
                }
                
                return true;
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                if (ee.InnerException != null)
                    Logger.Error(ee.InnerException);
                return false;
            }
        }

        public string ValidaCSD(empresa e, byte[] cert, byte[] llave, string passwordLlave)
        {
            try
            {
                using (var db = new NtLinkLocalServiceEntities())
                {
                    CreaRutas(e.RFC);
                    string path = Path.Combine(ConfigurationManager.AppSettings["Resources"], e.RFC);
                    if (!ValidaRfcEmisor(e.RFC, cert))
                    {
                        return "El RFC del emisor no corresponde con el certificado";
                    }
                    if (!ValidaCSDEmisor(cert))
                    {
                        return "El Certificado no es de tipo CSD";
                    }
                    
                        string pathCer = Path.Combine(path, "Certs", "csd.cer");
                        string pathKey = Path.Combine(path, "Certs", "csd.key");
                        File.WriteAllBytes(pathCer, cert);
                        File.WriteAllBytes(pathKey, llave);
                        //bool result = CertUtil.CreaP12l(pathKey, pathCer, passwordLlave, Path.ChangeExtension(pathCer, ".p12"));
                        //if (result != false)
                        return "El Certificado CSD  es correcto";
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                return "";
            }
            return "El Password de la llave no es correcta";
        }

        public bool ValidaCSDEmisor(byte[] certificado)
        {
            try
            {
                X509Certificate2 cer = new X509Certificate2(certificado);
                if (certificado != null)
                {
                    if (cer.SerialNumber != null)
                    {
                        string serialNumber =
                            cer.SerialNumber.Trim();
                        string a = serialNumber;
                        for (int i = 0; i < a.Length; i++)
                        {
                            if (i < serialNumber.Length)
                            {
                                serialNumber = serialNumber.Remove(i, 1);
                            }
                        }
                        Operaciones_IRFC o = new Operaciones_IRFC();
                        vLCO valor= o.CSDValidar(serialNumber);
                        //RfcLco valor = CSDValidar(serialNumber);
                        if (valor != null)
                        return true;
                    }
                }
            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                return false;
            }
            return false;
        }

        //public RfcLco CSDValidar(string cadena)
        //{
        //    try
        //    {
        //        using (var db = new NtLinkLocalServiceEntities())
        //        {
        //            return db.RfcLco.FirstOrDefault(p => p.noCertificado == cadena);
        //        }
        //    }
        //    catch (Exception ee)
        //    {
        //        Logger.Error(ee);
        //        if (ee.InnerException != null)
        //            Logger.Error(ee.InnerException);
        //        return null;
        //    }

        //}
    }
}
