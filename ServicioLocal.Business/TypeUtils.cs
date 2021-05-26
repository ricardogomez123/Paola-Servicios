using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ServicioLocal.Business
{
    public class TypeUtils
    {
        public static object ChangeType(object value, Type conversionType)
        {
            if (conversionType == null)
            {
                throw new ArgumentNullException("conversionType");
            }
            if (value == null)
            {
                return null;
            }
            if (conversionType.IsGenericType &&
              conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {

                NullableConverter nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }
            //if (conversionType.Equals(typeof(string)))
            //{
            //    if (value == null)
            //        return null;
            //    return string.IsNullOrEmpty(((string)value).Trim()) ? null : ((string) value).Trim();
            //}
            if (conversionType.Equals(typeof(bool)) && value == null)
            {
                return false;
            }
            if (conversionType.Equals(typeof(decimal)))
            {
                if (value == null) return null;
                if (string.IsNullOrEmpty((string)value)) return null;
                if (((string)value).Contains("$"))
                {
                    return decimal.Parse((string)value, NumberStyles.Currency);
                }
            }
            //if (conversionType.Equals(typeof(DateTime)) && value!= null)
            //{
            //    if ( string.IsNullOrEmpty(value.ToString()))
            //    {
            //        return null;    
            //    }
            //    else
            //    {
            //        DateTime result;
            //        if (!DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            //            result = DateTime.ParseExact((string) value, "d/MM/yyyy ", CultureInfo.InvariantCulture);
            //        return result;
            //    }

            //}
            if (conversionType == typeof(int))
            {
                if (string.IsNullOrEmpty((string)value)) return null;
            }

            return Convert.ChangeType(value, conversionType);
        }
    }
}
