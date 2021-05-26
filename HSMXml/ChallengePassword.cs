using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;


namespace HSMXml
{
    class ChallengePassword : AttributePkcs
    {
        public ChallengePassword(string pass) :base(PkcsObjectIdentifiers.Pkcs9AtChallengePassword, ToSet(pass))
        {

        }


        private static Asn1Set ToSet (string password)
        {
            Asn1EncodableVector v = new Asn1EncodableVector();
            v.Add(new DerPrintableString(password));
            return new DerSet(v);
        }
    }
}
