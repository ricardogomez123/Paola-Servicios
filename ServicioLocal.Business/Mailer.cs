using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using System.Net.Mime;
using System.ServiceModel;
using System.Text;
using ServicioLocalContract;

namespace ServicioLocal.Business
{
    public class Mailer : NtLinkBusiness
    {
        readonly string _host;
        readonly string _port;
        readonly string _username;
        readonly string _password;
        public string Bcc { get; set; }
        
        public Mailer()
        {
            _host = ConfigurationManager.AppSettings["Host"];
            _port = ConfigurationManager.AppSettings["Port"];
            _username = ConfigurationManager.AppSettings["UserName"];
            _password = ConfigurationManager.AppSettings["Password"];
        }

        public void Send(List<string> recipients, List<EmailAttachment> attachments, string message, string subject, string fromEmail, string fromDescription)
        {
            try
            {
                Logger.Debug("Enviando a " + recipients.Count + " emails");
                var client = new SmtpClient
                {
                    Host = this._host,
                    Port = int.Parse(this._port),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(this._username, this._password)
                };
                Logger.Info("Creando MailMessage");
                var mailMsg = new MailMessage
                {
                    Sender = new MailAddress(_username),
                    From = new MailAddress(fromEmail, fromDescription),
                    Subject = subject,
                    DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess,
                    Body = message,
                    BodyEncoding = Encoding.UTF8
                };

                Logger.Info("Cargando Bcc: " + Bcc);
                if (!string.IsNullOrEmpty(Bcc))
                    mailMsg.Bcc.Add( new MailAddress(Bcc));
                mailMsg.Headers.Add("Disposition-Notification-To", _username);
                mailMsg.IsBodyHtml = true;
                int i = 0;

                Logger.Info("Agregando attach");

                foreach (EmailAttachment attachment in attachments)
                {
                    MemoryStream mStream = new MemoryStream();

                    mStream.Write(attachment.Attachment, 0, attachment.Attachment.Length);
                    mStream.Position = 0;
                    mailMsg.Attachments.Add(new Attachment(mStream,attachment.Name));
                }
                foreach (var recipient in recipients)
                {
                    Logger.Info("Enviando a la direccion: " + recipient);
                    if (!string.IsNullOrEmpty(recipient))
                        mailMsg.To.Add(new MailAddress(recipient));
                }
                client.Send(mailMsg);
                Logger.Debug("Enviado correctamente");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new FaultException("Ocurrio un error al enviar el correo");
            }

        }



        public void Send(List<string> recipients, List<string> attachments, string message, string subject,string fromEmail, string fromDescription)
        {
            try
            {
                var client = new SmtpClient
                {
                    Host = this._host,
                    Port = int.Parse(this._port),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(this._username, this._password)
                };

                var mailMsg = new MailMessage
                {
                    Sender = new MailAddress(_username),
                    From = new MailAddress(fromEmail, fromDescription),
                    Subject = subject,
                    DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess,
                    Body = message,
                    BodyEncoding = Encoding.UTF8
                };
                mailMsg.Headers.Add("Disposition-Notification-To", _username);
                mailMsg.IsBodyHtml = true;
                foreach (string attachment in attachments)
                {
                    mailMsg.Attachments.Add(new Attachment(attachment));
                }
                foreach (var recipient in recipients)
                {
                    mailMsg.To.Add(new MailAddress(recipient));
                }
                client.Send(mailMsg);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new FaultException("Ocurrio un error al enviar el correo");
            }
            
        }
    }
}
