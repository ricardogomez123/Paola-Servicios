using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClienteCertificador.ServicioTimbrado;
using ClienteCertificador.Timbrador;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.ServiceModel;
using ClienteCertificador.Validador;

namespace ClienteCertificador
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }


        private string ConcatenaComplemento(XElement entrada, string xmlFinal)
        {
            XElement timbre = XElement.Load(new StringReader(xmlFinal));
            var complemento = entrada.Elements(Constantes.CFDVersionNamespace + "Complemento").FirstOrDefault();
            if (complemento == null)
            {
                entrada.Add(new XElement(Constantes.CFDVersionNamespace + "Complemento"));
                complemento = entrada.Elements(Constantes.CFDVersionNamespace + "Complemento").FirstOrDefault();
            }
            complemento.Add(timbre);
            var mem = new MemoryStream();
            var tw = new StreamWriter(mem, Encoding.UTF8);
            entrada.Save(tw, SaveOptions.DisableFormatting);
            string xml = Encoding.UTF8.GetString(mem.GetBuffer());
            xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
            xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
            return xml;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Comprobantes|*.xml";
            ofd.Multiselect = true;
            DialogResult dlg = ofd.ShowDialog();
            if (dlg == DialogResult.OK)
            {
                foreach (string f in ofd.FileNames)
                {
                    

                    LbArchivos.Items.Add(f);
                }
            }

            
        }

       

        private void button3_Click(object sender, EventArgs e)
        {
            using (Timbrador.CertificadorClient client = new Timbrador.CertificadorClient())
            {
                try
                {
                    foreach (var archivo in LbArchivos.Items)
                    {
                        string timbre;
                        string cfdi = File.ReadAllText(archivo.ToString());
                        timbre = client.TimbraCfdi(cfdi);
                        XElement xeCfdi = XElement.Load(new StringReader(cfdi));
                        textBox1.AppendText(archivo.ToString() + "\r\n--------------------------------------------------------------\r\n");
                        textBox1.AppendText(archivo.ToString() + " -> " + timbre + "\r\n");
                        try
                        {
                            string cfdiTimbrado = ConcatenaComplemento(xeCfdi, timbre);
                            File.WriteAllText(archivo.ToString() + ".timbre.xml", cfdiTimbrado, Encoding.UTF8);
                        }
                        catch (Exception ee)
                        {
                            ;
                        }
                    }
                    LbArchivos.Items.Clear();
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            Timbrador.CertificadorClient client = new Timbrador.CertificadorClient();
            try
            {
                foreach (var archivo in TxtUuids.Lines)
                {
                    
                    string[] datos = archivo.Split('\t');
                    string res = client.CancelaCfdi(datos[0], datos[1]);
                    TxtResultCancelar.AppendText(res + " -> " + res + "\r\n");
                }
                LbArchivos.Items.Clear();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
            finally
            {
                client.Close();
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            ServicioTimbrado.ServicioTimbradoClient cliente = new ServicioTimbradoClient();
            try
            {
                foreach (var archivo in LbArchivos.Items)
                {
                    string cfdi = File.ReadAllText(archivo.ToString());
                    var timbre = cliente.TimbraCfdiQr(txtUsuario.Text, txtPassword.Text, cfdi);
                    textBox1.AppendText(archivo.ToString() + "\r\n--------------------------------------------------------------\r\n");
                    textBox1.AppendText(archivo.ToString() + " -> " + timbre.Valido + "\r\n");
                    textBox1.AppendText(archivo.ToString() + " -> " + timbre.Cfdi + "\r\n");
                    textBox1.AppendText(archivo.ToString() + " -> " + timbre.DescripcionError + "\r\n");
                    //textBox1.AppendText(archivo.ToString() + " -> " + timbre.CadenaTimbre + "\r\n");
                    //textBox1.AppendText(archivo.ToString() + " -> " + timbre.DescripcionError + "\r\n");

                    try
                    {
                        File.WriteAllText(archivo.ToString() + ".timbre.txt", timbre.Cfdi, Encoding.UTF8);
                        File.WriteAllBytes(archivo.ToString() + "Qr.bmp" , Convert.FromBase64String(timbre.QrCodeBase64));
                    }
                    catch(FaultException fe)
                    {
                        textBox1.AppendText(fe + "\r\n");
                    }
                    catch (Exception ee)
                    {
                        ;
                    }
                }
                LbArchivos.Items.Clear();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
            finally
            {
                cliente.Close();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var client = new ServicioTimbrado.ServicioTimbradoClient();
            try
            {
                foreach (var archivo in TxtUuids.Lines)
                {
                    string res = client.CancelaCfdi(txtUsuario.Text, txtPassword.Text ,archivo, TxtRfc.Text);
                    TxtResultCancelar.AppendText(archivo.ToString() + " -> " + res + "\r\n");
                }
                LbArchivos.Items.Clear();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
            finally
            {
                client.Close();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var client = new ServicioTimbrado.ServicioTimbradoClient();
            try
            {
                var datos = client.ObtenerEmpresas(txtUsuario.Text, txtPassword.Text);
               
                dataGridView1.DataSource = datos;
                var sistema = client.ObtenerDatosCliente(txtUsuario.Text, txtPassword.Text);
                lblTimbres.Text ="Contratados: " + sistema.TimbresContratados + "Consumidos: " + sistema.TimbresConsumidos;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
            finally
            {
                client.Close();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ServicioValidadorClient cliente = new ServicioValidadorClient();
            try
            {
                foreach (var archivo in LbArchivos.Items)
                {
                    string cfdi = File.ReadAllText(archivo.ToString());
                    var timbre = cliente.Validar(txtUsuario.Text, txtPassword.Text, cfdi);
                    textBox1.AppendText(archivo.ToString() + " -> " + timbre.Valido + "\r\n");
                    foreach (Validacion validacion in timbre.Detalles)
                    {
                        textBox1.AppendText(validacion.Descripcion + "->" + validacion.Error + "\r\n");
                    }
                    
                }
                LbArchivos.Items.Clear();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
            finally
            {
                cliente.Close();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ServicioTimbrado.ServicioTimbradoClient cliente = new ServicioTimbradoClient();
            try
            {
                foreach (var archivo in LbArchivos.Items)
                {
                    string cfdi = File.ReadAllText(archivo.ToString(), Encoding.UTF8);
                    var timbre = cliente.TimbraCfdi(txtUsuario.Text, txtPassword.Text, cfdi);
                    //XElement xeCfdi = XElement.Load(new StringReader(cfdi));
                    textBox1.AppendText(archivo.ToString() + "\r\n--------------------------------------------------------------\r\n");
                    textBox1.AppendText(archivo.ToString() + " -> " + timbre + "\r\n");
                    //textBox1.AppendText(archivo.ToString() + " -> " + timbre.CadenaTimbre + "\r\n");
                    //textBox1.AppendText(archivo.ToString() + " -> " + timbre.DescripcionError + "\r\n");

                    try
                    {
                        //File.WriteAllText(archivo.ToString() + ".timbre.txt", timbre.Cfdi, Encoding.UTF8);
                       // File.WriteAllBytes(archivo.ToString() + "Qr.bmp", Convert.FromBase64String(timbre.QrCodeBase64));
                    }
                    catch (FaultException fe)
                    {
                        textBox1.AppendText(fe + "\r\n");
                    }
                    catch (Exception ee)
                    {
                        ;
                    }
                }
                LbArchivos.Items.Clear();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
            finally
            {
                cliente.Close();
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ServicioTimbrado.ServicioTimbradoClient cliente = new ServicioTimbradoClient();
            try
            {
                foreach (var archivo in LbArchivos.Items)
                {
                    string cfdi = File.ReadAllText(archivo.ToString(), Encoding.UTF8);
                    var timbre = cliente.CancelaCfdiRequest(txtUsuario.Text, txtPassword.Text, cfdi);
                    //XElement xeCfdi = XElement.Load(new StringReader(cfdi));
                    textBox1.AppendText(archivo.ToString() + "\r\n--------------------------------------------------------------\r\n");
                    textBox1.AppendText(archivo.ToString() + " -> " + timbre + "\r\n");
                    //textBox1.AppendText(archivo.ToString() + " -> " + timbre.CadenaTimbre + "\r\n");
                    //textBox1.AppendText(archivo.ToString() + " -> " + timbre.DescripcionError + "\r\n");

                    try
                    {
                        //File.WriteAllText(archivo.ToString() + ".timbre.txt", timbre.Cfdi, Encoding.UTF8);
                        // File.WriteAllBytes(archivo.ToString() + "Qr.bmp", Convert.FromBase64String(timbre.QrCodeBase64));
                    }
                    catch (FaultException fe)
                    {
                        textBox1.AppendText(fe + "\r\n");
                    }
                    catch (Exception ee)
                    {
                        ;
                    }
                }
                LbArchivos.Items.Clear();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
            finally
            {
                cliente.Close();
            }
        }
    }
}
