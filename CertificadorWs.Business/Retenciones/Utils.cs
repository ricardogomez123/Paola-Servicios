using System;
using System.Linq;
using System.Text;

namespace CertificadorWs.Business.Retenciones
{
    public class Utils
    {

        public static DateTime GetExpiryTime(string token)
        {
            
                var swt = token.Substring("wrap_access_token=\"".Length, token.Length - ("wrap_access_token=\"".Length + 1));
                var tokenValue = Uri.UnescapeDataString(swt);
                var properties = (from prop in tokenValue.Split('&')
                                  let pair = prop.Split(new[] { '=' }, 2)
                                  select new { Name = pair[0], Value = pair[1] })
                                 .ToDictionary(p => p.Name, p => p.Value);

                var expiresOnUnixTicks = int.Parse(properties["ExpiresOn"]);
                var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);

                var fecha = epochStart.AddSeconds(expiresOnUnixTicks);
                return TimeZone.CurrentTimeZone.ToLocalTime(fecha);
           
        }

        public static DateTime? GetExpiryTime2(string token)
        {
            try
            {
                var swt = token.Substring("wrap_access_token=\"".Length, token.Length - ("wrap_access_token=\"".Length + 1));
                var tokenValue = Uri.UnescapeDataString(swt);
                var properties = (from prop in tokenValue.Split('&')
                                  let pair = prop.Split(new[] { '=' }, 2)
                                  select new { Name = pair[0], Value = pair[1] })
                                 .ToDictionary(p => p.Name, p => p.Value);

                var expiresOnUnixTicks = int.Parse(properties["ExpiresOn"]);
                var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);

                var fecha = epochStart.AddSeconds(expiresOnUnixTicks);
                return TimeZone.CurrentTimeZone.ToLocalTime(fecha);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string RasurarNocertificado(string serie)
        {
            var result = new StringBuilder();
            if (serie.Length > 20)
            {
                for (int i = 1; i < serie.Length; i++)
                {
                    if (i % 2 != 0)
                        result.Append(serie[i]);
                }
                return result.ToString();
            }
            return serie;
        }

    }
}
