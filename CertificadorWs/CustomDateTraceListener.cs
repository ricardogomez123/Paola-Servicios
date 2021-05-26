//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CertificadorWs
{

    public class CustomDateTraceListener : XmlWriterTraceListener
    {

        private DateTime _now;
        private readonly string _fileName;
        private readonly string _extension;
        private readonly string _path;
        #region Member Functions

        

        private void DetermineOverQuota()
        {

            lock(Writer)
            {
                if (DateTime.Now.DayOfYear != _now.DayOfYear)
                {
                    base.Flush();
                    var newFIle = Path.Combine(_path, _fileName + "_" + DateTime.Now.ToString("yyyy-MM-dd") + _extension);
                    var dir = Path.GetDirectoryName(Path.GetFullPath(newFIle));
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                
                    Writer.Close();
                    this.Writer = new StreamWriter(newFIle,true,Encoding.UTF8);
                    _now = DateTime.Now;
                }
            }
        }

        #endregion               

        #region XmlWriterTraceListener Functions

        public CustomDateTraceListener(string file)
            : base(file )
        {
            Flush();
            file = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(file)),
                                Path.GetFileName(file) + "_" + DateTime.Now.ToString("yyyy-MM-dd") +
                                Path.GetExtension(file));
            var dir = Path.GetDirectoryName(Path.GetFullPath(file));
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            Writer.Close();
            this.Writer = new StreamWriter(file,true,Encoding.UTF8);
            _now = DateTime.Now;
            _fileName = Path.GetFileName(file);
            _path = Path.GetDirectoryName(Path.GetFullPath(file));
            _extension = Path.GetExtension(file);
        }




        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            DetermineOverQuota();
            base.TraceData(eventCache, source, eventType, id, data);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            DetermineOverQuota();
            base.TraceEvent(eventCache, source, eventType, id);
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            DetermineOverQuota();
            base.TraceData(eventCache, source, eventType, id, data);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            DetermineOverQuota();
            base.TraceEvent(eventCache, source, eventType, id, format, args);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            DetermineOverQuota();
            base.TraceEvent(eventCache, source, eventType, id, message);
        }

        public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
        {
            DetermineOverQuota();
            base.TraceTransfer(eventCache, source, id, message, relatedActivityId);

        }

        #endregion

    }

    
}
