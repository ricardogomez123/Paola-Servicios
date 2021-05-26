using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml;
using System.Configuration;
using ServicioLocal.Business;
using log4net;

namespace CertificadorWs.Business
{
    public class ValidadorEstructura : NtLinkBusiness, ICloneable
    {
        

        public ValidadorEstructura()
        {

        }

        public ValidadorEstructura(bool c)
        {
            _settings = new XmlReaderSettings();
            _settings.ValidationType = ValidationType.Schema;
            //_settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            _settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

            var ruta = ConfigurationManager.AppSettings["RutaXsd"];
            foreach (string schemaFile in Directory.EnumerateFiles(ruta))
            {
                string archivoXsd =  schemaFile;
                _settings.Schemas.Add(null, archivoXsd);
            }
        }

        private XmlReaderSettings _settings;


       





        public ValidadorInput Validate2(string xml)
        {
            try
            {
                var result = new ValidadorInput() {ErroresEstructura = new StringBuilder()};
                _settings.ValidationEventHandler += (s, a) =>
                {
                    XmlReader r = (XmlReader)s;
                    result.ErroresEstructura.AppendLine(r.Name + " - " + a.Message);
                };
                XmlReader reader = XmlReader.Create(new StringReader(xml), _settings);
                //result.ErroresEstructura = new List<string>();

                try
                {
                    while (reader.Read())
                    {
                    }
                    if (string.IsNullOrEmpty(result.ErroresEstructura.ToString()))
                        result.Valido = 0;
                    else result.Valido = 301;
                    return result;
                }
                catch (Exception ee)
                {
                    Logger.Error(ee);
                    return null;
                }

            }
            catch (Exception ee)
            {
                Logger.Error(ee);
                return null;
            }

        }

        

        public object Clone()
        {
            var c = this.MemberwiseClone() as ValidadorEstructura;
            c._settings = _settings.Clone();
            return c;
        }

        public void Dispose()
        {
            _settings = null;
            //GC.SuppressFinalize(this);
        }

        
    }

    public class ValidadorInput
    {
        public StringBuilder ErroresEstructura { get; set; }
        public int Valido { get; set; }
    }
}
