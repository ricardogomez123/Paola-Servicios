using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;

namespace HSMXml
{
    class ConverterSidetec : X509DefaultEntryConverter
    {
        public override Org.BouncyCastle.Asn1.Asn1Object GetConvertedValue(Org.BouncyCastle.Asn1.DerObjectIdentifier oid, string value)
        {
            Asn1Object obj = base.GetConvertedValue(oid, value);
            if (obj is DerUtf8String)
		    return new DerPrintableString(value);
		else
		    return obj;
        }
    }
}
