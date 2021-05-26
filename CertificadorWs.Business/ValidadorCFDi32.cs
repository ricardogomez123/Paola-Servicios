using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using BusinessLogic;
using ServicioLocal.Business;
using System.Xml;
using System.Configuration;
using ServicioLocalContract;
using log4net.Config;
using System.Text.RegularExpressions;
using ServicioLocal.catCFDI;
using System.Globalization;
using I_RFC_SAT;
 

namespace CertificadorWs.Business
{
    public class ValidadorCFDi32 : NtLinkBusiness
    {
        //public static void Main()
        //{
        //    while (true)
        //    {
        //        var val = new ValidadorCFDi32();
        //        string res = null;
        //        val.ProcesarCadena(File.ReadAllText(@"C:\10_19.xml", Encoding.UTF8), ref res);
        //        Console.WriteLine(res);
        //    }
        //}

        private X509Certificate2 _certificado;
        private readonly SerializadorTimbres _serializadorTimbres = new SerializadorTimbres();
        private static GeneradorCadenasCfd _genCadenas;
        private static ValidadorEstructura _validadorEstructura;

        public ValidadorCFDi32()
        {
            XmlConfigurator.Configure();
            if (_genCadenas == null)
                _genCadenas = new GeneradorCadenasCfd();
            if (_validadorEstructura == null)
                _validadorEstructura = new ValidadorEstructura(true);
            //Directory.CreateDirectory("..\\ErroresValidacion");
        }

        //public Dictionary<int, string> ProcesarArchivo(string inputFile, ref string acuseSat)
        //{
        //    try
        //    {
        //        //var info = new FileInfo(inputFile);
        //        Logger.Info("");
        //        XElement xe = XElement.Load(inputFile);
        //        XElement addenda = xe.Elements(Constantes.CFDVersionNamespace + "Addenda").FirstOrDefault();
        //        if (addenda != null)
        //        {
        //            addenda.Remove();
        //        }
        //        string res = null;
        //        TimbreFiscalDigital t = null;
        //        var x =  this.Procesar(xe, inputFile, ref res, ref t, ref acuseSat);
        //        Logger.Debug(x);
        //        return x;
        //    }
        //    catch(Exception ex)
        //    {
        //        Logger.Error("", ex);
        //        return this.CrearArchivoROE(new List<int> { 666 }, "WS", ex.Message);
        //    }
        //}
        private readonly string rutaEntrada = ConfigurationManager.AppSettings["rutaEntrada"];
        public Dictionary<int, string> ProcesarCadena(string RFC, string cadenaXml, ref string res, ref TimbreFiscalDigital timbre, ref string acuseSat, ref string hash)
        {
            try
            {
                string ruta = Path.Combine(rutaEntrada, DateTime.Now.Year.ToString(),
                                          DateTime.Now.Month.ToString().PadLeft(2,'0'),
                                          DateTime.Now.Day.ToString().PadLeft(2,'0'));
                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }
                Guid uid = Guid.NewGuid();
                string archivo = Path.Combine(ruta, uid.ToString() + ".xml");
                File.WriteAllText(archivo, cadenaXml,Encoding.UTF8);
                XElement xe = XElement.Load(new StringReader(cadenaXml));
                XElement addenda = xe.Elements(Constantes.CFDVersionNamespace + "Addenda").FirstOrDefault();
                if (addenda != null)
                {
                    addenda.Remove();
                }
                acuseSat = "";
                var x = this.Procesar(RFC,xe, "WS",addenda,uid, ref res, ref timbre, ref acuseSat, ref hash);
                Logger.Debug(x);
                return x;
            }
            catch (Exception eee)
            {
                Logger.Error(eee);
                return this.CrearArchivoROE(new List<int> { 666 }, "WS", eee.Message);
                // return false;
            }
        }
        private bool ValidaDigitoControl(string Clabe)
        {
            try
            {
                string s = Clabe.Substring(17);
                Clabe = Clabe.Remove(Clabe.Length - 1);//18 digitos menos uno para validarlo
                int[] factor = { 3, 7, 1 };
                int j = 0; int m = 0; int sum = 0;

                int[] A = Clabe.ToCharArray().Select(x => (int)Char.GetNumericValue(x)).ToArray();

                for (int i = 0; i < Clabe.Length; i++)
                {
                    if (j == 3)
                        j = 0;

                    m = A[i] * factor[j++];
                    if (m >= 10)
                        m = m % 10;
                    sum += m;
                }
                sum = sum % 10;
                sum = 10 - sum;
                if (sum == 10)
                    sum = 0;
                if (sum.ToString() == s)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; }
        }

