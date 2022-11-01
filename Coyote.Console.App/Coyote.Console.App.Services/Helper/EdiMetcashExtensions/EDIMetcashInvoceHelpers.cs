using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Coyote.Console.App.Services.Helper.EdiMetcashExtensions
{
    public static class EDIMetcashInvoceHelpers 
    {
        /// <param name="path">provide file path</param>
        internal static InvoiceBatch ParseXMLToInvoiceModel(string path)
        {
            if (File.Exists(path))
            {

                var xml = File.ReadAllText(path);
                return GenrateInvoiceText(xml);
            }
            else
            {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                throw new FileNotFoundException("XML File not found!", path);
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            }
        }
        internal static InvoiceBatch GenrateInvoiceText(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XMLExtensions.RemoveNamespace(doc, "ns0");

            XmlSerializer serializer = new XmlSerializer(typeof(InvoiceBatch));

            using (StringReader reader = new StringReader(doc.InnerXml))
            {
#pragma warning disable CA5369 // Use XmlReader For Deserialize
                return (InvoiceBatch)serializer.Deserialize(reader);
#pragma warning restore CA5369 // Use XmlReader For Deserialize
            }

        }
    }
    internal static class XMLExtensions
    {
        public static void RemoveNamespace(this XmlDocument document, string @namespace) =>
            document.InnerXml = Regex.Replace(
                document.InnerXml,
                $@"((?<=\</|\<){@namespace}:|xmlns:{@namespace}=""[^""]+"")",
                "");
    }
}
