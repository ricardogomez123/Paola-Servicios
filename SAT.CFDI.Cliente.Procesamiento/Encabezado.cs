using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SAT.CFDI.Cliente.Procesamiento
{
    public class Encabezado
    {
       

        public Encabezado(string strPRfc, string versionx, string strPNumCert, string strPUUID,
                          DateTime datPFecha, string strPXml)
        {
            RfcEmisor = strPRfc;
            version = versionx;
            NumeroCertificado = strPNumCert;
            UUID = strPUUID;
            Fecha = datPFecha;
            Xml = strPXml;
        }

        public Encabezado(string strPRfc, string fecha, IList iliPLista)
        {
            Fecha = DateTime.Parse(fecha);
            this.FechaString = fecha;
            this.LisMListaFolios = iliPLista;
            RfcEmisor = strPRfc;
        }

        public IList LisMListaFolios { get; set; }

        IList _iliMLista;

        public IList IliMLista
        {
            get { return _iliMLista; }
            set { _iliMLista = value; }
        }

        string _RfcEmisor;

        public string RfcEmisor
        {
            get { return _RfcEmisor; }
            set { _RfcEmisor = value; }
        }

        string _version;

        public string version
        {
            get { return _version; }
            set { _version = value; }
        }

        string _NumeroCertificado;

        public string NumeroCertificado
        {
            get { return _NumeroCertificado; }
            set { _NumeroCertificado = value; }
        }

        string _UUID;

        public string UUID
        {
            get { return _UUID; }
            set { _UUID = value; }
        }

        DateTime _Fecha;

        public DateTime Fecha
        {
            get { return _Fecha; }
            set { _Fecha = value; }
        }

        public string FechaString { get; set; }

        string _xml;

        public string Xml
        {
            get { return _xml; }
            set { _xml = value; }
        }

    }
}
