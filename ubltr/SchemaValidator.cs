using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Diagnostics;

namespace ubltr
{
    public static class SchemaValidator
    {
        public static string SchemaValidatorErrors = "";

        public static bool ValidateEnvolope(string schemaFile, string xmlFile)
        {
            bool res = false;
            try
            {
                SchemaValidatorErrors = "";
                XmlSchemaSet schemaSet = new XmlSchemaSet();
                schemaSet.Add("http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader", schemaFile);

                XmlReaderSettings settings = new XmlReaderSettings();
                foreach (XmlSchema schema in schemaSet.Schemas())
                {
                    settings.Schemas.Add(schema);
                }
                //settings.Schemas.Add(compiledSchema);
                settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
                settings.ValidationType = ValidationType.Schema;

                // Create the XmlReader object.
                XmlReader reader = XmlReader.Create(xmlFile, settings);

                // Parse the file. 
                while (reader.Read()) ;
                res = true;
            }
            catch (Exception e)
            {
                res = false;
                Debug.WriteLine("Error While Validating against Schema " + e.Message.ToString());
            }
            return res;
        }

        public static bool Validate(string schemaFile,string xmlFile)
        {
            bool res = false;
            try
            {
                SchemaValidatorErrors = "";
                XmlSchemaSet schemaSet = new XmlSchemaSet();
                schemaSet.Add("urn:oasis:names:specification:ubl:schema:xsd:Invoice-2", schemaFile);
                /*schemaSet.Add("urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2", schemaFile);
                schemaSet.Add("urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2", schemaFile);
                schemaSet.Add("urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2", schemaFile);
                schemaSet.Add("urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2", schemaFile);
                schemaSet.Add("urn:un:unece:uncefact:documentation:2", schemaFile);
                schemaSet.Add("urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2", schemaFile);
                schemaSet.Add("urn:oasis:names:specification:ubl:schema:xsd:Invoice-2", schemaFile);*/

                //XmlSchema compiledSchema = null;


                XmlReaderSettings settings = new XmlReaderSettings();
                foreach (XmlSchema schema in schemaSet.Schemas())
                {
                    settings.Schemas.Add(schema);
                }
                //settings.Schemas.Add(compiledSchema);
                settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
                settings.ValidationType = ValidationType.Schema;

                // Create the XmlReader object.
                XmlReader reader = XmlReader.Create(xmlFile, settings);

                // Parse the file. 
                while (reader.Read()) ;
                res = true;
            }
            catch (Exception e)
            {
                res = false;
                Debug.WriteLine("Error While Validating against Schema " + e.Message.ToString());
            }
            return res;
        }

        // Display any warnings or errors.
        private static void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
                SchemaValidatorErrors += "\tWarning: Matching schema not found.  No validation occurred." + args.Message;
            else
                SchemaValidatorErrors += "\tValidation error: " + args.Message;

        }
    }
}
