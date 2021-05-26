using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using PruebasUnitarias.NtLink;
using ServicioLocal.Business;
using ServicioLocalContract;

namespace PruebasUnitarias
{
    class Program
    {
        static void Main(string[] args)
        {
            MensajeCancelacion msg = new MensajeCancelacion();
            var msgCan = msg.CerarMensajeCancelacion(new List<string>() {"6726DB4B-6CFA-4F8D-9561-3EF1D20266E1"}, "SID080303VE0",
                @"C:\Sidetec\Resources\SID080303VE0\Certs\csd.key", @"C:\Sidetec\Resources\SID080303VE0\Certs\csd.cer",
                "Side1313");
            var cte = new ServicioTimbradoClient();
            var res = cte.CancelaCfdiRequest("jorge.arce@sidetec.com.mx", "Sidetec.08", msgCan);
            Console.WriteLine(res.Acuse);
            Console.ReadKey();
        }
    }
}
