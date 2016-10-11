using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifc4
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;
    using System.Diagnostics;

    //http://social.msdn.microsoft.com/Forums/en-US/xmlandnetfx/thread/1ea6126f-e2d3-430f-aedb-56d228fcb38d

    public class Validation
    {
        public delegate void InfoEventHandler(object sender, Ifc4.EventArgs.MessageLoggedEventArgs e);
        public event InfoEventHandler Info;

        private int _ErrorCounter = 0;
        private int _WarningCounter = 0;

        private int _NumberOfSeparator = 60;

        public Validation()
        {
            WriteLog(String.Empty.PadRight(_NumberOfSeparator, '*'));

            List<System.Reflection.AssemblyName> assemblyNames = new List<System.Reflection.AssemblyName>();

            System.Reflection.Assembly assembly = null;
            assembly = System.Reflection.Assembly.GetEntryAssembly();
            if (assembly != null)
                assemblyNames.Add(assembly.GetName());

            assembly = System.Reflection.Assembly.GetExecutingAssembly();
            if (assembly != null)
                assemblyNames.Add(assembly.GetName());

            foreach (var assemblyName in assemblyNames.OrderBy(item => item.Name))
            {
                WriteLog(String.Format("{0}, Version:{1}", assemblyName.Name, assemblyName.Version));
            }
            WriteLog(String.Empty.PadRight(_NumberOfSeparator, '*'));
        }

        private string GetSchemaXsdFullName(string fileName)
        {
            // sollte hier sein
            string fullName = Path.Combine(AssemblyDirectoryFullName, fileName);
            if (File.Exists(fullName))
                return fullName;

            // Datei kann nicht gefunden werden oder fehlt
            // versuche die Datei aus den Resourcen zu erstellen
            fullName = GetIfcSchemaFromResource(String.Concat("Ifc4.Schema.", fileName), fileName);
            return fullName;
        }

        private string AssemblyDirectoryFullName
        {
            get
            {
                FileInfo fileInfo = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
                return fileInfo.DirectoryName;
            }
        }

        private string GetIfcSchemaFromResource(string resourceName, string schemaFileName)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            string[] schemaFullNames = new string[]
            {
                System.IO.Path.Combine(AssemblyDirectoryFullName, schemaFileName), // // erster Versuch parallel zum Dll
                System.IO.Path.Combine(Path.GetTempPath(), schemaFileName) // zweiter Versuch im Temp Folder
            };

            for (int i = 0; i < schemaFullNames.Length; i++)
            {
                if (System.IO.File.Exists(schemaFullNames[i]))
                    return schemaFullNames[i];
            }

            var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                for (int i = 0; i < schemaFullNames.Length; i++)
                {
                    try
                    {
                        using (FileStream fileStream = new FileStream(schemaFullNames[i], FileMode.Create))
                        {
                            CopyStream(stream, fileStream);
                        }
                        return schemaFullNames[i];
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            return String.Empty;
        }

        private static void CopyStream(Stream source, Stream target)
        {
            const int bufferSize = 0x1000; //4096
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;
            while ((bytesRead = source.Read(buffer, 0, bufferSize)) > 0)
                target.Write(buffer, 0, bytesRead);
        }

        private Ifc4.EventArgs.MessageLoggedEventArgs m_MessageLoggedEventArgs;
        private void WriteLog(string s)
        {
            if (m_MessageLoggedEventArgs == null)
                m_MessageLoggedEventArgs = new Ifc4.EventArgs.MessageLoggedEventArgs();

            m_MessageLoggedEventArgs.AddMessage(s);
        }

        //private void ABC()
        //{
        //    List<string> l = new List<string>();
        //    foreach (var type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes().OrderBy(item => item.Name))
        //    {

        //        List<string> l1 = new List<string>();

        //        Type tmp = type;
        //        while (tmp != null)
        //        {
        //            if (tmp == typeof(Ifc4.IfcProduct))
        //            {
        //                if (!type.IsAbstract)
        //                    l.Add(type.FullName);
        //            }
        //            tmp = tmp.BaseType;
        //        }

        //    }

        //    l.Sort();
        //    foreach (var ll in l)
        //    {
        //        System.Diagnostics.Debug.WriteLine(ll);
        //    }
        //}

        public bool Validate(string xmlFullName, bool useLocalSchemaFiles)
        {
            try
            {
                _ErrorCounter = 0; // reset counter
                _WarningCounter = 0; // reset counter

                //WriteLog(String.Empty.PadRight(_NumberOfSeparator, '*'));
                WriteLog("Start schema validation...");

                if (String.IsNullOrEmpty(xmlFullName))
                {
                    WriteLog("No open file found!");
                    return false;
                }

                WriteLog(String.Format("IfcXml File: '{0}'", xmlFullName));

                string schemaXsdFullName = GetSchemaXsdFullName("ifcXML4.xsd");
                WriteLog(String.Format("Xsd File: {0}", schemaXsdFullName));

                XmlReaderSettings settings = new XmlReaderSettings();

                if (useLocalSchemaFiles)
                {
                    // externe XML-Ressourcen aufösen
                    Uri uri = new Uri(AssemblyDirectoryFullName + "\\");
                    CustomXmlResolver resolver = new CustomXmlResolver(uri);
                    resolver.Credentials = System.Net.CredentialCache.DefaultCredentials;
                    resolver.UriMap.Add(@"http://www.buildingsmart-tech.org/ifcXML/IFC4/final/ifcXML4.xsd", "ifcXML4.xsd");

                    WriteLog(String.Format("Mapping path: '{0}'", uri.AbsolutePath));
                    foreach (var c in resolver.UriMap)
                    {
                        WriteLog(String.Format("Map '{0}' to local file '{1}'", c.Key, c.Value));
                    }

                    settings.Schemas.XmlResolver = resolver;
                }




                //string targetNamespace = "http://www.buildingsmart-tech.org/ifcXML/IFC4/final";
                //Uri schemaUri = new Uri("http://www.buildingsmart-tech.org/ifcXML/IFC4/final/ifcXML4.xsd");
                //string schemaFullName = schemaUri.AbsoluteUri;
                //namespaces.Add("ifc", "http://www.buildingsmart-tech.org/ifcXML/IFC4/final");
                //XmlSchemaSet schemaSet = new XmlSchemaSet();
                //schemaSet.Add(targetNamespace, "ifcXML4.xsd");
                //foreach (XmlSchema schema in schemaSet.Schemas("http://www.buildingsmart-tech.org/ifcXML/IFC4/final"))
                //{
                //    schema.Write(Console.Out);
                //}

                //settings.Schemas.Add(null, "ifcXML4.xsd");
                settings.Schemas.Add(null, schemaXsdFullName);
                
                //settings.Schemas.Add(targetNamespace, schemaFullName);

                settings.ValidationType = ValidationType.Schema;
                //settings.DtdProcessing= DtdProcessing.Ignore
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
                //settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessIdentityConstraints;

                settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

                string source = System.IO.File.ReadAllText(xmlFullName);

                using (MemoryStream stream = new MemoryStream())
                {
                    using(StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(source);
                        writer.Flush();
                        stream.Position = 0;

                        using (XmlReader reader = XmlReader.Create(stream, settings))
                        {
                            while (reader.Read())
                            {
                            }
                        }

                    }
                }
                return (_ErrorCounter == 0);
            }
            catch (Exception exc)
            {
                WriteLog("");
                WriteLog("Fatal error: Cannot validate file.");
                WriteLog(exc.Message);
                WriteLog("");
                return false;
            }
            finally
            {
                WriteLog(String.Format("{0} Error(s); {1} Warnings(s)", _ErrorCounter, _WarningCounter));
                WriteLog("Validation finished.");
                WriteLog(String.Empty.PadRight(_NumberOfSeparator, '*'));

                if (Info != null)
                {
                    Ifc4.EventArgs.MessageLogType result;
                    if (_ErrorCounter > 0)
                    {
                        result = Ifc4.EventArgs.MessageLogType.Error;
                    }
                    else
                    {
                        result = Ifc4.EventArgs.MessageLogType.Valid;
                        if (_WarningCounter > 0)
                            result |= Ifc4.EventArgs.MessageLogType.Warning;
                    }

                    m_MessageLoggedEventArgs.Type = result;
                    Info(this, m_MessageLoggedEventArgs);
                }
            }
        }

        private void ValidationCallBack(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                _WarningCounter++;
                WriteLog(String.Format("\tWarning: Matching schema not found.  No validation occurred."));
            }
            else
            {
                _ErrorCounter++;
                WriteLog(String.Format("\tSeverity:{0}", e.Severity));
            }
            WriteLog(String.Format("\tMessage  :{0}", e.Message));
        }

        public class CustomXmlResolver : XmlUrlResolver
        {
            public Uri CacheUri { get; set; }
            public Dictionary<string, string> UriMap = new Dictionary<string, string>();

            public CustomXmlResolver(Uri cacheUri)
                : base()
            {
                CacheUri = cacheUri;
            }

            public override Uri ResolveUri(Uri baseUri, string relativeUri)
            {
                if (UriMap.ContainsKey(relativeUri))
                    return new Uri(CacheUri, UriMap[relativeUri]);

                return base.ResolveUri(baseUri, relativeUri);
            }
        }



    }

    public enum ValidationEnumType
    {
        NotDefined,
        IsNotNull,
        IsNotNullOrNotEmpty,
        GreaterThan,
        GreaterThanOrEqual
    }

    public class ValidationProperty
    {
        public ValidationProperty(string propertyName, Type propertyType, ValidationEnumType validationType, object compareValue)
        {
            PropertyName = propertyName;
            PropertyType = propertyType;
            ValidationType = validationType;
            CompareValue = compareValue;
        }

        public string PropertyName { get; private set; }

        public Type PropertyType { get; private set; }

        public ValidationEnumType ValidationType { get; private set; }

        public object CompareValue { get; private set; }
    }


}
