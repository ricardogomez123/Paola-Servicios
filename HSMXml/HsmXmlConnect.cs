using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using HSMXml.LunaXml;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using log4net;
using System.Linq;
using X509Certificate = System.Security.Cryptography.X509Certificates.X509Certificate;
using X509Extension = Org.BouncyCastle.Asn1.X509.X509Extension;

namespace HSMXml
{
    public class HsmXmlConnect
    {
        private static bool RemoteCertificateValidate(object sender, X509Certificate cert,X509Chain chain, SslPolicyErrors error)
        {
            return true;
        }
        
        public HsmXmlConnect()
        { 
            ServicePointManager.ServerCertificateValidationCallback
                  += RemoteCertificateValidate;
        }




        private bool Extraer(ref authTokenType token, string alias, ref byte[] modulus, ref byte[] exponent)
        {
            LunaXml.xmCryptoService client = new xmCryptoService();
            try
            {
                extract ex = new extract();
                ex.KeyAlias = alias;
                //ex.WrappingOptions = new WrappingOptionsType();
                ex.AuthToken = token;
                var res = client.extract(ex);
                token = res.AuthToken;
                if (res.Result.ResultMajor != null && res.Result.ResultMajor == "urn:oasis:names:tc:dss:resultmajor:Success")
                {
                    KeyValueType type = (KeyValueType)res.KeyInfo.Items[0];
                    RSAKeyValueType rsatype = (RSAKeyValueType) type.Item;
                    modulus = rsatype.Modulus;
                    exponent = rsatype.Exponent;
                    return true;
                }
                   
                else
                {
                    Log.Error((res.Result.ResultMessage != null ? res.Result.ResultMessage.Value : ""));
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error al crear la llave privada " + ex);
                return false;
            }
            finally
            {
                client.Dispose();
            }
        }


        private static readonly ILog Log = LogManager.GetLogger(typeof(HsmXmlConnect));
       
        
        
        public bool CreateSectretKey(ref authTokenType token,string keyAlias)
        {
            LunaXml.xmCryptoService client = new xmCryptoService();
            try
            {
                generateSecretKey sec = new generateSecretKey();
                sec.AuthToken = token;
                sec.KeyAlgorithm = KeyAlgorithmType.DES3;
                sec.KeyAlias = keyAlias;
                sec.KeySize = "1024";
                sec.ReturnKeyInfo = true;
                generateSecretKeyResponse res =  client.generateSecretKey(sec);
                token = res.AuthToken;
                if (res.Result.ResultMajor != null && res.Result.ResultMajor == "urn:oasis:names:tc:dss:resultmajor:Success")
                    return true;
                else 
                {
                    Log.Error((res.Result.ResultMessage != null ? res.Result.ResultMessage.Value: ""));
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error al crear la llave privada " + ex);
                return false;
            }
            finally
            {
                client.Dispose();
            }

        }


        public bool BorrarObjeto(ref authTokenType token, string alias)
        {
            LunaXml.xmCryptoService client = new xmCryptoService();
            try
            {
                deleteObject del = new deleteObject();
                del.AuthToken = token;
                del.ObjectAlias = alias;
                deleteObjectResponse res = client.deleteObject(del);
                token = res.AuthToken;
                if (res.Result.ResultMajor != null && res.Result.ResultMajor == "urn:oasis:names:tc:dss:resultmajor:Success")
                    return true;
                else return false;
                
            }
            catch (Exception ex)
            {
                Log.Error("Error al crear la llave privada " + ex);
                return false;
            }
            finally
            {
                client.Dispose();
            }
        }

        public Dictionary<string, string> SubirLlavePrivada1(ref authTokenType token, byte[] llave, string aliasLlave, string aliasCifrar)
        {
            string llaveAcifrar = Cifrar(ref token, Convert.ToBase64String(llave), aliasCifrar);
            string header =
               "<?xml version='1.0' encoding='utf-8'?><soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\">";
            string body = "<soapenv:Body><ns5:inject xmlns:ns5=\"http://xmCrypto.safenet-inc.com/xsd\">";
            string injectedKeyAlias = "<ns5:InjectedKeyAlias>" + aliasLlave + "</ns5:InjectedKeyAlias>";
            string keytoInject = "<KeyToInject xmlns=\"http://www.w3.org/2000/09/xmldsig#\">" +
                "<xmc:WrappedKey xmlns:xmc = \"http://xmCrypto.safenet-inc.com/xsd\"><xmc:WrappedKeyValue>" +
                llaveAcifrar + "</xmc:WrappedKeyValue><xmc:MaskingKeyAlias>" + aliasCifrar + "</xmc:MaskingKeyAlias>" +
                "<xmc:KeyAlgorithm>RSAPrivate</xmc:KeyAlgorithm><xmc:Mechanism>CBC</xmc:Mechanism>" +
                "<xmc:Padding>PKCS5Padding</xmc:Padding><xmc:Parameter><xmc:ParameterName>IV</xmc:ParameterName>" +
                "<xmc:ParameterValue>MTIzNDU2Nzg=</xmc:ParameterValue></xmc:Parameter></xmc:WrappedKey></KeyToInject>";
            string publicobject = "<ns5:PublicObject>false</ns5:PublicObject>";
            string politica = "<ns5:Policy><ns5:PolicyName>SetKeySpace</ns5:PolicyName><ns5:PolicyValue>soapAdmin</ns5:PolicyValue></ns5:Policy>";
            string tokenauth = "<ns5:AuthToken><ns5:SessionStateToken>" + token.SessionStateToken +
                               "</ns5:SessionStateToken></ns5:AuthToken></ns5:inject></soapenv:Body></soapenv:Envelope>";
            string soap = header + body + injectedKeyAlias + keytoInject + publicobject + politica + tokenauth;
            string res = Ejecutar(soap);
            var x = ExtraerTokendeRespuesta(res);
            return x;

        }



        public Dictionary<string, string> SubirLlavePrivada1(ref authTokenType token, string archivo, string password, string aliasLlave, string aliasCifrar)
        {
            byte[] bytesLlave = File.ReadAllBytes(archivo);
            byte[] bytesDescifrados = OpensslKey.DecodeEncryptedPrivateKeyInfoBytes(bytesLlave, password);
            string llaveAcifrar = Cifrar(ref token, Convert.ToBase64String(bytesDescifrados), aliasCifrar);
            string header =
               "<?xml version='1.0' encoding='utf-8'?><soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\">";
            string body = "<soapenv:Body><ns5:inject xmlns:ns5=\"http://xmCrypto.safenet-inc.com/xsd\">";
            string injectedKeyAlias = "<ns5:InjectedKeyAlias>" + aliasLlave + "</ns5:InjectedKeyAlias>";
            string keytoInject = "<KeyToInject xmlns=\"http://www.w3.org/2000/09/xmldsig#\">"+ 
                "<xmc:WrappedKey xmlns:xmc = \"http://xmCrypto.safenet-inc.com/xsd\"><xmc:WrappedKeyValue>" +
                llaveAcifrar + "</xmc:WrappedKeyValue><xmc:MaskingKeyAlias>" + aliasCifrar + "</xmc:MaskingKeyAlias>" +
                "<xmc:KeyAlgorithm>RSAPrivate</xmc:KeyAlgorithm><xmc:Mechanism>CBC</xmc:Mechanism>"+
                "<xmc:Padding>PKCS5Padding</xmc:Padding><xmc:Parameter><xmc:ParameterName>IV</xmc:ParameterName>"+
                "<xmc:ParameterValue>MTIzNDU2Nzg=</xmc:ParameterValue></xmc:Parameter></xmc:WrappedKey></KeyToInject>";
            string publicobject = "<ns5:PublicObject>false</ns5:PublicObject>";
            string politica = "<ns5:Policy><ns5:PolicyName>SetKeySpace</ns5:PolicyName><ns5:PolicyValue>soapAdmin</ns5:PolicyValue></ns5:Policy>";
            string tokenauth = "<ns5:AuthToken><ns5:SessionStateToken>" +  token.SessionStateToken +
                               "</ns5:SessionStateToken></ns5:AuthToken></ns5:inject></soapenv:Body></soapenv:Envelope>";
            string soap =  header + body + injectedKeyAlias + keytoInject + publicobject + politica + tokenauth;
            string res = Ejecutar(soap);
            var x = ExtraerTokendeRespuesta(res);
            return x;

        }


        public string Cifrar (ref authTokenType token, string texto, string alias )
        {
            LunaXml.xmCryptoService client = new xmCryptoService();
            try
            {
                encrypt en = new encrypt();
                en.DataToEncrypt = texto;
                en.AuthToken = token;
                en.EncryptionKeyAlias = alias;
                en.Mechanism = MechanismType.CBC;
                en.Padding = paddingType.PKCS5Padding;
                en.Parameter = new[] { new ParameterType() { ParameterName = "IV", ParameterValue = "MTIzNDU2Nzg=" } };
                encryptResponse res = client.encrypt(en);
                token = res.AuthToken;
                if (res.Result.ResultMajor != null && res.Result.ResultMajor == "urn:oasis:names:tc:dss:resultmajor:Success")
                    return res.EncryptedData;
                else
                {
                    Log.Error((res.Result.ResultMessage != null ? res.Result.ResultMessage.Value : ""));
                    return null;
                }
                
            }
            catch (Exception ex)
            {
                Log.Error("Error al intentar firmar: " + ex);
                return null;
            }
            finally
            {
                client.Dispose();
            }
        }



        public bool GenerarCsr(ref authTokenType token, string privateKeyalias, string publicKeyAlias, string subject, string fileName, string challenge )
        {
            LunaXml.xmCryptoService client = new xmCryptoService();
            try
            {
                byte[] modulus = new byte[]{};
                byte [] exponent = new byte[]{};
                this.Extraer(ref token, publicKeyAlias, ref modulus, ref exponent);
                RsaKeyParameters param = new RsaKeyParameters(false, new BigInteger(modulus), new BigInteger(exponent));
                DerSet derset = null;
                if (challenge != null)
                {
                    ChallengePassword chpass = new ChallengePassword(challenge);
                    derset= new DerSet(chpass);
                    //IList oid = new ArrayList();
                    //IList values = new ArrayList();
                    //oid.Add(PkcsObjectIdentifiers.Pkcs9AtChallengePassword);
                    //var pass = new DerPrintableString(challenge);
                    ////Asn1OctetString oct = pass.ToAsn1Object(); //new DerOctetString(pass);//Encoding.ASCII.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes("AABBccc22"))));
                    
                    //X509Extension ext = new X509Extension(false,new DerOctetString(pass.GetEncoded()));
                    //values.Add(pass);
                    //X509Extensions extensions = new X509Extensions(oid, values);
                    //derset = new DerSet(extensions.ToAsn1Object());
                }
                else
                {
                    derset = new DerSet();
                }
               
                //string sub =
                //"2.5.4.45=SAT970701NN3 / GATF730321GG5, SERIALNUMBER= / GATF730321HJCRRR01, O=SERVICIO DE ADMINISTRACION TRIBUTARIA, OU=PACNLC091211KC657202";
                //+ ", 1.2.840.113549.1.9.7= NtLink2012"
                X509Name sub = new X509Name(subject , new ConverterSidetec());
                Pkcs10CertificationRequestDelaySigned ds = new Pkcs10CertificationRequestDelaySigned("SHA1WITHRSA", sub, param, derset);
                string pafirmar = Convert.ToBase64String(ds.GetDataToSign());
                string firmados = Firmar(ref token, pafirmar, privateKeyalias, SignatureModeType.SHA1withRSA);
                byte[] bytes = Convert.FromBase64String(firmados);
                    
                ds.SignRequest(bytes);
                File.WriteAllBytes(fileName, ds.GetDerEncoded());
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
            finally
            {
                client.Dispose();
            }
            
        }



        public bool ValidarSello(string cadena, byte[] firma, string rutaCert, StringBuilder strPError)
        {
            try
            {
                X509Certificate2 certificado = new X509Certificate2(rutaCert);
                var rsa = (RSACryptoServiceProvider)certificado.PublicKey.Key;
                var sha = new SHA1Managed();
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(cadena));
                return rsa.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"), firma);
            }
            catch (Exception exc)
            {
                Log.Error(exc.Message);
                return false;
            }
        }


        public string Firmar(ref authTokenType token, string texto, string alias, SignatureModeType tipodefirma)
        {
            LunaXml.xmCryptoService client = new xmCryptoService();
            try
            {
                sign textoafirmar = new sign();
                textoafirmar.AuthToken = token;
                textoafirmar.SignatureModeSpecified = true;
                textoafirmar.DataToSign = Convert.ToBase64String(Encoding.UTF8.GetBytes(texto)); 
                textoafirmar.SignatureMode = tipodefirma;
                textoafirmar.SigningKeyAlias = alias;
                signResponse respuesta = client.sign(textoafirmar);
                token = respuesta.AuthToken;
                if (respuesta.Result.ResultMajor != null && respuesta.Result.ResultMajor == "urn:oasis:names:tc:dss:resultmajor:Success")
                    return respuesta.Signature;
                else
                {
                    Log.Error((respuesta.Result.ResultMessage != null ? respuesta.Result.ResultMessage.Value : ""));
                    return null;
                }
                
            }
            catch (Exception ex)
            {
                Log.Error("Error al intentar firmar: "+ ex);
                return null;
            }
            finally
            {
                    client.Dispose();
            }
        }

        public bool Logout(authTokenType token)
        {
            xmCryptoService client = new xmCryptoService();
            try
            {
                logout log = new logout();
                log.AuthToken = token;
                client.logout(log);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Error al intentar realizar logout: " + ex);
                return false;
            }
            finally
            {
                client.Dispose();
                //if (client.State == CommunicationState.Faulted)
                //    client.Abort();
                //else
                //    client.Close();
            }
        }
 
        public Dictionary<string, string> GetAliasList(ref authTokenType token, string keySpace)
        {
            xmCryptoService client = new xmCryptoService();
            
            try
            {
                Dictionary<string,string> dic = new Dictionary<string, string>();
                string tipo = string.Empty;
                getAliasListResponse respuesta = null;
                getAliasList getAliasList = new getAliasList();

                getAliasList.AuthToken = token;
                getAliasList.KeySpace = keySpace;
               
                respuesta = client.getAliasList(getAliasList);
                token = respuesta.AuthToken;
                foreach (var alias in respuesta.ObjectAlias) {
                    var info = new getObjectInfo { ObjectAlias = alias, AuthToken = respuesta.AuthToken };
                    getObjectInfoResponse resp =  client.getObjectInfo(info);
                    respuesta.AuthToken = resp.AuthToken;
                    if (resp.KeyInfo != null && resp.KeyInfo.ItemsElementName[0] == ItemsChoiceType1.X509Data)
                        tipo = "Certificado";
                    else
                    {
                        tipo = resp.CryptoObject.Policy[1].PolicyValue; 
                        if (tipo == "Private Key")
                            tipo = "Llave Privada";
                        else
                        {
                            tipo = "Llave Pública";
                        }
                    }
                    dic.Add(alias, tipo);
                }

                token = respuesta.AuthToken;
                return dic; 
            }
            catch (Exception ee)
            {
                Log.Error("Error al intentar realizar login: " + ee);
                return null;
            }
        }



        public List<string> GetAliasPublicas(ref authTokenType token, string keySpace)
        {
            xmCryptoService client = new xmCryptoService();
            try
            {
                List<string> dic = new List<string>();
                string tipo = string.Empty;
                getAliasListResponse respuesta = null;
                getAliasList getAliasList = new getAliasList();

                getAliasList.AuthToken = token;
                getAliasList.KeySpace = keySpace;

                respuesta = client.getAliasList(getAliasList);

                foreach (var alias in respuesta.ObjectAlias)
                {
                    var info = new getObjectInfo {ObjectAlias = alias, AuthToken = respuesta.AuthToken};
                    getObjectInfoResponse resp = client.getObjectInfo(info);
                    respuesta.AuthToken = resp.AuthToken;
                    if (resp.CryptoObject.Policy[1].PolicyValue == "Public Key")
                    {
                        dic.Add(alias);
                    }
                }
                token = respuesta.AuthToken;
                return dic;
            }
            catch (Exception ee)
            {
                Log.Error("Error al intentar realizar login: " + ee);
                return null;
            }
        }



        public List<string> GetAliasPrivadas(ref authTokenType token, string keySpace)
        {
            xmCryptoService client = new xmCryptoService();
            try
            {
                List<string> dic = new List<string>();
                string tipo = string.Empty;
                getAliasListResponse respuesta = null;
                getAliasList getAliasList = new getAliasList();

                getAliasList.AuthToken = token;
                getAliasList.KeySpace = keySpace;

                respuesta = client.getAliasList(getAliasList);

                foreach (var alias in respuesta.ObjectAlias)
                {
                    var info = new getObjectInfo { ObjectAlias = alias, AuthToken = respuesta.AuthToken };
                    getObjectInfoResponse resp = client.getObjectInfo(info);
                    respuesta.AuthToken = resp.AuthToken;
                    if (resp.CryptoObject.Policy != null && resp.CryptoObject.Policy[1].PolicyValue == "Private Key")
                    {
                        dic.Add(alias);
                    }
                }
                token = respuesta.AuthToken;
                return dic;
            }
            catch (Exception ee)
            {
                Log.Error("Error al intentar realizar login: " + ee);
                return null;
            }
        }

        public List<string > GetAliasSimetricas(ref authTokenType token, string keySpace)
        {
            xmCryptoService client = new xmCryptoService();
            try
            {
                List<string> dic = new List<string>();
                string tipo = string.Empty;
                getAliasListResponse respuesta = null;
                getAliasList getAliasList = new getAliasList();

                getAliasList.AuthToken = token;
                getAliasList.KeySpace = keySpace;

                respuesta = client.getAliasList(getAliasList);

                foreach (var alias in respuesta.ObjectAlias)
                {
                    var info = new getObjectInfo { ObjectAlias = alias, AuthToken = respuesta.AuthToken };
                    getObjectInfoResponse resp = client.getObjectInfo(info);
                    respuesta.AuthToken = resp.AuthToken;
                   if(resp.CryptoObject.Policy!= null && resp.CryptoObject.Policy[1].PolicyValue == "Secret Key")
                   {
                       dic.Add(alias);
                   }
                }
                token = respuesta.AuthToken;
                return dic;
            }
            catch (Exception ee)
            {
                Log.Error("Error al intentar realizar login: " + ee);
                return null;
            }
        }

         
        public bool GenerarPardeLlaves(ref authTokenType token, int tamaño, string alias, KeyAlgorithmType algoritmo)
        {
            xmCryptoService client = new xmCryptoService();
            try
            {  
                generateKeyPair keyPair = new generateKeyPair();
                generateKeyPairResponse res = new generateKeyPairResponse();
                keyPair.KeyAlgorithm = KeyAlgorithmType.RSA;
                keyPair.KeySize = tamaño.ToString();
                keyPair.AuthToken = token;
                keyPair.KeyAlias = alias;
                res = client.generateKeyPair(keyPair);
                token = res.AuthToken;
                if (res.Result.ResultMajor != null && res.Result.ResultMajor == "urn:oasis:names:tc:dss:resultmajor:Success")
                    return true;
                else
                {
                    Log.Error((res.Result.ResultMessage != null ? res.Result.ResultMessage.Value : ""));
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error al intentar generar par de llaves: " + ex);
                return false;
            }
        }

        public Dictionary<string , string> SubirLlavePrivada(authTokenType token, string archivoLlave, string pass, string alias)
        {
            byte[] bytes = File.ReadAllBytes(archivoLlave);
            RSAParameters param = OpensslKey.DecodeEncryptedPrivateKeyInfoParams(bytes, pass);
            string soap = ArmarSoapPrivada(alias, Convert.ToBase64String(param.Modulus), Convert.ToBase64String(param.Exponent), token.SessionStateToken);
            return ExtraerTokendeRespuesta(Ejecutar(soap));
        }

        public Dictionary<string ,string> ExtraerTokendeRespuesta(string soap)
        {
            bool success;
            string mensaje, autorizacion;
            XmlDocument doc = new XmlDocument();
            authTokenType tok =  new authTokenType();
            Dictionary<string, string> dic = new Dictionary<string, string>();

            doc.LoadXml(soap);
            
            foreach (XmlNode node in doc.ChildNodes[1].ChildNodes[1].ChildNodes[0]){
                if (node.Name == "dss:Result") {
                    success = node.FirstChild.InnerText.IndexOf("Success") >= 0;
                    if (!success) {
                        mensaje = node.ChildNodes[1].InnerText;
                        dic.Add("Estado", "False");
                        dic.Add("Mensaje", mensaje);
                    }
                    else 
                        dic.Add("Estado", "True");
                }
                if (node.Name == "xmc:AuthToken")
                {
                    autorizacion = node.FirstChild.InnerText;
                    dic.Add("Autorizacion", autorizacion);
                }
            }

            return dic;
        }

        public  authTokenType Login(string usuario, string contraseña)
        {
            xmCryptoService client = new xmCryptoService();
            try
            {
                var log = new login { UserID = usuario, password = contraseña, authModel = authModelType.PROP };
                loginResponse response = client.login(log);
                return response.AuthToken;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public bool GenerarLLavesSoftware(string subject, string challenge,string fileName)
        {
            try
            {
                RsaKeyPairGenerator r = new RsaKeyPairGenerator();
            var param = new RsaKeyGenerationParameters(new BigInteger ("10001",16), new SecureRandom(), 1024,80);
            r.Init(param);
            AsymmetricCipherKeyPair  k = r.GenerateKeyPair();
            var privada =  PrivateKeyInfoFactory.CreatePrivateKeyInfo(k.Private);
            SubjectPublicKeyInfo pubInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(k.Public);
            string priv = Convert.ToBase64String(privada.GetDerEncoded());
            string pub = Convert.ToBase64String(pubInfo.GetDerEncoded());
            File.WriteAllText("Privada.pem", priv);
            File.WriteAllText("Publica.pem", pub);
            RsaPrivateCrtKeyParameters privateKey = (RsaPrivateCrtKeyParameters) PrivateKeyFactory.CreateKey(Convert.FromBase64String(priv));
            RsaKeyParameters publicKey = (RsaKeyParameters) PublicKeyFactory.CreateKey(Convert.FromBase64String(pub));

            DerSet derset = null;
                if (challenge != null)
                {
                    ChallengePassword chpass = new ChallengePassword(challenge);
                    derset= new DerSet(chpass);
                  
                }
                else
                {
                    derset = new DerSet();
                }
              
                X509Name sub = new X509Name(subject , new ConverterSidetec());
                Pkcs10CertificationRequest ds = new Pkcs10CertificationRequest("SHA1WITHRSA", sub, publicKey, derset,privateKey);
                
                File.WriteAllBytes(fileName, ds.GetDerEncoded());
                return true;
            }
            catch (Exception ee)
            {
                
                Log.Error(ee.Message);
                return false;
            }
            

           
        }


        public  string ArmarSoapPublica(string keyAlias, string certbase64, string sessionToken)
        {
            string header =
                "<?xml version='1.0' encoding='UTF-8'?><soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\">";
            string body = "<soapenv:Body><ns5:inject xmlns:ns5=\"http://xmCrypto.safenet-inc.com/xsd\">";
            string injectedKeyAlias = "<ns5:InjectedKeyAlias>" + keyAlias + "</ns5:InjectedKeyAlias>";
            string keytoInject = "<KeyToInject xmlns=\"http://www.w3.org/2000/09/xmldsig#\"><X509Data><X509Certificate>" +
                                 certbase64 + "</X509Certificate></X509Data></KeyToInject>";
            string publicobject = "<ns5:PublicObject>false</ns5:PublicObject>";
            string politica = "<ns5:Policy><ns5:PolicyName>SetKeySpace</ns5:PolicyName><ns5:PolicyValue>soapAdmin</ns5:PolicyValue></ns5:Policy>";
            string tokenauth = "<ns5:AuthToken><ns5:SessionStateToken>" + sessionToken +
                               "</ns5:SessionStateToken></ns5:AuthToken></ns5:inject></soapenv:Body></soapenv:Envelope>";
            return header + body + injectedKeyAlias + keytoInject + publicobject + politica + tokenauth;
        }

        public  string ArmarSoapPrivada(string keyAlias, string modulo, string exponente, string sessionToken)
        {
            string header =
                "<?xml version='1.0' encoding='utf-8'?><soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\">";
            string body = "<soapenv:Body><ns5:inject xmlns:ns5=\"http://xmCrypto.safenet-inc.com/xsd\">";
            string injectedKeyAlias = "<ns5:InjectedKeyAlias>" + keyAlias + "</ns5:InjectedKeyAlias>";
            string keytoInject = "<KeyToInject xmlns=\"http://www.w3.org/2000/09/xmldsig#\"><KeyValue><RSAKeyValue>" +
                                 "<Modulus>" + modulo + "</Modulus><Exponent>" + exponente + "</Exponent></RSAKeyValue></KeyValue></KeyToInject>";
            string publicobject = "<ns5:PublicObject>false</ns5:PublicObject>";
            string politica = "<ns5:Policy><ns5:PolicyName>SetKeySpace</ns5:PolicyName><ns5:PolicyValue>soapAdmin</ns5:PolicyValue></ns5:Policy>";
            string tokenauth = "<ns5:AuthToken><ns5:SessionStateToken>" + sessionToken +
                               "</ns5:SessionStateToken></ns5:AuthToken></ns5:inject></soapenv:Body></soapenv:Envelope>";
            return header + body + injectedKeyAlias + keytoInject + publicobject + politica + tokenauth;
            #region Con objetos
            //inject inject1 = new inject();
            //inject1.AuthToken = token;
            //KeyInfoType kit = new KeyInfoType();
            //inject1.InjectedKeyAlias = alias;
            //inject1.PublicObject = true;
            //string keyName = alias;
            //xmCryptoService client = new xmCryptoService();
            //byte[] bytes = File.ReadAllBytes(archivoLlave);

            //RSAParameters param = OpensslKey.DecodeEncryptedPrivateKeyInfoParams(bytes, pass);
            //RSAKeyValueType key = new RSAKeyValueType();
            //key.Modulus = param.Modulus;
            //key.Exponent = param.Exponent; 
            //KeyValueType key2 = new KeyValueType();
            //key2.Item = key;
            //kit.Items = new object[] { key2 };
            //kit.ItemsElementName = new[] { ItemsChoiceType1.KeyValue };
            //inject1.KeyToInject = kit;
            //client.inject(inject1);
            //return true;
            #endregion
        }

        public  Dictionary<string, string> SubirLlavePublica(authTokenType token, string archivoLlave, string pass, string alias)
        {
                X509Certificate2 x509Certificate2 = new X509Certificate2(archivoLlave);
                string soap = ArmarSoapPublica(alias, Convert.ToBase64String(x509Certificate2.RawData), token.SessionStateToken);
                return ExtraerTokendeRespuesta(Ejecutar(soap)); 
        }

        public  HttpWebRequest CreateWebRequest()
        {
            Configuration appConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var serviceModelSection = System.ServiceModel.Configuration.ServiceModelSectionGroup.GetSectionGroup(appConfig);
            var addr = ConfigurationManager.AppSettings["direccionwebservice"];
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(addr);//@"https://192.168.4.26:8443/xmc/services/xmCryptoService");
            webRequest.Headers.Add(@"SOAPAction", "\"urn:inject\"");
            webRequest.ContentType = "text/xml; charset=utf-8";
            webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; MS Web Services Client Protocol 2.0.50727.5456)";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        public  string Ejecutar(string soap)
        {
            HttpWebRequest request = CreateWebRequest();
            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(soap);

            using (Stream stream = request.GetRequestStream())
            {
                soapEnvelopeXml.PreserveWhitespace = true;
                soapEnvelopeXml.Save(stream);
            }

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    string soapResult = rd.ReadToEnd();
                    return soapResult;
                }
            } 
        }
        
    }
}
