using System;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using ServicioLocal.Business;

namespace CertificadorWs.Business
{
    public class SerializadorTimbres
    {
        private XmlSerializer ser;
        private XmlSerializer ser2;

        private XmlSerializerNamespaces namespaces;
        
        public SerializadorTimbres()
        {
            ser = new XmlSerializer(typeof(TimbreFiscalDigital));
            ser2 = new XmlSerializer(typeof(ServicioLocal.Business.TimbreRetenciones.TimbreFiscalDigital));
            namespaces = new XmlSerializerNamespaces();

            namespaces.Add("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
        }

        public string GetTimbreXml(TimbreFiscalDigital p)
        { 

            MemoryStream memStream;
            memStream = new MemoryStream();
            TextWriter xmlWriter;
            xmlWriter = new StreamWriter(memStream, Encoding.UTF8);
            ser.Serialize(xmlWriter, p, namespaces);
            xmlWriter.Close();
            memStream.Close();
            string xml = Encoding.UTF8.GetString(memStream.GetBuffer());
            xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
            xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
            xml = xml.Replace("'", "&apos;");
            xml = xml.Replace("http://www.sat.gob.mx/TimbreFiscalDigital TimbreFiscalDigital.xsd", "http://www.sat.gob.mx/TimbreFiscalDigital http://www.sat.gob.mx/TimbreFiscalDigital/TimbreFiscalDigital.xsd");
            return xml;
        }
        public string GetTimbreRenecionesXml(ServicioLocal.Business.TimbreRetenciones.TimbreFiscalDigital p)
        {
            MemoryStream memStream = new MemoryStream();
            TextWriter xmlWriter = new StreamWriter(memStream, Encoding.UTF8);
            this.ser2.Serialize(xmlWriter, p, this.namespaces);
            xmlWriter.Close();
            memStream.Close();
            string xml = Encoding.UTF8.GetString(memStream.GetBuffer());
            xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
            xml = xml.Substring(0, xml.LastIndexOf(Convert.ToChar(62)) + 1);
            xml = xml.Replace("'", "&apos;");
           // return xml.Replace("http://www.sat.gob.mx/TimbreFiscalDigital TimbreFiscalDigital.xsd", "http://www.sat.gob.mx/sitio_internet/cfd/TimbreFiscalDigital/TimbreFiscalDigital.xsd");
            return xml;
        }
    }
}
