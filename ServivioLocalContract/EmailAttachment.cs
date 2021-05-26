using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServicioLocalContract
{
    public class EmailAttachment
    {
        public byte[] Attachment { get; set; }
        public string Name { get; set; }
    }
}
