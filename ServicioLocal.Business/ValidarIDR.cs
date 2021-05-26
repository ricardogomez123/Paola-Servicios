

using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using MessagingToolkit.QRCode.Codec;
using log4net;
using log4net.Config;
using System;
using System.Text.RegularExpressions;
using ServicioLocal.catCFDI;
using System.Security.Cryptography.X509Certificates;
using I_RFC_SAT;
using System.Security.Cryptography;
using Org.BouncyCastle.X509;
using ServicioLocalContract;


namespace ServicioLocal.Business
{
    public class ValidarIDR : NtLinkBusiness
    {
   

        public ValidarIDR()
        {
            XmlConfigurator.Configure();
        }
        //---------------------------------------------------------------------------------------------
        public string ProcesarIDR(IdentificacionDeRecurso I, string tipoDeComprobante, string version)
        {
            try
            {

                if (version != "3.3")
                    return ("IRMGCT101 - El atributo Version no tiene un valor válido.");

                if (tipoDeComprobante != "I")
                    return ("IRMGCT102 - El atributo TipoDeComprobante no cumple con el valor permitido.");

                if (I.TipoOperacion == "a")
                {
                    if (I.IdentificacionDelGasto != null)
                        if (I.IdentificacionDelGasto.Count() > 1)
                            return ("IRMGCT103 - El elemento \"IdentificacionDelGasto\" no se debe registrar");
                    if (I.DispersionDelRecurso == null)
                        if (I.DispersionDelRecurso.Count() == 0)
                            return ("IRMGCT104 - El elemento \"DispersionDelRecurso\" se debe registrar");
                }

                if (I.TipoOperacion == "b")
                {
                    if (I.DispersionDelRecurso != null)
                        if (I.DispersionDelRecurso.Count() > 1)
                            return ("IRMGCT105 - El elemento \"IDispersionDelRecurso\" no se debe registrar");
                    if (I.IdentificacionDelGasto == null)
                        if (I.IdentificacionDelGasto.Count() == 0)
                            return ("IRMGCT106 - El elemento \"IdentificacionDelGasto\" se debe registrar");

                }

                if (I.RemanenteSpecified == true)
                    if (I.ReintegroRemanenteSpecified == false)
                        return ("IRMGCT107 - El atributo \"ReintegroRemanente\" debe registrarse porque existe el atributo \"Remanente\".");
                if (I.ReintegroRemanenteSpecified == true)
                    if (I.ReintegroRemanFechaSpecified == false)
                        return ("IRMGCT108 - El atributo \"ReintegroRemanFecha\" debe registrarse porque existe el atributo \"ReintegroRemanente\".");

                if (I.IdentificacionDelGasto != null)
                    if (I.IdentificacionDelGasto.Count() > 0)
                    {
                        foreach (var id in I.IdentificacionDelGasto)
                        {
                            if (!string.IsNullOrEmpty(id.NumFolioDoc))
                                if (id.FechaDeGastoSpecified == false)
                                    return ("IRMGCT109 - El atributo \"FechaDeGasto\" debe registrarse por que existe el atributo \"NumFolioDoc\".");

                            if (!string.IsNullOrEmpty(id.NumFolioDoc))
                                if (id.ImporteGastoSpecified == false)
                                    return ("IRMGCT110 - El atributo \"ImporteGasto\" debe registrarse por que existe el atributo \"NumFolioDoc\".");


                        }
                    }


                return "0";
            }
            catch (Exception ee)
            {
                Logger.Error("", ee);
                return ("IRMGCT999 - Error no clasificado");

            }

        }
        //-------------------------------------------------------------------------------------
        public int ValidaRFCLCO(string rfc)
        {
            try
            {
                //var lcoLogic = new LcoLogic();
                var lcoLogic = new Operaciones_IRFC();
                vLCO lco = lcoLogic.SearchLCOByRFC(rfc);
                return lco == null ? 402 : 0;

            }
            catch (Exception ee)
            {
                Logger.Error("", ee);
                return 402;
            }

        }



    }

}