        public string ProcesarNomina(List<Nomina> nomina, Comprobante comp)
        {
            
            try

            {

                if (nomina == null)
                    return ("NOM150 - El nodo Nomina no se puede utilizar dentro del elemento ComplementoConcepto");
                if (nomina.Count == 0)
                    return ("NOM150 - El nodo Nomina no se puede utilizar dentro del elemento ComplementoConcepto");

                decimal TotalOtrosPagos = 0;
                decimal TotalPercepciones = 0;
                decimal TotalDeducciones = 0;
                bool TotalDeduccionesbool = false;

                if (nomina.Count > 1) //para varias nominas
                {

                    TotalDeducciones = 0;
                    foreach (Nomina nom in nomina)
                    {
                        if (nom.TotalOtrosPagosSpecified == true)
                            TotalOtrosPagos = TotalOtrosPagos + nom.TotalOtrosPagos;
                        if (nom.TotalPercepcionesSpecified == true)
                            TotalPercepciones = TotalPercepciones + nom.TotalPercepciones;
                        if (nom.TotalDeduccionesSpecified == true)
                        {
                            TotalDeducciones = TotalDeducciones + nom.TotalDeducciones;
                            TotalDeduccionesbool = true;
                        }
                        if(nom.Receptor!=null)
                        if(nom.Receptor.TipoRegimen!="02" &&nom.Receptor.TipoRegimen!="13" )
                            return ("NOM225 - El nodo Nomina no se puede repetir para los tipo de regimen 02 y 13.");

                    }
                }
                else
                {

                    if (nomina[0].TotalOtrosPagosSpecified == true)
                        TotalOtrosPagos = nomina[0].TotalOtrosPagos;
                    if (nomina[0].TotalPercepcionesSpecified == true)
                        TotalPercepciones = nomina[0].TotalPercepciones;
                    if (nomina[0].TotalDeduccionesSpecified == true)
                    {
                        TotalDeducciones = nomina[0].TotalDeducciones;
                        TotalDeduccionesbool=true;

                    }
                }


                foreach (Nomina nom in nomina)
                {

                    //***********************************Valida CFDI 3.2*********************************************************
                    /*
                     if (!string.IsNullOrEmpty(comp.Fecha))
                     {
                  

                         //fecha = fecha.Replace("fecha=", "");
                         //fecha = fecha.Replace("\"", "");
                         Regex regex7 = new Regex(@"(20[1-9][0-9])-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])T(([01][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9])");
                         if (!regex7.IsMatch(comp.Fecha))
                             return ("NOM101 - El atributo fecha no cumple con el patrón requerido.");
                     }
                     //if (comp.MetodoPago.ToString() != "NA")
                     //    return ("NOM102 - El atributo metodoDePago debe tener el valor NA.");
                     if (comp.NoCertificado != null)
                     {
                         Regex regex3 = new Regex(@"[0-9]{20}");
                         if (!regex3.IsMatch(comp.NoCertificado))
                             return ("NOM103 - El atributo noCertificado no cumple con el patrón requerido.");
                     }
                        if(comp.Moneda!="MXN")
                            return ("NOM104 - El atributo Moneda debe tener el valor MXN.");
                     if(comp.TipoCambioSpecified==true)
                         if(comp.TipoCambio!=1)
                             return ("NOM105 - El atributo TipoCambio no tiene el valor = 1.");
                     if(comp.SubTotal!=(nom.TotalPercepciones+nom.TotalOtrosPagos))
                         return ("NOM106 - El valor del atributo subTotal no coincide con la suma de Nomina12:TotalPercepciones más Nomina12:TotalOtrosPagos.");
                     if(comp.Descuento!=nom.TotalDeducciones)
                         return ("NOM107 - El valor de descuento no es igual a Nomina12:TotalDeducciones.");
                     // Get current culture's NumberFormatInfo object.
                     NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;
                     string decimalSeparator = nfi.CurrencyDecimalSeparator;

                    */
                    if (comp.Emisor != null)
                    {
                        /*
                        if(comp.Emisor.RegimenFiscal!=null)
                        if(comp.Emisor.RegimenFiscal.Count()>1)
                               return ("NOM117 - Solo debe existir un solo nodo RegimenFiscal.");

                       if (comp.Emisor.Rfc.Length == 12)
                       {
                           if (nom.Emisor != null)
                               if (!string.IsNullOrEmpty(nom.Emisor.Curp))
                                   return ("NOM113 - El atributo Nomina12:Emisor:Curp. no aplica para persona moral.");

                       
                       }
                       if (comp.Emisor.Rfc.Length == 13)
                       {
                           if (nom.Emisor != null)
                           {
                               if (string.IsNullOrEmpty(nom.Emisor.Curp))
                                   return ("NOM114 - El atributo Nomina12:Emisor:Curp. Debe aplicar para persona física.");
                           }else
                               return ("NOM114 - El atributo Nomina12:Emisor:Curp. Debe aplicar para persona física.");
                       
                       }
                   */
                        //-----------------rfc------------------

                        if (!string.IsNullOrEmpty(comp.Emisor.Rfc))
                        {
                            I_RFC_SAT.Operaciones_IRFC r = new I_RFC_SAT.Operaciones_IRFC();
                            vI_RFC t = r.Consultar_IRFC(comp.Emisor.Rfc);
                            if (t != null)
                            {
                                /*
                                if (t.SUBCONTRATACION == "1")
                                {
                                    if (nom.Receptor.SubContratacion == null)
                                        return ("NOM115 - El nodo Subcontratacion se debe registrar.");
                                    if (nom.Receptor.SubContratacion.Count < 1)
                                        return ("NOM115 - El nodo Subcontratacion se debe registrar.");


                                }
                                 */
                                if (t.SNCF == "1")
                                {
                                    if (nom.Emisor == null)
                                        return ("NOM165 - El nodo Nomina.Emisor.EntidadSNCF debe existir.");
                                    if (nom.Emisor.EntidadSNCF == null)
                                        return ("NOM165 - El nodo Nomina.Emisor.EntidadSNCF debe existir.");

                                }
                                else
                                {
                                    if (nom.Emisor != null)
                                        if (nom.Emisor.EntidadSNCF != null)
                                            return ("NOM166 - El nodo Nomina.Emisor.EntidadSNCF no debe existir.");
                                }
                            }

                        }
                        /*
                            if (!string.IsNullOrEmpty(comp.Receptor.Rfc))
                            {
                                I_RFC_SAT.Operaciones_IRFC r = new I_RFC_SAT.Operaciones_IRFC();
                                if(!r.inscrito(comp.Receptor.Rfc))
                                    return ("NOM122 - El atributo cfdi:Comprobante.Receptor.rfc no es válido según la lista de RFC inscritos no cancelados en el SAT (l_RFC).");
                            }
                        */
                        if (nom.Emisor != null)
                            if (!string.IsNullOrEmpty(nom.Emisor.RfcPatronOrigen))
                            {
                                I_RFC_SAT.Operaciones_IRFC r = new I_RFC_SAT.Operaciones_IRFC();
                                if (!r.inscrito(nom.Emisor.RfcPatronOrigen))
                                    return ("NOM161 - El atributo Nomina.Emisor.RfcPatronOrigen no está inscrito en el SAT (l_RFC).");

                            }
                        if (nom.Receptor.SubContratacion != null)
                        {

                            if (nom.Receptor.SubContratacion.Count > 0)
                            {

                                foreach (NominaReceptorSubContratacion sub in nom.Receptor.SubContratacion)
                                {

                                    if (!string.IsNullOrEmpty(sub.RfcLabora))
                                    {
                                        if (sub.RfcLabora != "XEXX010101000")//rfc generico
                                        {
                                            I_RFC_SAT.Operaciones_IRFC r = new I_RFC_SAT.Operaciones_IRFC();
                                            if (!r.inscrito(sub.RfcLabora))
                                                return ("NOM187 - El valor del atributo Nomina.Receptor.SubContratacion.RfcLabora no está en la lista de RFC (l_RFC).");
                                        }
                                    }
                                }

                            }
                        }



                    }



                    /*
                    if (comp.Receptor != null)
                    { 
                       if(comp.Receptor.Rfc.Length!=13)
                           return ("NOM121 - El atributo cfdi:Comprobante.Receptor.rfc debe ser persona física (13 caracteres).");
               
                    }
                    if (comp.Conceptos != null)
                    { 
                      if(comp.Conceptos.Count()!=1)
                          return ("NOM124 - El nodo concepto solo debe existir uno, sin elementos hijo.");
               
                    }
                    if (comp.Version == "3.2")
                    { 
                      if(!string.IsNullOrEmpty(comp.Conceptos[0].NoIdentificacion))
                          return ("NOM125 - El atributo noIdentificacion no debe existir.");
                      if(comp.Conceptos[0].Cantidad!=1)
                          return ("NOM126 - El atributo cfdi:Comprobante.Conceptos.Concepto.cantidad no tiene el valor =  “1”.");
                      if (comp.Conceptos[0].Unidad != "ACT")
                          return ("NOM127 - El atributo cfdi:Comprobante.Conceptos.Concepto.unidad no tiene el valor =  “ACT”.");
                      if (comp.Conceptos[0].Descripcion != "Pago de nómina")
                          return ("NOM128 - El atributo cfdi:Comprobante.Conceptos.Concepto.descripcion,  no tiene el valor “Pago de nómina”.");
                      if (comp.Conceptos[0].ValorUnitario != (nom.TotalOtrosPagos+nom.TotalPercepciones))
                          return ("NOM129 - El valor del atributo.cfdi:Comprobante.Conceptos.Concepto.valorUnitario no coincide con la suma TotalPercepciones más TotalOtrosPagos.");
                      if (comp.Conceptos[0].Importe != (nom.TotalOtrosPagos + nom.TotalPercepciones))
                          return ("NOM130 - El valor del atributo.cfdi:Comprobante.Conceptos.Concepto.Importe no coincide con la suma TotalPercepciones más TotalOtrosPagos.");
           

                    }

                    if (comp.Impuestos != null)
                    {
                        if(comp.Impuestos.Retenciones!=null ||comp.Impuestos.TotalImpuestosRetenidosSpecified==true
                            || comp.Impuestos.TotalImpuestosTrasladadosSpecified==true || comp.Impuestos.Traslados!=null)
                        return ("NOM131 - El nodo cfdi:Comprobante.Impuestos no cumple la estructura.");
                    }
                    */
                    //************************************cfdi3.3*********************************************************
                    if (comp.Version == "3.3")
                    {
                        if (comp.Moneda != "MXN")
                            return ("NOM132 - El atributo Moneda no tiene el valor =  “MXN”.");
                        if (comp.FormaPago != "99")
                            return ("NOM133 - El atributo FormaPago no tiene el valor =  99.");

                        if (comp.TipoDeComprobante != "N")
                            return ("NOM134 - El atributo TipoDeComprobante no tiene el valor =  N.");

                        if (!string.IsNullOrEmpty(comp.Emisor.Rfc))
                        {
                            if (comp.Emisor.Rfc.Count() == 12)
                            {
                                if (nom.Emisor != null)
                                    if (!string.IsNullOrEmpty(nom.Emisor.Curp))
                                        return ("NOM135 - El atributo Nomina12:Emisor:Curp, no aplica para persona moral.");

                            }
                            if (comp.Emisor.Rfc.Count() == 13)
                            {
                                if (nom.Emisor != null)
                                    if (string.IsNullOrEmpty(nom.Emisor.Curp))
                                        return ("NOM136 - El atributo Nomina12:Emisor:Curp, debe aplicar para persona fisica.");

                            }

                        }

                        //if (!string.IsNullOrEmpty(comp.Receptor.Rfc))
                        //{
                        //    if (comp.Receptor.Rfc.Count() != 13)
                        //        return ("NOM137 - El atributo Comprobante.Receptor.rfc, debe ser de longitud 13.");

                        //    if (comp.Receptor.Rfc != "XAXX010101000") // nuevo par los trabajadores que estan muertos..
                        //    {
                        //        I_RFC_SAT.Operaciones_IRFC r = new I_RFC_SAT.Operaciones_IRFC();
                        //        if (!r.inscrito(comp.Receptor.Rfc))
                        //            return ("NOM138 - El atributo Comprobante.Receptor.rfc, no está en la lista de RFC inscritos no cancelados en el SAT (l_RFC).");
                        //    }
                        //}

                        if (comp.Impuestos != null)
                            return ("NOM149 - El nodo Comprobante.Impuestos, no debe existir.");

                        if (comp.Conceptos != null)
                        {
                            if (comp.Conceptos.Count() != 1)
                                return ("NOM139 - El nodo Comprobante.Conceptos.Concepto, Solo puede registrarse un nodo concepto, sin elementos hijo.");

                            foreach (var conp in comp.Conceptos)
                            {
                                if (conp.Impuestos != null)
                                    return ("NOM139 - El nodo Comprobante.Conceptos.Concepto, Solo puede registrarse un nodo concepto, sin elementos hijo.");
                                if (conp.ComplementoConcepto != null)
                                    return ("NOM139 - El nodo Comprobante.Conceptos.Concepto, Solo puede registrarse un nodo concepto, sin elementos hijo.");
                                if (conp.CuentaPredial != null)
                                    return ("NOM139 - El nodo Comprobante.Conceptos.Concepto, Solo puede registrarse un nodo concepto, sin elementos hijo.");
                                if (conp.InformacionAduanera != null)
                                    return ("NOM139 - El nodo Comprobante.Conceptos.Concepto, Solo puede registrarse un nodo concepto, sin elementos hijo.");
                                if (conp.Parte != null)
                                    return ("NOM139 - El nodo Comprobante.Conceptos.Concepto, Solo puede registrarse un nodo concepto, sin elementos hijo.");


                                if (conp.ClaveProdServ != "84111505")
                                    return ("NOM140 - El atributo Comprobante.Conceptos.Concepto,ClaveProdServ no tiene el valor =  “84111505”.");

                                if (!string.IsNullOrEmpty(conp.NoIdentificacion))
                                    return ("NOM141 - El atributo Comprobante.Conceptos.Concepto.NoIdentificacion, no debe existir.");
                                if (conp.Cantidad != 1)
                                    return ("NOM142 - El atributo Comprobante.Conceptos.Concepto,Cantidad no tiene el valor =  “1”.");
                                if (conp.ClaveUnidad != "ACT")
                                    return ("NOM143 - El atributo Comprobante.Conceptos.Concepto,ClaveUnidad no tiene el valor =  “ACT”.");
                                if (!string.IsNullOrEmpty(conp.Unidad))
                                    return ("NOM144 - El atributo Comprobante.Conceptos.Concepto,Unidad, no debe existir.");
                                if (conp.Descripcion != "Pago de nómina")
                                    return ("NOM145 - El atributo Comprobante.Conceptos.Concepto,Descripcion no tiene el valor =  “Pago de nómina”.");


                            }
                        }

                        if (nom.TotalPercepcionesSpecified == false && nom.TotalOtrosPagosSpecified == false)
                            return ("NOM151 - El nodo Nomina no tiene TotalPercepciones y/o TotalOtrosPagos.");

                        if (comp.Conceptos[0].ValorUnitario != (TotalOtrosPagos + TotalPercepciones))
                            return ("NOM146 - El valor del atributo Comprobante.Conceptos.Concepto,ValorUnitario no coincide con la suma TotalPercepciones más TotalOtrosPagos.");
                        if (comp.Conceptos[0].Importe != (TotalOtrosPagos + TotalPercepciones))
                            return ("NOM147 - El valor del atributo Comprobante.Conceptos.Concepto,Importe no coincide con la suma TotalPercepciones más TotalOtrosPagos.");
                        if (comp.Conceptos[0].DescuentoSpecified == true && nom.TotalDeduccionesSpecified == true)
                            if (comp.Conceptos[0].Descuento != TotalDeducciones)
                                return ("NOM148 - El valor del atributo Comprobante.Conceptos.Concepto,Descuento no es igual a el valor del campo Nomina12:TotalDeducciones.");
                        if (comp.Conceptos[0].DescuentoSpecified == false && nom.TotalDeduccionesSpecified == true)
                            return ("NOM148 - El valor del atributo Comprobante.Conceptos.Concepto,Descuento no es igual a el valor del campo Nomina12:TotalDeducciones.");
                        if (comp.Conceptos[0].DescuentoSpecified == true && TotalDeduccionesbool == false)
                            return ("NOM148 - El valor del atributo Comprobante.Conceptos.Concepto,Descuento no es igual a el valor del campo Nomina12:TotalDeducciones.");


                    }


                    if (!string.IsNullOrEmpty(nom.Receptor.PeriodicidadPago))
                    {
                        c_PeriodicidadPago myTipoPeriodicidadPago;
                        Enum.TryParse("Item" + nom.Receptor.PeriodicidadPago, out myTipoPeriodicidadPago);
                        if (myTipoPeriodicidadPago.ToString() != "Item" + nom.Receptor.PeriodicidadPago)
                            return ("NOM180 - El valor del atributo Nomina.Receptor.PeriodicidadPago no cumple con un valor del catálogo c_PeriodicidadPago.");
                    }
                    if (nom.TipoNomina != "E" && nom.TipoNomina != "O")
                        return ("NOM152 - El valor del atributo Nomina.TipoNomina no cumple con un valor del catálogo c_TipoNomina.");
                    if (nom.TipoNomina == "O")
                        if (nom.Receptor.PeriodicidadPago != "01" && nom.Receptor.PeriodicidadPago != "02" &&
                            nom.Receptor.PeriodicidadPago != "03" && nom.Receptor.PeriodicidadPago != "04" &&
                            nom.Receptor.PeriodicidadPago != "05" && nom.Receptor.PeriodicidadPago != "06" &&
                            nom.Receptor.PeriodicidadPago != "07" && nom.Receptor.PeriodicidadPago != "08" &&
                            nom.Receptor.PeriodicidadPago != "09" && nom.Receptor.PeriodicidadPago != "10")
                            return ("NOM153 - El valor del atributo tipo de periodicidad no se encuentra entre 01 al 10.");
                    if (nom.TipoNomina == "E")
                        if (nom.Receptor.PeriodicidadPago != "99")
                            return ("NOM154 - El valor del atributo tipo de periodicidad no es 99.");
                    if (Convert.ToDateTime(nom.FechaFinalPago) < Convert.ToDateTime(nom.FechaInicialPago))
                        return ("NOM155 - El valor del atributo FechaInicialPago no es menor o igual al valor del atributo FechaFinalPago.");
                    if (nom.Percepciones == null)
                        if (nom.TotalPercepcionesSpecified == true)
                            return ("NOM156 - El atributo Nomina.TotalPercepciones, no debe existir.");
                    if (nom.Percepciones != null)
                        if (nom.TotalPercepciones != (nom.Percepciones.TotalSueldos + nom.Percepciones.TotalSeparacionIndemnizacion + nom.Percepciones.TotalJubilacionPensionRetiro))
                            return ("NOM157 - El valor del atributo Nomina.TotalPercepciones no coincide con la suma TotalSueldos más TotalSeparacionIndemnizacion más TotalJubilacionPensionRetiro del  nodo Percepciones.");
                    if (nom.Deducciones == null)
                        if (nom.TotalDeduccionesSpecified == true)
                            return ("NOM158 - El atributo Nomina.TotalDeducciones, no debe existir.");
                    if (nom.Deducciones != null)
                        if (nom.TotalDeducciones != (nom.Deducciones.TotalImpuestosRetenidos + nom.Deducciones.TotalOtrasDeducciones))
                            return ("NOM159 - El valor del atributo Nomina.TotalDeducciones no coincide con la suma de los atributos TotalOtrasDeducciones más TotalImpuestosRetenidos del elemento Deducciones.");

                    if (nom.OtrosPagos != null)
                    {
                        var t = 0M;
                        foreach (NominaOtroPago otros in nom.OtrosPagos)
                        {
                            t += otros.Importe;
                        }

                        if (nom.TotalOtrosPagos != t)
                            return ("NOM160 - El valor del atributo Nomina.TotalOtrosPagos no está registrado o  no coincide con la suma de los atributos Importe de los nodos nomina12:OtrosPagos:OtroPago.");

                    }


                    if (nom.Receptor.TipoContrato == "10" || nom.Receptor.TipoContrato == "09" ||
                        nom.Receptor.TipoContrato == "99")
                    {
                        if (nom.Emisor != null)
                            if (!string.IsNullOrEmpty(nom.Emisor.RegistroPatronal))
                                return ("NOM163 - El atributo Nomina.Emisor.RegistroPatronal  no se debe registrar.");
                    }
                    if (nom.Emisor != null)
                    {
                        if (nom.Receptor.TipoContrato == "01" || nom.Receptor.TipoContrato == "02" ||
                         nom.Receptor.TipoContrato == "03" || nom.Receptor.TipoContrato == "04" ||
                         nom.Receptor.TipoContrato == "05" || nom.Receptor.TipoContrato == "06" ||
                         nom.Receptor.TipoContrato == "07" || nom.Receptor.TipoContrato == "08")
                            if (string.IsNullOrEmpty(nom.Emisor.RegistroPatronal))
                                return ("NOM162 - El atributo Nomina.Emisor.RegistroPatronal se debe registrar.");

                        if (!string.IsNullOrEmpty(nom.Emisor.RegistroPatronal))
                            if (string.IsNullOrEmpty(nom.Receptor.NumSeguridadSocial) || nom.Receptor.FechaInicioRelLaboralSpecified == false
                                || string.IsNullOrEmpty(nom.Receptor.Antigüedad) || nom.Receptor.RiesgoPuestoSpecified == false || nom.Receptor.SalarioDiarioIntegradoSpecified == false)
                            {

                                return ("NOM164 - Los atributos NumSeguridadSocial, FechaInicioRelLaboral, Antigüedad,RiesgoPuesto,SalarioDiarioIntegrado deben existir.");
                            }
                        if (nom.Emisor.EntidadSNCF != null)
                        {
                            if (nom.Emisor.EntidadSNCF.OrigenRecurso != "IF" && nom.Emisor.EntidadSNCF.OrigenRecurso != "IM" &&
                                nom.Emisor.EntidadSNCF.OrigenRecurso != "IP")
                                return ("NOM167 - El valor del atributo Nomina.Emisor.EntidadSNCF.OrigenRecurso no cumple con un valor del catálogo c_OrigenRecurso.");

                            if (nom.Emisor.EntidadSNCF.OrigenRecurso == "IM")
                                if (nom.Emisor.EntidadSNCF.MontoRecursoPropioSpecified == false)
                                    return ("NOM168 - El atributo Nomina.Emisor.EntidadSNCF.MontoRecursoPropio debe existir.");

                            if (nom.Emisor.EntidadSNCF.OrigenRecurso != "IM")
                                if (nom.Emisor.EntidadSNCF.MontoRecursoPropioSpecified == true)
                                    return ("NOM169 - El atributo Nomina.Emisor.EntidadSNCF.MontoRecursoPropio no debe existir.");

                            if (nom.Emisor.EntidadSNCF.MontoRecursoPropio > (nom.TotalOtrosPagos + nom.TotalPercepciones))
                                return ("NOM170 - El valor del atributo Nomina.Emisor.EntidadSNCF.MontoRecursoPropio no es menor a la suma de los valores de los atributos TotalPercepciones y TotalOtrosPagos.");
                        }
                    }
                    if (nom.Receptor.TipoContrato != "01" && nom.Receptor.TipoContrato != "02" &&
                        nom.Receptor.TipoContrato != "03" && nom.Receptor.TipoContrato != "04" &&
                        nom.Receptor.TipoContrato != "05" && nom.Receptor.TipoContrato != "06" &&
                        nom.Receptor.TipoContrato != "07" && nom.Receptor.TipoContrato != "08" &&
                        nom.Receptor.TipoContrato != "09" && nom.Receptor.TipoContrato != "10" &&
                        nom.Receptor.TipoContrato != "99")
                        return ("NOM171 - El valor del atributo Nomina.Receptor.TipoContrato no cumple con un valor del catálogo c_TipoContrato.");
                    if (nom.Receptor.TipoJornadaSpecified == true)
                        if (nom.Receptor.TipoJornada != "01" && nom.Receptor.TipoJornada != "02" && nom.Receptor.TipoJornada != "03"
                             && nom.Receptor.TipoJornada != "04" && nom.Receptor.TipoJornada != "05" && nom.Receptor.TipoJornada != "06"
                              && nom.Receptor.TipoJornada != "07" && nom.Receptor.TipoJornada != "08" && nom.Receptor.TipoJornada != "99")
                            return ("NOM172 - El valor del atributo Nomina.Receptor.TipoJornada no cumple con un valor del catálogo c_TipoJornada.");

                    if (nom.Receptor.FechaInicioRelLaboralSpecified == true)
                        if (Convert.ToDateTime(nom.Receptor.FechaInicioRelLaboral) > Convert.ToDateTime(nom.FechaFinalPago))
                            return ("NOM173 - El valor del atributo Nomina.Receptor.FechaInicioRelLaboral no es menor o igual al atributo a FechaFinalPago.");

                    if (!string.IsNullOrEmpty(nom.Receptor.Antigüedad))
                    {
                        Regex regex = new Regex(@"P[1-9][0-9]{0,3}W");
                        if (regex.IsMatch(nom.Receptor.Antigüedad))
                        {
                            TimeSpan ts = Convert.ToDateTime(nom.FechaFinalPago) - Convert.ToDateTime(nom.Receptor.FechaInicioRelLaboral);
                            int dias = ts.Days;
                            dias = (dias + 1) / 7;
                            string cadena = "";
                            for (int ctr = 0; ctr < nom.Receptor.Antigüedad.Length; ctr++)
                                if (Char.IsNumber(nom.Receptor.Antigüedad[ctr]))
                                    cadena = cadena + nom.Receptor.Antigüedad[ctr];

                            if (dias < Convert.ToInt16(cadena))
                                return ("NOM174 - El valor numérico del atributo Nomina.Receptor.Antigüedad no es menor o igual al cociente de (la suma del número de días transcurridos entre la FechaInicioRelLaboral y la FechaFinalPago más uno) dividido entre siete.");
                        }
                        Regex regex2 = new Regex(@"P([1-9][0-9]?Y)?(([1-9]|1[012])M)?(0|[1-9]|[12][0-9]|3[01])D");
                        if (regex2.IsMatch(nom.Receptor.Antigüedad))
                        {/*
                        string xx = nom.Receptor.Antigüedad.Replace("M0D", "M");
                        string s = DiferenciaFechas(Convert.ToDateTime(nom.FechaFinalPago), Convert.ToDateTime(nom.Receptor.FechaInicioRelLaboral));
                        if (s == "P1M" && s != xx)
                        {
                            DateTime fechaMes = Convert.ToDateTime(nom.FechaFinalPago);
                            int anyo = fechaMes.Year;
                            int mes = fechaMes.Month;
                            int dias = DateTime.DaysInMonth(anyo, mes);
                            s = "P" + dias + "D";
                            if (s != xx)
                                return ("NOM175 - El valor del atributo Nomina.Receptor.Antigüedad. no cumple con el número de años, meses y días transcurridos entre la FechaInicioRelLaboral y la FechaFinalPago.");

                        }
                        else
                        {
                            if (s != xx)
                                return ("NOM175 - El valor del atributo Nomina.Receptor.Antigüedad. no cumple con el número de años, meses y días transcurridos entre la FechaInicioRelLaboral y la FechaFinalPago.");
                        }
                      */
                            string s = DiferenciaFechas(Convert.ToDateTime(nom.FechaFinalPago), Convert.ToDateTime(nom.Receptor.FechaInicioRelLaboral));
                            if (s != nom.Receptor.Antigüedad)
                            {
                                s = DiferenciaFechas2(Convert.ToDateTime(nom.FechaFinalPago), Convert.ToDateTime(nom.Receptor.FechaInicioRelLaboral));
                                if (s != nom.Receptor.Antigüedad)

                                    return ("NOM175 - El valor del atributo Nomina.Receptor.Antigüedad. no cumple con el número de años, meses y días transcurridos entre la FechaInicioRelLaboral y la FechaFinalPago.");
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(nom.Receptor.TipoRegimen))
                    {
                        c_TipoRegimen myTipoRegimen;
                        Enum.TryParse("Item" + nom.Receptor.TipoRegimen, out myTipoRegimen);

                        if (myTipoRegimen.ToString() != "Item" + nom.Receptor.TipoRegimen)
                            return ("NOM176 - El valor del atributo Nomina.Receptor.TipoRegimen no cumple con un valor del catálogo c_TipoRegimen.");
                    }
                    if (nom.Receptor.TipoContrato == "01" || nom.Receptor.TipoContrato == "02" ||
                        nom.Receptor.TipoContrato == "03" || nom.Receptor.TipoContrato == "04" ||
                        nom.Receptor.TipoContrato == "05" || nom.Receptor.TipoContrato == "06" ||
                        nom.Receptor.TipoContrato == "07" || nom.Receptor.TipoContrato == "08")
                        if (nom.Receptor.TipoRegimen != "02" && nom.Receptor.TipoRegimen != "03" && nom.Receptor.TipoRegimen != "04")
                            return ("NOM177 - El atributo Nomina.Receptor.TipoRegimen no es 02, 03 ó 04.");

                    if (nom.Receptor.TipoContrato == "10" || nom.Receptor.TipoContrato == "09" ||
                        nom.Receptor.TipoContrato == "99")
                        if (nom.Receptor.TipoRegimen != "05" && nom.Receptor.TipoRegimen != "06" && nom.Receptor.TipoRegimen != "07"
                            && nom.Receptor.TipoRegimen != "08" && nom.Receptor.TipoRegimen != "09" && nom.Receptor.TipoRegimen != "10"
                            && nom.Receptor.TipoRegimen != "11" && nom.Receptor.TipoRegimen != "12" && nom.Receptor.TipoRegimen != "13" && nom.Receptor.TipoRegimen != "99")
                            return ("NOM178 - El atributo Nomina.Receptor.TipoRegimen no está entre 05 a 99.");

                    if (nom.Receptor.RiesgoPuestoSpecified == true)
                        if (nom.Receptor.RiesgoPuesto != "1" && nom.Receptor.RiesgoPuesto != "2" && nom.Receptor.RiesgoPuesto != "3" &&
                            nom.Receptor.RiesgoPuesto != "4" && nom.Receptor.RiesgoPuesto != "5")
                            return ("NOM179 - El valor del atributo Nomina.Receptor.RiesgoPuesto no cumple con un valor del catálogo c_RiesgoPuesto.");

                    if (nom.Receptor.BancoSpecified == true)
                    {
                        c_Banco myTipoBanco;
                        Enum.TryParse("Item" + nom.Receptor.Banco, out myTipoBanco);
                        if (myTipoBanco.ToString() != "Item" + nom.Receptor.Banco)
                            return ("NOM181 - El valor del atributo Nomina.Receptor.Banco no cumple con un valor del catálogo c_Banco.");
                    }
                    if (!string.IsNullOrEmpty(nom.Receptor.CuentaBancaria))
                        if (nom.Receptor.CuentaBancaria.Length != 10 && nom.Receptor.CuentaBancaria.Length != 11
                            && nom.Receptor.CuentaBancaria.Length != 16 && nom.Receptor.CuentaBancaria.Length != 18)
                            return ("NOM182 - El atributo CuentaBancaria no cumple con la longitud de 10, 11, 16 ó 18 posiciones.");

                    if (!string.IsNullOrEmpty(nom.Receptor.CuentaBancaria))
                        if (nom.Receptor.CuentaBancaria.Length == 18)
                        {
                            if (nom.Receptor.BancoSpecified == true)
                                return ("NOM183 - El atributo Banco no debe existir.");
                            if (!ValidaDigitoControl(nom.Receptor.CuentaBancaria))
                                return ("NOM184 - El dígito de control del atributo CLABE no es correcto.");
                        }
                    if (!string.IsNullOrEmpty(nom.Receptor.CuentaBancaria))
                        if (nom.Receptor.CuentaBancaria.Length == 10 || nom.Receptor.CuentaBancaria.Length == 11
                            || nom.Receptor.CuentaBancaria.Length == 16)
                            if (nom.Receptor.BancoSpecified == false)
                                return ("NOM185 - El atributo Banco debe existir.");
                    if (!string.IsNullOrEmpty(nom.Receptor.ClaveEntFed))
                    {
                        // ServicioLocal.Business.c_Estado myTipoEstado = new ServicioLocal.Business.c_Estado();
                        // Enum.TryParse<ServicioLocal.Business.c_Estado>(nom.Receptor.ClaveEntFed, out myTipoEstado);
                        // if (myTipoEstado.ToString() != nom.Receptor.ClaveEntFed)
                        int VanderaEstado = 0;
                        foreach (ServicioLocal.Business.c_Estado estado in Enum.GetValues(typeof(ServicioLocal.Business.c_Estado)))
                        {
                            if (estado.ToString() == nom.Receptor.ClaveEntFed)
                            {
                                VanderaEstado = 1;
                                break;
                            }
                        }
                        if (VanderaEstado == 0)
                            return ("NOM186 - El valor del atributo ClaveEntFed no cumple con un valor del catálogo c_Estado.");
                    }

                    if (nom.Receptor.SubContratacion != null)
                    {
                        if (nom.Receptor.SubContratacion.Count > 0)
                        {
                            decimal tiempo = 0M;
                            foreach (NominaReceptorSubContratacion sub in nom.Receptor.SubContratacion)
                            {
                                tiempo += sub.PorcentajeTiempo;
                            }

                            if (tiempo != 100)
                                return ("NOM188 - La suma de los valores registrados en el atributo Nomina.Receptor.SubContratacion.PorcentajeTiempo no es igual a 100.");
                        }
                    }
                    if (nom.Percepciones != null)
                    {
                        decimal da = nom.Percepciones.TotalSueldos + nom.Percepciones.TotalSeparacionIndemnizacion + nom.Percepciones.TotalJubilacionPensionRetiro;
                        decimal da2 = nom.Percepciones.TotalExento + nom.Percepciones.TotalGravado;
                        if (da != 0 && da2 != 0)
                            if (da2 != da)
                                return ("NOM189 - La suma de los valores de los atributos TotalSueldos más TotalSeparacionIndemnizacion más TotalJubilacionPensionRetiro no es igual a la suma de los valores de los atributos TotalGravado más TotalExento.");
                    }
                    if (nom.Percepciones != null)
                    {
                        if (nom.Percepciones.Percepcion != null)
                        {
                            decimal TotalSueldos = 0;
                            decimal TotalSeparacionIndemnizacion = 0;
                            decimal TotalJubilacionPensionRetiro = 0;
                            decimal TotalGravado = 0;
                            decimal TotalExento = 0;

                            if (nom.Percepciones.JubilacionPensionRetiro != null)
                            {
                                if (nom.Percepciones.JubilacionPensionRetiro.TotalUnaExhibicionSpecified == true)
                                    if (nom.Percepciones.JubilacionPensionRetiro.MontoDiarioSpecified == true || nom.Percepciones.JubilacionPensionRetiro.TotalParcialidadSpecified == true)
                                        return ("NOM209 - Los atributos MontoDiario y TotalParcialidad no deben existir, ya que existe valor en TotalUnaExhibicion.");

                                if (nom.Percepciones.JubilacionPensionRetiro.TotalParcialidadSpecified == true)
                                    if (nom.Percepciones.JubilacionPensionRetiro.MontoDiarioSpecified == false || nom.Percepciones.JubilacionPensionRetiro.TotalUnaExhibicionSpecified == true)
                                        return ("NOM210 - El atributo MontoDiario debe existir y el atributo TotalUnaExhibicion no debe existir, ya que Nomina.Percepciones.JubilacionPensionRetiro.TotalParcialidad tiene valor.");
                            }

                            decimal ImporteExentoImporteGravado = 0;
                            foreach (NominaPercepcionesPercepcion per in nom.Percepciones.Percepcion)
                            {
                                if (per.TipoPercepcion != "022" && per.TipoPercepcion != "023" && per.TipoPercepcion != "025"
                                    && per.TipoPercepcion != "039" && per.TipoPercepcion != "044")
                                {
                                    TotalSueldos += per.ImporteGravado + per.ImporteExento;
                                }
                                if (per.TipoPercepcion == "022" || per.TipoPercepcion == "023" || per.TipoPercepcion == "025")
                                {
                                    TotalSeparacionIndemnizacion += per.ImporteGravado + per.ImporteExento;
                                }
                                if (per.TipoPercepcion == "039" || per.TipoPercepcion == "044")
                                {
                                    TotalJubilacionPensionRetiro += per.ImporteGravado + per.ImporteExento;
                                }
                                TotalGravado += per.ImporteGravado;
                                TotalExento += per.ImporteExento;

                                if ((per.ImporteGravado + per.ImporteExento) <= 0)
                                    return ("NOM195 - La suma de los importes de los atributos ImporteGravado e ImporteExento no es mayor que cero.");
                                c_TipoPercepcion myTipoPercepcion;
                                Enum.TryParse("Item" + per.TipoPercepcion, out myTipoPercepcion);
                                if (myTipoPercepcion.ToString() != ("Item" + per.TipoPercepcion))
                                    return ("NOM196 - El valor del atributo Nomina.Percepciones.Percepcion.TipoPercepcion no cumple con un valor del catálogo c_TipoPercepcion.");

                                if (per
                                    .TipoPercepcion != "039" && per.TipoPercepcion != "044" && per.TipoPercepcion != "023" && per.TipoPercepcion != "025" && per.TipoPercepcion != "022")
                                    if (nom.Percepciones.TotalSueldosSpecified == false)
                                        return ("NOM197 - TotalSueldos, debe existir. Ya que la clave expresada en TipoPercepcion es distinta de 022, 023, 025, 039 y 044.");
                                if (per.TipoPercepcion == "022" || per.TipoPercepcion == "023" || per.TipoPercepcion == "025")
                                    if (nom.Percepciones.TotalSeparacionIndemnizacionSpecified == false || nom.Percepciones.SeparacionIndemnizacion == null)
                                        return ("NOM198 - TotalSeparacionIndemnizacion y el elemento SeparacionIndemnizacion, debe existir. Ya que la clave expresada en TipoPercepcion es 022 ó 023 ó 025.");
                                if (per.TipoPercepcion == "039" || per.TipoPercepcion == "044")
                                    if (nom.Percepciones.TotalJubilacionPensionRetiroSpecified == false || nom.Percepciones.JubilacionPensionRetiro == null)
                                        return ("NOM199 - TotalJubilacionPensionRetiro y el elemento JubilacionPensionRetiro debe existir,  ya que la clave expresada en el atributo TipoPercepcion es 039 ó 044");

                                if (per.TipoPercepcion == "039")
                                {
                                    if (nom.Percepciones.JubilacionPensionRetiro != null)
                                    {
                                        if (nom.Percepciones.JubilacionPensionRetiro.TotalUnaExhibicionSpecified == false ||
                                           nom.Percepciones.JubilacionPensionRetiro.MontoDiarioSpecified == true || nom.Percepciones.JubilacionPensionRetiro.TotalParcialidadSpecified == true)
                                            if (nom.Percepciones.JubilacionPensionRetiro.TotalUnaExhibicionSpecified == false || nom.Percepciones.JubilacionPensionRetiro.MontoDiario > 0 || nom.Percepciones.JubilacionPensionRetiro.TotalParcialidad > 0)
                                                return ("NOM200 - TotalUnaExhibicion debe existir y no deben existir TotalParcialidad, MontoDiario. Ya que la clave expresada en el atributo TipoPercepcion es 039.");
                                    }
                                    else
                                        return ("NOM200 - TotalUnaExhibicion debe existir y no deben existir TotalParcialidad, MontoDiario. Ya que la clave expresada en el atributo TipoPercepcion es 039.");

                                }
                                if (per.TipoPercepcion == "044")
                                {
                                    if (nom.Percepciones.JubilacionPensionRetiro != null)
                                    {
                                        if (nom.Percepciones.JubilacionPensionRetiro.TotalUnaExhibicionSpecified == true ||
                                           nom.Percepciones.JubilacionPensionRetiro.MontoDiarioSpecified == false || nom.Percepciones.JubilacionPensionRetiro.TotalParcialidadSpecified == false)
                                            return ("NOM201 - TotalUnaExhibicion no debe existir y deben existir TotalParcialidad, MontoDiario. Ya que la clave expresada en el atributo TipoPercepcion es 044.");
                                    }
                                    else
                                        return ("NOM201 - TotalUnaExhibicion no debe existir y deben existir TotalParcialidad, MontoDiario. Ya que la clave expresada en el atributo TipoPercepcion es 044.");

                                }

                                if (per.TipoPercepcion == "045")
                                {
                                    if (per.AccionesOTitulos == null)
                                        return ("NOM202 - El elemento AccionesOTitulos debe existir. Ya que la clave expresada en el atributo TipoPercepcion es 045.");

                                }
                                else
                                    if (per.AccionesOTitulos != null)
                                        return ("NOM203 - El elemento AccionesOTitulos no debe existir. Ya que la clave expresada en el atributo TipoPercepcion no es 045.");
                                //-----
                                if (per.TipoPercepcion == "019")
                                {
                                    if (per.HorasExtra == null)
                                        return ("NOM204 - El elemento HorasExtra, debe existir. Ya que la clave expresada en el atributo TipoPercepcion es 019.");
                                    if (per.HorasExtra.Count == 0)
                                        return ("NOM204 - El elemento HorasExtra, debe existir. Ya que la clave expresada en el atributo TipoPercepcion es 019.");

                                }
                                else
                                {
                                    if (per.HorasExtra != null)
                                        if (per.HorasExtra.Count > 0)
                                            return ("NOM205 - El elemento HorasExtra, no debe existir. Ya que la clave expresada en el atributo TipoPercepcion no es 019.");

                                }
                                //---------
                                if (per.TipoPercepcion == "014")
                                {
                                    if (nom.Incapacidades == null)
                                        return ("NOM206 - El nodo Incapacidades debe existir, Ya que la clave expresada en el atributo TipoPercepcion es 014.");
                                    if (nom.Incapacidades.Count == 0)
                                        return ("NOM206 - El nodo Incapacidades debe existir, Ya que la clave expresada en el atributo TipoPercepcion es 014.");
                                    ImporteExentoImporteGravado = ImporteExentoImporteGravado + per.ImporteExento + per.ImporteGravado;
                                }
                                //------------
                                if (per.HorasExtra != null)
                                    if (per.HorasExtra.Count > 0)
                                    {

                                        foreach (NominaPercepcionesPercepcionHorasExtra h in per.HorasExtra)
                                        {
                                            c_TipoHoras myTipoTipoHoras;
                                            Enum.TryParse("Item" + h.TipoHoras, out myTipoTipoHoras);
                                            if (myTipoTipoHoras.ToString() != "Item" + h.TipoHoras)
                                                return ("NOM208 - El valor del atributo Nomina.Percepciones.Percepcon.HorasExtra.TipoHoras no cumple con un valor del catálogo c_TipoHoras.");

                                        }
                                    }
                                //------



                            }
                            if (ImporteExentoImporteGravado > 0)
                            {
                                decimal i = 0M;
                                foreach (NominaIncapacidad inc in nom.Incapacidades)
                                {
                                    if (inc.ImporteMonetarioSpecified == true)
                                        i += inc.ImporteMonetario;
                                }
                                if (i != (ImporteExentoImporteGravado))
                                    return ("NOM207 - La suma de los campos ImporteMonetario no es igual a la suma de los valores ImporteGravado e ImporteExento de la percepción, Ya que la clave expresada en el atributo TipoPercepcion es 014.");
                            }
                            //-----------------
                            if (nom.Percepciones.TotalGravado != TotalGravado)
                                return ("NOM193 - El valor del atributo Nomina.Percepciones.TotalGravado, no es igual a la suma de los atributos ImporteGravado de los nodos Percepcion.");
                            //---
                            if (nom.Percepciones.TotalExento != TotalExento)
                                return ("NOM194 - El valor del atributo Nomina.Percepciones.TotalExento, no es igual a la suma de los atributos ImporteExento de los nodos Percepcion.");

                            if (nom.Percepciones.TotalSueldos != TotalSueldos)
                                return ("NOM190 - El valor del atributo Nomina.Percepciones.TotalSueldos , no es igual a la suma de los atributos ImporteGravado e ImporteExento donde la clave expresada en el atributo TipoPercepcion es distinta de 022 Prima por Antigüedad, 023 Pagos por separación, 025 Indemnizaciones, 039 Jubilaciones, pensiones o haberes de retiro en una exhibición y 044 Jubilaciones, pensiones o haberes de retiro en parcialidades.");
                            //-----------
                            if (nom.Percepciones.TotalSeparacionIndemnizacion != TotalSeparacionIndemnizacion)
                                return ("NOM191 - El valor del atributo Nomina.Percepciones.TotalSeparacionIndemnizacion, no es igual a la suma de los atributos ImporteGravado e ImporteExento donde la clave en el atributo TipoPercepcion es igual a 022 Prima por Antigüedad, 023 Pagos por separación ó 025 Indemnizaciones.");
                            //----------------
                            if (nom.Percepciones.TotalJubilacionPensionRetiro != TotalJubilacionPensionRetiro)
                                return ("NOM192 - El valor del atributo Nomina.Percepciones.TotalJubilacionPensionRetiro, no es igual a la suma de los atributos ImporteGravado e importeExento donde la clave expresada en el atributo TipoPercepcion es igual a 039(Jubilaciones, pensiones o haberes de retiro en una exhibición)  ó 044 (Jubilaciones, pensiones o haberes de retiro en parcialidades).");



                        }
                    }
                    //-------------------------------
                    if (nom.Deducciones != null)
                    {

                        decimal totalimporte = 0M;
                        int bandera = 0;

                        decimal suma006 = 0M;
                        foreach (NominaDeduccionesDeduccion de in nom.Deducciones.Deduccion)
                        {
                            //---
                            c_TipoDeduccion myTipoTipoDeduccion;
                            Enum.TryParse("Item" + de.TipoDeduccion, out myTipoTipoDeduccion);
                            if (myTipoTipoDeduccion.ToString() != "Item" + de.TipoDeduccion)
                                return ("NOM213 - El valor del atributo Nomina.Deducciones.Deduccion.TipoDeduccion no cumple con un valor del catálogo c_TipoDeduccion.");

                            if (de.Importe <= 0)
                                return ("NOM216 - Nomina.Deducciones.Deduccion.Importe no es mayor que cero.");

                            if (de.TipoDeduccion == "002")
                            {
                                bandera = 1;

                                totalimporte += de.Importe;
                            }
                            //---
                            if (de.TipoDeduccion == "006")
                            {

                                suma006 += de.Importe;
                            }


                        }
                        if (bandera == 0)
                            if (nom.Deducciones.TotalImpuestosRetenidosSpecified == true)
                                return ("NOM212 - Nomina.Deducciones.TotalImpuestosRetenidos no debe existir, ya que no existen deducciones con clave 002 en el atributo TipoDeduccion.");

                        if (nom.Deducciones.TotalImpuestosRetenidos != totalimporte)
                            return ("NOM211 - El valor en el atributo Nomina.Deducciones.TotalImpuestosRetenidos no es igual a la suma de los atributos Importe de las deducciones que tienen expresada la clave 002 en el atributo TipoDeduccion.");

                        //--------------
                        if (suma006 > 0)
                        {
                            if (nom.Incapacidades == null)
                                return ("NOM214 - Debe existir el elemento Incapacidades, ya que la clave expresada en Nomina.Deducciones.Deduccion.TipoDeduccion es 006.");
                            if (nom.Incapacidades.Count == 0)
                                return ("NOM214 - Debe existir el elemento Incapacidades, ya que la clave expresada en Nomina.Deducciones.Deduccion.TipoDeduccion es 006.");
                            decimal i = 0M;
                            foreach (NominaIncapacidad inc in nom.Incapacidades)
                            {
                                if (inc.ImporteMonetarioSpecified == true)
                                    i += inc.ImporteMonetario;
                            }
                            if (i != suma006)
                                return ("NOM215 - El atributo Deduccion:Importe no es igual a la suma de los nodos Incapacidad:ImporteMonetario. Ya que la clave expresada en Nomina.Deducciones.Deduccion.TipoDeduccion es 006");
                        }

                    }
                    if (nom.OtrosPagos != null)
                    {

                        foreach (NominaOtroPago otro in nom.OtrosPagos)
                        {

                            c_TipoOtroPago myTipoTipoOtroPago;
                            Enum.TryParse("Item" + otro.TipoOtroPago, out myTipoTipoOtroPago);
                            if (myTipoTipoOtroPago.ToString() != "Item" + otro.TipoOtroPago)
                                return ("NOM217 - El valor del atributo Nomina.OtrosPagos.OtroPago.TipoOtroPago no cumple con un valor del catálogo c_TipoOtroPago.");
                            if (otro.TipoOtroPago == "004")
                                if (otro.CompensacionSaldosAFavor == null)
                                    return ("NOM218 - El nodo CompensacionSaldosAFavor debe existir, ya que el valor de Nomina.OtrosPagos.OtroPago.TipoOtroPago es 004.");

                            if (otro.TipoOtroPago == "002")
                            {
                                if (otro.SubsidioAlEmpleo == null)
                                    return ("NOM219 - El nodo SubsidioAlEmpleo. debe existir, ya que el valor de Nomina.OtrosPagos.OtroPago.TipoOtroPago es 002.");
                               

                                if (otro.Importe < 0)
                                    return ("NOM220 - Nomina.OtrosPagos.OtroPago.Importe no es mayor que cero.");
                            }
                            else// diferente a subsidio para el empleo
                            {
                                if (otro.Importe <= 0)
                                    return ("NOM220 - Nomina.OtrosPagos.OtroPago.Importe no es mayor que cero.");

                            }

                            if (otro.SubsidioAlEmpleo != null)
                            {
                                if (otro.SubsidioAlEmpleo.SubsidioCausado < otro.Importe)
                                    return ("NOM221 - Nomina.OtrosPagos.OtroPago.SubsidioAlEmpleo.SubsidioCausado no es mayor o igual que el valor del atributo Importe del nodo OtroPago.");
                            }
                            //else
                            //    if (nom.Receptor.TipoRegimen == "02")
                            //        return ("NOM219 - El nodo SubsidioAlEmpleo. debe existir.");
                       
                            if (otro.CompensacionSaldosAFavor != null)
                            {
                                if (otro.CompensacionSaldosAFavor.RemanenteSalFav > otro.CompensacionSaldosAFavor.SaldoAFavor)
                                    return ("NOM222 - Nomina.OtrosPagos.OtroPago.CompensacionSaldosAFavor.SaldoAFavor no es mayor o igual que el valor del atributo CompensacionSaldosAFavor:RemanenteSalFav.");
                                DateTime localDate = DateTime.Now;
                                int año = localDate.Year;
                                int mes = localDate.Month;
                                if (!string.IsNullOrEmpty(nom.FechaPago))
                                {
                                    DateTime F = Convert.ToDateTime(nom.FechaPago);
                                    mes = F.Month;
                                }

                                if (mes == 12)//
                                {
                                    if (año < otro.CompensacionSaldosAFavor.Año)
                                        return ("NOM223 - Nomina.OtrosPagos.OtroPago.CompensacionSaldosAFavor.Año  no es menor que el año en curso.");
                                }
                                else
                                {
                                    if (año <= otro.CompensacionSaldosAFavor.Año)
                                        return ("NOM223 - Nomina.OtrosPagos.OtroPago.CompensacionSaldosAFavor.Año  no es menor que el año en curso.");
                                }
                            }
                        }

                    }
                    //else
                    //{
                    //   if(nom.Receptor.TipoRegimen=="02")
                    //       return ("NOM219 - El nodo SubsidioAlEmpleo. debe existir.");
                                      
                    //}
                    if (nom.Incapacidades != null)
                        if (nom.Incapacidades.Count > 0)
                        {

                            foreach (NominaIncapacidad inc in nom.Incapacidades)
                            {
                                c_TipoIncapacidad myTipoTipoIncapacidad;
                                Enum.TryParse("Item" + inc.TipoIncapacidad, out myTipoTipoIncapacidad);
                                if (myTipoTipoIncapacidad.ToString() != "Item" + inc.TipoIncapacidad)
                                    return ("NOM224 - El valor del atributo Incapacidades.Incapacidad.TipoIncapacidad no cumple con un valor del catálogo c_TIpoIncapacidad.");

                            }
                        }
                    bool tr = false; bool tr2 = false;
                    if (nom.Receptor.TipoRegimen == "02")
                    {
                        if (nom.OtrosPagos == null)
                            return ("NOM226 - El elemento OtroPago no contiene un atributo TipoOtroPago con la clave 002 o se registró junto con otro atributo TipoOtroPago con clave 007 o 008.");

                        foreach (NominaOtroPago otro in nom.OtrosPagos)
                        {
                            if (otro.TipoOtroPago == "002")
                                tr = true;
                            if (otro.TipoOtroPago == "007" || otro.TipoOtroPago == "008")
                                tr2 = true;
                        }
                        if ((tr == false && tr2 == false) || (tr == true && tr2 == true))
                            return ("NOM226 - El elemento OtroPago no contiene un atributo TipoOtroPago con la clave 002 o se registró junto con otro atributo TipoOtroPago con clave 007 o 008.");


                    }
                    else
                    {
                        if (nom.OtrosPagos != null)
                            foreach (NominaOtroPago otro in nom.OtrosPagos)
                            {
                                if (otro.TipoOtroPago == "002")
                                    tr = true;
                                if (otro.TipoOtroPago == "007" || otro.TipoOtroPago == "008")
                                    tr2 = true;
                            }
                        if (tr == true || tr2 == true)
                            return ("NOM227 - En el atributo TipoOtroPago no deben registrarse las claves 002, 007 o 008 ya que en el atributo TipoRegimen no existe la clave 02.");


                    }
                    if (nom.OtrosPagos != null)
                        foreach (NominaOtroPago otro in nom.OtrosPagos)
                        {
                            if (otro.TipoOtroPago == "002")
                            {
                                if (otro.SubsidioAlEmpleo != null)
                                {
                                    if (otro.Importe > otro.SubsidioAlEmpleo.SubsidioCausado)
                                        return ("NOM228 - El Importe del elemento OtroPago no es menor o igual que el valor del atributo SubsidioCausado");

                                    if (nom.NumDiasPagados > 31)
                                    {
                                        if (otro.SubsidioAlEmpleo.SubsidioCausado > (13.39M * nom.NumDiasPagados))
                                            return ("NOM229 - El valor del atributo SubsidioCausado no puede ser mayor que el resultado de multiplicar el factor 13.39 por el valor del atributo NumDiasPagados.");

                                    }

                                }
                            }


                        }

                }


                return "0";

                
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return ("NOM225-  "+ ex.Message);
                throw;
            }
            
        }
        private string DiferenciaFechas(DateTime newdt, DateTime olddt)
        {
            newdt = newdt.AddDays(1);
            Int32 anos;
            Int32 meses;
            Int32 dias;
            String str = "P";


            anos = (newdt.Year - olddt.Year);
            meses = (newdt.Month - olddt.Month);
            dias = (newdt.Day - olddt.Day);

            if (meses < 0)
            {
                anos -= 1;
                meses += 12;
            }

            if (dias < 0)
            {
                meses -= 1;
                int DiasAno = newdt.Year;
                int DiasMes = newdt.Month;

                if ((newdt.Month - 1) == 0)
                {
                    DiasAno = DiasAno - 1;
                    DiasMes = 12;
                }
                else
                {
                    DiasMes = DiasMes - 1;
                }

                dias += DateTime.DaysInMonth(DiasAno, DiasMes);
            }
            //------------
           /* if (meses == 0 && anos>0)
            {
                meses = 12;
                anos=anos - 1;
            }*/
            //--------------------
            if (anos < 0)
            {
                return "Fecha Invalida";
            }
            if (anos > 0)
                str = str + anos.ToString() + "Y";
            if (meses > 0)
                str = str + meses.ToString() + "M";
          //  if (dias > 0)
                str = str + dias.ToString() + "D";

            return str;
        }
        private string DiferenciaFechas2(DateTime newdt, DateTime olddt)
        {
            newdt = newdt.AddDays(1);
            Int32 anos;
            Int32 meses;
            Int32 dias;
            Int32 diasM;
            String str = "P";


            anos = (newdt.Year - olddt.Year);
            meses = (newdt.Month - olddt.Month);
            dias = (newdt.Day - olddt.Day);

            if (meses < 0)
            {
                anos -= 1;
                meses += 12;
            }

            if (dias < 0)
            {
                meses -= 1;
                int DiasAno = newdt.Year;
                int DiasMes = newdt.Month;

                if ((newdt.Month - 1) == 0)
                {
                    DiasAno = DiasAno - 1;
                    DiasMes = 12;
                }
                else
                {
                    DiasMes = DiasMes - 1;
                }
                diasM = DateTime.DaysInMonth(DiasAno, DiasMes);

                dias += DateTime.DaysInMonth(DiasAno, DiasMes);

            }
            else
                diasM = DateTime.DaysInMonth(olddt.Year, olddt.Month);

            //------------
            /* if (meses == 0 && anos>0)
             {
                 meses = 12;
                 anos=anos - 1;
             }*/
            //--------------------
            if (0 == dias && meses > 0)
            {
                meses = meses - 1;
                dias = diasM;
            }
            if (anos < 0)
            {
                return "Fecha Invalida";
            }
            if (anos > 0)
                str = str + anos.ToString() + "Y";
            if (meses > 0)
                str = str + meses.ToString() + "M";
            //if (dias > 0)

            str = str + dias.ToString() + "D";

            return str;
        }
        private Dictionary<int, string> Procesar(string RFC,XElement xe, string archivoEntrada,XElement addenda,Guid uuid, ref string res, ref TimbreFiscalDigital timbre, ref string acuseSat, ref string hash)
        {
            
            Dictionary<int, string> erroresPac = new Dictionary<int, string>();
            try
            {
                string strContent = xe.ToString();

                var errores = new List<int>();
                string version = xe.Attribute("Version") == null ? "" : xe.Attribute("Version").Value;
                if (version == "")
                {
                    errores.Add(301);
                    List<int> errorRoe = errores.Where(p => p != 0).ToList();
                    erroresPac = CrearArchivoROE(errorRoe, archivoEntrada);
                    return erroresPac;
                }

                var validadorEstructura = _validadorEstructura.Clone() as ValidadorEstructura;

                //1.-Que cumpla la con el estándar de XML (Conforme al W3C) y con la estructura XML (XSD y complementos aplicables)
                if (version != xe.Attribute("Version").Value)
                {
                    errores.Add(106);
                    erroresPac = CrearArchivoROE(new List<int> { 106 }, archivoEntrada);
                    return erroresPac;
                }
                /* -------------se quito para que no valide los catalogos caidos-------------------*/
                var errorXmlValidacion = validadorEstructura.Validate2(strContent);
                errores.Add(errorXmlValidacion.Valido);
                if (errorXmlValidacion.Valido != 0)
                {
                    erroresPac = CrearArchivoROE(errores, archivoEntrada, errorXmlValidacion.ErroresEstructura.ToString());
                    return erroresPac;
                }
                


                string sello = xe.Attribute("Sello") == null ? "" : xe.Attribute("Sello").Value;
                string serieCert = xe.Attribute("NoCertificado") == null ? string.Empty : xe.Attribute("NoCertificado").Value;
                string fecha = xe.Attribute("Fecha") == null ? "" : xe.Attribute("Fecha").Value;

                DateTime fechaEmisionCfdi = Convert.ToDateTime(fecha);
                string rfc = ((XElement)xe.FirstNode).Attribute("Rfc") == null ? "" : ((XElement)xe.FirstNode).Attribute("Rfc").Value;
                rfc = RFC;
                string cadenaOriginal;
                lock (_genCadenas)
                {
                    cadenaOriginal = _genCadenas.CadenaOriginal(strContent);
                }


                //1.1.-Validamos que el contenido del CSD se trate realmente de un X509//
                if (this._certificado == null || CertUtil.ValidarLongitudCertificado(_certificado.SerialNumber) != serieCert) // Error 399
                {
                    byte[] cert = (xe.Attribute("Certificado") == null || string.IsNullOrEmpty(xe.Attribute("Certificado").Value)) ? new byte[0] : Convert.FromBase64String(xe.Attribute("Certificado").Value);
                    if (cert.Length == 0)
                    {
                        errores.Add(399);
                        DesSerializar(xe, cadenaOriginal);
                        erroresPac = CrearArchivoROE(errores, archivoEntrada);
                        return erroresPac;
                    }
                    try
                    {
                        _certificado = new X509Certificate2(cert);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        errores.Add(399);
                        DesSerializar(xe, cadenaOriginal);
                        erroresPac = CrearArchivoROE(errores, archivoEntrada);
                        return erroresPac;
                    }
                }

                DateTime datFechaExpiracionCSD = Convert.ToDateTime(_certificado.GetExpirationDateString());
                DateTime datFechaEfectivaCSD = Convert.ToDateTime(_certificado.GetEffectiveDateString());
                fecha = xe.Attribute("Fecha") == null ? "" : xe.Attribute("Fecha").Value;

                var validadorDatos = new ValidadorDatos();

                //-Que el CSD del Emisor haya sido firmado por uno de los Certificados de Autoridad de SAT
                errores.Add(validadorDatos.ValidaCertificadoAc(_certificado));
                //-Que la llave utilizada para sellar corresponda a un CSD (no de FIEL)
                var fechaFiel = new DateTime(2015,03,04);
                if (fechaEmisionCfdi >= fechaFiel)
                { 
                    errores.Add(validadorDatos.ValidaCertificadoCSDnoFIEL(_certificado)); 
                }
                //-Que el sello del Emisor sea válido 
                byte[] firma = null;
                try
                {
                    firma = Convert.FromBase64String(sello);
                }
                catch (Exception ee)
                {
                    Logger.Error(ee);
                    errores.Add(302);
                }
                if (firma != null)
                {
                    errores.Add(validadorDatos.ValidarSello(cadenaOriginal, firma, _certificado, ref hash));
                }
                Logger.Debug(hash);
                //-Que el CSD del Emisor corresponda al RFC que viene como Emisor en el Comprobante
                errores.Add(validadorDatos.ValidaRFCEmisor(rfc, _certificado.SubjectName.Name));
                //**6.-Que el CSD del Emisor no haya sido revocado, utilizando la lista de LCO
                errores.Add(validadorDatos.VerificaCSDRevocado(serieCert, fecha));
                //7.-Que la fecha de emisión esté dentro de la vigencia del CSD del Emisor
                errores.Add(validadorDatos.ValidaFechaEmisionXml(fechaEmisionCfdi, datFechaExpiracionCSD, datFechaEfectivaCSD));
                //8.-Que no contenga un timbre previo

                //Cambios de Jorge Arce para Steren
                string cfdiTimbrado = null;
                string uuidDuplicado = null;
                var duplicado = validadorDatos.ValidaTimbrePrevio(xe, hash, ref cfdiTimbrado,ref uuidDuplicado);
                errores.Add(duplicado);
                if (duplicado == 307)
                {
                    timbre = new TimbreFiscalDigital(){UUID = uuidDuplicado};
                    res = cfdiTimbrado;
                    return new Dictionary<int, string>();
                }
                
                //9.-Que el rango de la fecha de generación no sea mayor a 72 horas para la emisión del timbre
                errores.Add(validadorDatos.ValidaRangoFecha(fechaEmisionCfdi));
                //10.-Que exista el RFC del emisor conforme al régimen autorizado
                errores.Add(validadorDatos.ValidaRFCLCO(rfc));
                //11.-Que la fecha de emisión sea posterior al 01 de Enero 2011
                errores.Add(validadorDatos.ValidaFechaEmision2011(fechaEmisionCfdi));

                var erroresReales = errores.Where(l => l != 0);
                
                if (erroresReales.Any())
                {
                    erroresPac = CrearArchivoROE(erroresReales.ToList(), archivoEntrada);
                    return erroresPac;
                }
                  
                var genTimbre = new GeneradorTimbreFiscalDigital("SLOT", 666);
                string serieCertPac = ConfigurationManager.AppSettings["NoSerieCertPac"];
                string RFCPac = ConfigurationManager.AppSettings["RFCPac"];
                string Leyenda = "";

            

                timbre = genTimbre.GeneraTimbreFiscalDigitalCadenas(rfc, serieCertPac,sello, xe,uuid, RFCPac,Leyenda);


                if (timbre != null)
                {
                    Comprobante comprobante = DesSerializar(xe, cadenaOriginal);

                    string strTimbre = GetXmlTimbre(timbre);
                    // Envio al SAT
                    string xmlCompleto = ConcatenaTimbre(xe, strTimbre, addenda,rfc,uuid.ToString());
                    //res = timbre.xmlString;
                    string resSat = null;
                    res = xmlCompleto; //solo para probar
                    if (ConfigurationManager.AppSettings["EnvioSat"] == "true")
                    {
                        comprobante.Complemento = new ComprobanteComplemento { timbreFiscalDigital = timbre };
                        var enviadorSAT = new Enviador();
                        TimbreWs33 ws = new TimbreWs33
                        {
                            Uuid = timbre.UUID,
                            Xml = xmlCompleto,
                            RfcEmisor = rfc,
                            RfcReceptor = comprobante.Receptor.Rfc,
                            FechaFactura =Convert.ToDateTime(comprobante.Fecha),
                            Hash = hash.Replace("-", "")

                        };

                        if (enviadorSAT.EnvioSAT(comprobante, ws, cadenaOriginal))
                        {

                            timbre.SelloSAT = "SELLO INVALIDO, AMBIENTE DE PRUEBAS";
                            acuseSat = resSat;
                            //var timbreXml = _serializadorTimbres.GetTimbreXml(timbre);
                            string xml = xmlCompleto;//ConcatenaTimbre(xe, timbreXml, addenda,rfc, uuid.ToString());
                            res = xml;
                        }
                        else
                        {
                            acuseSat = resSat;
                            res = resSat;
                            erroresPac.Add(101, "Error al enviar al SAT");
                            return erroresPac;
                        }
                    }
                    return erroresPac;

                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                this.CrearArchivoROE(new List<int> { 666 }, archivoEntrada, ex.Message);
                throw;
            }
            return erroresPac;
        }

        private string rutaTimbrado = ConfigurationManager.AppSettings["RutaTimbrado"];
        private string ConcatenaTimbre(XElement entrada, string xmlFinal, XElement addenda, string rfc, string uuid)
        {
            XElement timbre = XElement.Load(new StringReader(xmlFinal));
            var complemento = entrada.Elements(Constantes.CFDVersionNamespace + "Complemento").FirstOrDefault();
            if (complemento == null)
            {
                entrada.Add(new XElement(Constantes.CFDVersionNamespace + "Complemento"));
                complemento = entrada.Elements(Constantes.CFDVersionNamespace + "Complemento").FirstOrDefault();
            }
            //else
            //{
            //    complemento.Remove();
            //}
            complemento.Add(timbre);
            //entrada.Add(complemento);
            if (addenda != null)
            {
                //XElement add = new XElement(Constantes.CFDVersionNamespace + "Addenda");//XElement.Parse(addenda.ToString());
                //add.Elements()
                //add.Name = ;
                
                entrada.Add(addenda);

            }

            var tw = new SidetecStringWriter(Encoding.UTF8);
            entrada.Save(tw, SaveOptions.DisableFormatting);
            string xml = tw.ToString();
            //var directorio =Path.Combine(rutaTimbrado,rfc, DateTime.Now.ToString("yyyyMMdd"));
            //if (!Directory.Exists(directorio))
            //    Directory.CreateDirectory(directorio);
            //var fileName = Path.Combine(directorio,
            //                            "Comprobante_" + uuid + ".xml");
            //using (StreamWriter sw = new StreamWriter(fileName, false,Encoding.UTF8))
            //{
            //    sw.Write(xml);
            //}            
            return xml;
        }

        private string GetXmlTimbre(TimbreFiscalDigital p)
        {
            XmlSerializer ser = new XmlSerializer(typeof(TimbreFiscalDigital));
            using (MemoryStream memStream = new MemoryStream())
            {
                var sw = new StreamWriter(memStream, Encoding.UTF8);
                using (XmlWriter xmlWriter = XmlWriter.Create(sw, new XmlWriterSettings() { Indent = false, Encoding = Encoding.UTF8 }))
                {
                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                    namespaces.Add("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
                    ser.Serialize(xmlWriter, p, namespaces);
                    string xml = Encoding.UTF8.GetString(memStream.GetBuffer());
                    xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
                    xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
                    return xml;
                }
            }
        }

        private readonly XNamespace _ns = "http://www.sat.gob.mx/cfd/3";

        //private string ConcatenaTimbre(XElement entrada, string xmlTimbre)
        //{
        //    XElement timbre = XElement.Load(new StringReader(xmlTimbre));
        //    var complemento = entrada.Elements(_ns + "Complemento").FirstOrDefault();
        //    if (complemento == null)
        //    {
        //        entrada.Add(new XElement(_ns + "Complemento"));
        //        complemento = entrada.Elements(_ns + "Complemento").FirstOrDefault();
        //    }
        //    complemento.Add(timbre);
        //    MemoryStream mem = new MemoryStream();
        //    StreamWriter tw = new StreamWriter(mem, Encoding.UTF8);
        //    //XmlWriter xmlWriter = XmlWriter.Create(tw,
        //    //                                     new XmlWriterSettings() {Indent = false, Encoding = Encoding.UTF8});
        //    entrada.Save(tw, SaveOptions.DisableFormatting);
        //    string xml = Encoding.UTF8.GetString(mem.GetBuffer());
        //    xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
        //    xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
        //    return xml;
        //}




        private Dictionary<int, string> CrearArchivoROE(IEnumerable<int> errores, string archivoEntrada, string extraInfo = "")
        {
            Dictionary<int, string> resultado = new Dictionary<int, string>();
            var errorOutput = new StringBuilder();

            errorOutput.AppendLine("Archivo Invalido");
            errorOutput.AppendLine("Path: " + archivoEntrada);
            foreach (int error in errores)
            {
                resultado.Add(error, Constantes.ErroresValidacion[error] + (extraInfo == "" ? "" : " - Extra: " + extraInfo));
            }
            return resultado;
        }

        private Comprobante DesSerializar(XElement element, string cadena)
        {
            var ser = new XmlSerializer(typeof(Comprobante));
            string xml = element.ToString();
            var reader = new StringReader(xml);
            var comLXMLComprobante = (Comprobante)ser.Deserialize(reader);
            comLXMLComprobante.XmlString = xml;
            comLXMLComprobante.CadenaOriginal = cadena;
            return comLXMLComprobante;
        }
    }
}
