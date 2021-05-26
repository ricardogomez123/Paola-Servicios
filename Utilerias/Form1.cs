using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using PACEnviadorSATConsole;
using ServicioLocal.Business;

namespace Utilerias
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static string InsertaCfdi(string file)
        {
            //var salida = new StreamWriter("command.sql");
            GeneradorCadenas gen = new GeneradorCadenas();
            string sql = "INSERT INTO [dbo].[TimbreWs] ([FechaFactura] " +
                         ",[RfcEmisor] ,[RfcReceptor] " +
                         ",[Xml] ,[Uuid] ,[Status], [Hash]) VALUES (";

            string comprobante = null;

            comprobante = File.ReadAllText(file, Encoding.UTF8);

            var cadena = gen.CadenaOriginal(comprobante);
            SHA1Managed sha = new SHA1Managed();

            var hash = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(cadena))).Replace("-", "");
            var reader = XmlReader.Create(new StringReader(comprobante));
            string emisor = null;
            string receptor = null;
            string fechaC = null;
            string uuid = null;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.LocalName == "Emisor")
                    {
                        emisor = reader.GetAttribute("rfc");


                    }
                    if (reader.LocalName == "Receptor")
                        receptor = reader.GetAttribute("rfc");
                    if (reader.LocalName == "Comprobante")
                    {
                        var f = Convert.ToDateTime(reader.GetAttribute("fecha"));
                        fechaC = f.ToString("yyyy-MM-dd hh:mm:ss");



                    }
                    if (reader.LocalName == "TimbreFiscalDigital")
                    {
                        uuid = reader.GetAttribute("UUID");
                    }

                }

            }
            var command = sql + "'" + fechaC + "'," +
                          "'" + emisor + "'," +
                          "'" + receptor + "'," +
                          "'" + "" + "'," +
                          "'" + uuid + "'," + "0," +
                          "'" + hash + "');";
            Console.WriteLine(command);
            Console.WriteLine("--------------------------------------------------");
            return command;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Xml|*.xml;*.XML;";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach(var file in ofd.FileNames)
                {
                    txtSql.AppendText(InsertaCfdi(file));
                    txtSql.AppendText("\r\n");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtId.Text))
            {
                NtLinkTimbrado tim = new NtLinkTimbrado();
                var timbre = tim.ObtenerTimbre(int.Parse(txtId.Text));
                ProcesoEnvioSAT envio = new ProcesoEnvioSAT();
                var res = envio.EnvioSAT(timbre);
                txtLogEnvio.Text = "Enviado: " + res.ToString();
            }


        }
    }
}
