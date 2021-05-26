using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Security.Cryptography;

namespace ClienteCertificador
{

    public class RandomPassword
    {
        // Define default min and max password lengths.
        private const int DefaultMinPasswordLength = 8;
        private const int DefaultMaxPasswordLength = 12;

        // Define supported password characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private const string PasswordCharsLcase = "abcdefgijkmnopqrstwxyz";
        private const string PasswordCharsUcase = "ABCDEFGHJKLMNPQRSTWXYZ";
        private const string PasswordCharsNumeric = "23456789";

        /// <summary>
        /// Generates a random password.
        /// </summary>
        /// <returns>
        /// Randomly generated password.
        /// </returns>
        /// <remarks>
        /// The length of the generated password will be determined at
        /// random. It will be no shorter than the minimum default and
        /// no longer than maximum default.
        /// </remarks>
        public static string Generate()
        {
            return Generate(DefaultMinPasswordLength, DefaultMaxPasswordLength);
        }

        /// <summary>
        /// Generates a random password of the exact length.
        /// </summary>
        /// <param name="length">
        /// Exact password length.
        /// </param>
        /// <returns>
        /// Randomly generated password.
        /// </returns>
        public static string Generate(int length)
        {
            return Generate(length, length);
        }

        /// <summary>
        /// Generates a random password.
        /// </summary>
        /// <param name="minLength">
        /// Minimum password length.
        /// </param>
        /// <param name="maxLength">
        /// Maximum password length.
        /// </param>
        /// <returns>
        /// Randomly generated password.
        /// </returns>
        /// <remarks>
        /// The length of the generated password will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        public static string Generate(int minLength, int maxLength)
        {
            // Make sure that input parameters are valid.
            if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                return null;

            // Create a local array containing supported password characters
            // grouped by types. You can remove character groups from this
            // array, but doing so will weaken the password strength.
            var charGroups = new[]
                                 {
                                     PasswordCharsLcase.ToCharArray(), PasswordCharsUcase.ToCharArray(),
                                     PasswordCharsNumeric.ToCharArray()
                                 };

            // Use this array to track the number of unused characters in each
            // character group.
            var charsLeftInGroup = new int[charGroups.Length];

            // Initially, all characters in each group are not used.
            for (int i = 0; i < charsLeftInGroup.Length; i++)
                charsLeftInGroup[i] = charGroups[i].Length;

            // Use this array to track (iterate through) unused character groups.
            var leftGroupsOrder = new int[charGroups.Length];

            // Initially, all character groups are not used.
            for (int i = 0; i < leftGroupsOrder.Length; i++)
                leftGroupsOrder[i] = i;

            // Because we cannot use the default randomizer, which is based on the
            // current time (it will produce the same "random" number within a
            // second), we will use a random number generator to seed the
            // randomizer.

            // Use a 4-byte array to fill it with random bytes and convert it then
            // to an integer value.
            var randomBytes = new byte[4];

            // Generate 4 random bytes.
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            // Convert 4 bytes into a 32-bit integer value.
            int seed = (randomBytes[0] & 0x7f) << 24 | randomBytes[1] << 16 | randomBytes[2] << 8 | randomBytes[3];

            // Now, this is real randomization.
            var random = new Random(seed);

            // Allocate appropriate memory for the password.
            char[] password = minLength < maxLength
                                  ? new char[random.Next(minLength, maxLength + 1)]
                                  : new char[minLength];

            // Index of the last non-processed group.
            int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            // Generate password characters one at a time.
            for (int i = 0; i < password.Length; i++)
            {
                // If only one character group remained unprocessed, process it;
                // otherwise, pick a random character group from the unprocessed
                // group list. To allow a special character to appear in the
                // first position, increment the second parameter of the Next
                // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                int nextLeftGroupsOrderIdx;
                if (lastLeftGroupsOrderIdx == 0)
                    nextLeftGroupsOrderIdx = 0;
                else
                    nextLeftGroupsOrderIdx = random.Next(0,
                                                         lastLeftGroupsOrderIdx);

                // Get the actual index of the character group, from which we will
                // pick the next character.
                int nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                // Get the index of the last unprocessed characters in this group.
                int lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                // If only one unprocessed character is left, pick it; otherwise,
                // get a random character from the unused character list.
                int nextCharIdx = lastCharIdx == 0 ? 0 : random.Next(0, lastCharIdx + 1);

                // Add this character to the password.
                password[i] = charGroups[nextGroupIdx][nextCharIdx];

                // If we processed the last character in this group, start over.
                if (lastCharIdx == 0)
                    charsLeftInGroup[nextGroupIdx] =
                        charGroups[nextGroupIdx].Length;
                // There are more unprocessed characters left.
                else
                {
                    // Swap processed character with the last unprocessed character
                    // so that we don't pick it until we process all characters in
                    // this group.
                    if (lastCharIdx != nextCharIdx)
                    {
                        char temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] =
                            charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }
                    // Decrement the number of unprocessed characters in
                    // this group.
                    charsLeftInGroup[nextGroupIdx]--;
                }

                // If we processed the last group, start all over.
                if (lastLeftGroupsOrderIdx == 0)
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                // There are more unprocessed groups left.
                else
                {
                    // Swap processed group with the last unprocessed group
                    // so that we don't pick it until we process all groups.
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] =
                            leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }
                    // Decrement the number of unprocessed groups.
                    lastLeftGroupsOrderIdx--;
                }
            }
            return new string(password);
        }
    }


    public class Constantes
    {





        public static IDictionary<int, string> ErroresValidacion
        {
            get
            {
                var result = new Dictionary<int, string>
                                 {
                                     {301, "XML mal formado"},
                                     {302, "Sello mal formado o inválido"},
                                     {303, "Sello no corresponde a emisor o caduco"},
                                     {304, "Certificado revocado o caduco"},
                                     {305, "La fecha de emisión no esta dentro de la vigencia del CSD del Emisor"},
                                     {306, "EL certificado no es de tipo CSD"},
                                     {307, "El CFDI contiene un timbre previo"},
                                     {308, "Certificado no expedido por el SAT"},
                                     {401, "Fecha y hora de generación fuera de rango"},
                                     {402, "RFC del emisor no se encuentra en el régimen de contribuyentes"},
                                     {403, "La fecha de emisión no es posterior al 01 de enero 2011"},
                                     {
                                         102,
                                         "El Certificado registrado en el XML no coincide con el registrado en la Base de datos"
                                         },
                                     {101, "El Emisor no esta registrado en la base de datos"},
                                     {501, "Autenticación no valida"},
                                     {502, "Comprobante no encontrado"},
                                     {503, "Los metadatos recibidos no son validos"},
                                     {504, "La estructura del comprobante recibido no es válida"},
                                     {505, "Los metadatos proporcionados no corresponden al comprobante"},
                                     {601, "La expresión impresa proporcionada no es válida"},
                                     {602, "Comprobante no encontrado"},
                                     {500, "Usuario inválido"},
                                     {103, "Error al conectar al Web Service de envio SAT"},
                                     {398, "El Emisor no tiene obligaciones"},
                                     {104, "Error al conectar Web Service de cancelacion del SAT"},
                                     {399, "El CSD del emisor es invalido"},
                                     {202, "UUID Previamente cancelado"},
                                     {105, "El CFDi no puede ser cancelado debido a su estatus"},
                                     {203, "UUID no corresponde a emisor"},
                                     {204, "UUID no aplicable para cancelacion"},
                                     {205, "UUID no existe"},
                                     {201, "UUID Cancelado exitosamente"},
                                     {666, "Error General en el sistema, verificar el Log de la aplicación"}
                                 };

                return result;
            }
        }






        public static XNamespace CFDVersionNamespace
        {
            get { return XNamespace.Get("http://www.sat.gob.mx/cfd/3"); }
        }

        public static XNamespace CFDTimbreFiscalVersionNamespace
        {
            get { return XNamespace.Get("http://www.sat.gob.mx/TimbreFiscalDigital"); }
        }

        public static IList<string> XsdsToLoad
        {
            get
            {
                var result = new List<string>
                                 {
                                     "cfdv32.xsd",
                                     "detallista.xsd",
                                     "Divisas.xsd",
                                     "donat11.xsd",
                                     "ecb.xsd",
                                     "ecc.xsd",
                                     "iedu.xsd",
                                     "implocal.xsd",
                                     "leyendasFisc.xsd",
                                     "pfic.xsd",
                                     "psgecfd.xsd",
                                     "terceros11.xsd",
                                     "TimbreFiscalDigital.xsd",
                                     "TuristaPasajeroExtranjero.xsd",
                                     "TuristaPasajeroExtranjero.xsd",
                                     "ventavehiculos.xsd"
                                 };

                return result;
            }
        }
    }
}
