using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

namespace CertificadorWs.Business.Retenciones
{
    public class ValidadorEstructuraRetenciones : ICloneable
    {
        XmlReaderSettings settings = new XmlReaderSettings();
        public ValidadorEstructuraRetenciones()
        {
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            var ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XsdRet");
            foreach (string schemaFile in Directory.EnumerateFiles(ruta).OrderBy(p => p))
            {
                string archivoXsd = schemaFile;
                settings.Schemas.Add(null, archivoXsd);
            }
        }



        public ValidacionEstructuraRetencion Validar(string xmlRetenciones)
        {
            var result = new ValidacionEstructuraRetencion();
            result.ErrorList = new List<string>();
            result.Valido = true;
            settings.ValidationEventHandler += (s, a) =>
            {
                XmlReader r = (XmlReader)s;
                result.ErrorList.Add(r.Name + " - " + a.Message);
                result.Valido = false;
            };
            XmlReader reader = XmlReader.Create(new StringReader(xmlRetenciones), settings);
            while (reader.Read())
            {
            }

            return result;
        }

        public object Clone()
        {
            var s = settings.Clone();
            var o = this.MemberwiseClone() as ValidadorEstructuraRetenciones;
            o.settings = s;
            return o;
        }
    }
}
