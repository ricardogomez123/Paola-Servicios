

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
    public class ValidarINE : NtLinkBusiness
    {
   

        public ValidarINE()
        {
            XmlConfigurator.Configure();
        }
        //---------------------------------------------------------------------------------------------
        public string ProcesarINE(INE ine)
        {
            
            if (ine.TipoProceso == INETipoProceso.Ordinario)
            {
                if (ine.TipoComiteSpecified == false)
                    return ("180 - Atributo TipoProceso: con valor {Ordinario} , debe existir el atributo ine:TipoComite");

            }
            else
            {

              if(ine.Entidad==null)
                  return ("181 - Atributo TipoProceso: con el valor {Precampaña} o el valor {Campaña}, debe existir al menos un elemento Entidad:Ambito");
             if(ine.Entidad.Count()==0)
                 return ("181 - Atributo TipoProceso: con el valor {Precampaña} o el valor {Campaña}, debe existir al menos un elemento Entidad:Ambito");
          
                foreach (var en in ine.Entidad)
              {
                  if (en.AmbitoSpecified == false)
                  {
                      return ("181 - Atributo TipoProceso: con el valor {Precampaña} o el valor {Campaña}, debe existir al menos un elemento Entidad:Ambito");
                      
                  }
              }
                if(ine.TipoComiteSpecified==true)
                    return ("182 - Atributo TipoProceso: con el valor {Precampaña} o el valor {Campaña}, no debe existir ine:TipoComite");
                if (ine.IdContabilidadSpecified == true)
                    return ("183 - Atributo TipoProceso: con el valor {Precampaña} o el valor {Campaña}, no debe existir ine:IdContabilidad");
            
            }
            if (ine.TipoComiteSpecified == true)
            {
                if (ine.TipoComite == INETipoComite.EjecutivoNacional)
                {
                    if (ine.Entidad != null)
                        return ("184 - Atributo TipoComite, con valor {Ejecutivo Nacional}, no debe existir ningún elemento ine:Entidad");
                }
                if (ine.TipoComite == INETipoComite.EjecutivoEstatal)
                {
                    if (ine.IdContabilidadSpecified == true)
                        return ("185 - Atributo TipoComite, con valor {Ejecutivo Estatal} , no debe existir ine:idContabilidad");
                }
                if (ine.TipoComite == INETipoComite.EjecutivoEstatal || ine.TipoComite == INETipoComite.DirectivoEstatal)
                {
                    if (ine.Entidad == null)
                        return ("186 - El TipoComite es Ejecutivo Estatal o Directivo Estatal, por lo que debe existir al menos un elemento Entidad y en ningún caso debe existir Ambito");
                    foreach (var en in ine.Entidad)
                    {
                        if (en.AmbitoSpecified == true)
                        {
                            return ("186 - El TipoComite es Ejecutivo Estatal o Directivo Estatal, por lo que debe existir al menos un elemento Entidad y en ningún caso debe existir Ambito");

                        }
                    }
            
                }
            }
            List<EntidadAC> Lista1 = new List<EntidadAC>();
            if(ine.Entidad!=null)
            foreach (var en in ine.Entidad)
            {
                var itemExists = Lista1.Exists(element => element.Ambito == en.Ambito.ToString() && element.ClaveEntidad== en.ClaveEntidad.ToString());
                if (!itemExists)
                {
                    EntidadAC AC = new EntidadAC();
                    AC.ClaveEntidad = en.ClaveEntidad.ToString();
                    AC.Ambito = en.Ambito.ToString();
                    Lista1.Add(AC);

                }
                else
                    return ("187 - Elemento Entidad, no se debe repetir la combinación de ine:Entidad:ClaveEntidad  con ine:Entidad:Ambito");

                if(en.AmbitoSpecified==true)
                if (en.Ambito == INEEntidadAmbito.Local)
                {
                    if (en.ClaveEntidad == t_ClaveEntidad.NAC ||en.ClaveEntidad == t_ClaveEntidad.CR1 ||
                        en.ClaveEntidad == t_ClaveEntidad.CR2 ||en.ClaveEntidad == t_ClaveEntidad.CR3 ||
                        en.ClaveEntidad == t_ClaveEntidad.CR4 ||en.ClaveEntidad == t_ClaveEntidad.CR5)
                    {
                        return ("188 - No se pueden seleccionar las claves  NAC, CR1, CR2, CR3, CR4 y CR5 por que el Ambito es Local");
                                 
                    }
                }


            }

            return "0";

        }
        //-------------------------------------------------------------------------------------
       

    }

    class EntidadAC
    {
        public string ClaveEntidad { get; set; }
        public string Ambito { get; set; }
    
    }
}