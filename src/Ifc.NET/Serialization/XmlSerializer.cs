using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;




namespace Ifc4.Serialization
{

    // ifcXML4.xsd
    // anpassen

    //<xs:attribute name="itemType">
    //    <xs:simpleType>
    //        <!-- <xs:list itemType="xs:QName"/> -->
    //        <xs:list itemType="xs:string"/>
    //    </xs:simpleType>
    //</xs:attribute>


    // http://msdn.microsoft.com/query/dev11.query?appId=Dev11IDEF1&l=DE-DE&k=k(System.Xml.Serialization.XmlSerializer);k(TargetFrameworkMoniker-.NETFramework,Version%3Dv4.5);k(DevLang-csharp)&rd=true

    // TOODJV

    //WinDiff benhutzen

    // suche auskommentierte Attribute
    //[System.Xml.Serialization.XmlTextAttribute()]

    //Defining Default Values with the ShouldSerialize and Reset Methods
    //http://msdn.microsoft.com/en-us/library/53b8022e.aspx

    // suche
    //public bool ShouldSerializeDimensions()
    //{
    //    return dimensionsField != null;
    //}


    //http://msdn.microsoft.com/de-de/library/system.xml.serialization.xmltextattribute%28v=vs.110%29.aspx
    //In manchen Fällen generiert XML Schema Definition-Tool (Xsd.exe) das XmlTextAttribute
    //beim Erstellen von Klassen aus einer XML-Schemadefinitionsdatei (XSD-Datei).
    //Dies erfolgt, wenn das Schema einen complexType mit gemischtem Inhalt enthält.
    //In diesem Fall enthält die entsprechende Klasse einen Member, der ein Zeichenfolgenarray zurückgibt,
    //auf das XmlTextAttribute angewendet wird. Wenn das Xml Schema Definition-Tool
    //z.B. das folgende Schema verarbeitet: 


//<IfcRelAggregates GlobalId="2YBqaV_8L15eWJ9DA1sGmT">
//<RelatingObject xsi:nil="true" ref="i1895" xsi:type="IfcProject"/>
//<RelatedObjects>
//<IfcBuilding xsi:nil="true" ref="i1928"/>
//</RelatedObjects>
//</IfcRelAggregates


    // Problem:

    // -----------------------------------------------------------------------------------  

    //<IfcRelAssociatesClassification id="i119" GlobalId="3B4llDvRT06RwZqgSshzce" Name="CAFM-Connect Katalog 2014 zu IfcClassification">
    //  <RelatedObjects>
    //    <IfcProject ref="i100" xsi:nil="true" />
    //  </RelatedObjects>
    //  <RelatingClassification>
    //    <IfcClassification ref="i118" xsi:nil="true" />
    //  </RelatingClassification>
    //</IfcRelAssociatesClassification>

    // Error: <IfcProject ref="i100" xsi:nil="true" />
    // OK:    <IfcProject ref="i100" />

    // -----------------------------------------------------------------------------------  


    public class IfcXmlSerializer : System.Xml.Serialization.XmlSerializer
    {
        // public delegate void MessageLoggedEventHandler(object sender, MessageLoggedEventArgs e);
        public event Ifc4.Workspace.MessageLoggedEventHandler MessageLogged;

        public IfcXmlSerializer(Type type) : base(type)
        {
            this.UnknownAttribute += IfcXmlSerializer_UnknownAttribute;
            this.UnknownElement += IfcXmlSerializer_UnknownElement;
            this.UnknownNode += IfcXmlSerializer_UnknownNode;
            this.UnreferencedObject += IfcXmlSerializer_UnreferencedObject;
        }

        //public enum MessageLogType
        //{
        //    Info,
        //    Debug,
        //    Valid,
        //    Error,
        //    Warning
        //}

        //public class MessageLoggedEventArgs : System.EventArgs
        //{
        //    public MessageLoggedEventArgs()
        //        : this(new List<string> { }, MessageLogType.Info, false, false)
        //    {
        //    }

        //    public MessageLoggedEventArgs(Exception exception)
        //        : this((exception != null ? exception.Message : ""))
        //    {
        //        Exception = exception;
        //    }

        //    public MessageLoggedEventArgs(string message)
        //        : this(message, MessageLogType.Info, false, false)
        //    {
        //    }

        //    public MessageLoggedEventArgs(string message, MessageLogType type)
        //        : this(message, type, false, false)
        //    {
        //    }

        //    public MessageLoggedEventArgs(string message, MessageLogType type, bool doEvents, bool clear)
        //        : base()
        //    {
        //        Exception = null;
        //        Type = type;
        //        Messages = new List<string>();
        //        AddMessage(message);
        //        DoEvents = doEvents;
        //        Clear = clear;
        //    }

        //    public MessageLoggedEventArgs(List<string> messages, MessageLogType type, bool doEvents, bool clear)
        //        : base()
        //    {
        //        Exception = null;
        //        Type = type;
        //        Messages = new List<string>();
        //        if (messages != null)
        //            Messages.AddRange(messages);
        //        DoEvents = doEvents;
        //        Clear = clear;
        //    }

        //    public string Message
        //    {
        //        get { return String.Join("", Messages.ToArray()); }
        //    }
        //    public List<string> Messages { get; private set; }
        //    public Exception Exception { get; private set; }
        //    public bool DoEvents { get; private set; }
        //    public bool Clear { get; private set; }
        //    public MessageLogType Type { get; set; }

        //    public void AddMessage(string message)
        //    {
        //        if (message == null)
        //            return;

        //        Messages.Add(String.Concat("\r\n", DateTime.Now.ToLongTimeString().PadRight(12), message));
        //    }
        //}

        //internal bool RaiseMessageLogged(object sender, MessageLoggedEventArgs e)
        //{
        //    if (MessageLogged != null)
        //    {
        //        MessageLogged(sender, e);
        //        return true;
        //    }
        //    return false;
        //}

        //internal bool RaiseMessageLogged(string message)
        //{
        //    return RaiseMessageLogged(this, new MessageLoggedEventArgs(message));
        //}

        //internal void RaiseMessageLogged(Exception exc)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendLine(exc.Message);

        //    Exception innerException = exc.InnerException;
        //    while (innerException != null)
        //    {
        //        sb.AppendLine(innerException.Message);
        //        innerException = innerException.InnerException;
        //    }

        //    RaiseMessageLogged(sb.ToString());
        //}

        public string Serialize<T>(T t) where T : class
        {

            // see http://stackoverflow.com/questions/1408336/xmlserialization-and-xsischemalocation-xsd-exe
            // add field to ifcXML class
            //public partial class ifcXML : uos
            //{

            //    [System.Xml.Serialization.XmlAttribute("schemaLocation", Namespace = System.Xml.Schema.XmlSchema.InstanceNamespace)]
            //    public string xsiSchemaLocation = @"http://www.buildingsmart-tech.org/ifcXML/IFC4/final http://www.buildingsmart-tech.org/ifcXML/IFC4/final/ifcXML4.xsd";


            //// Original Doku
            //<ifcXML
            //xmlns="http://www.buildingsmart-tech.org/ifcXML/IFC4/final"
            //xmlns:ifc="http://www.buildingsmart-tech.org/ifcXML/IFC4/final"
            //xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
            //xsi:schemaLocation="http://www.buildingsmart-tech.org/ifcXML/IFC4/final http://www.buildingsmart-tech.org/ifcXML/IFC4/final/ifcXML4.xsd"
            //id="ifcXML4">
            //<!-- any content -->
            //</ifcXML>

            // -------------------------------------------------------------------------------

            //// Catalogue Export
            //<ifcXML
            //xmlns:ifc="http://www.buildingsmart-tech.org/ifcXML/IFC4/final"
            //xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
            //xsi:schemaLocation="http://www.buildingsmart-tech.org/ifcXML/IFC4/final http://www.buildingsmart-tech.org/ifcXML/IFC4/final/ifcXML4.xsd"
            //xmlns="http://www.buildingsmart-tech.org/ifcXML/IFC4/final"
            //id="ifcXML4">
            //<!-- any content -->
            //</ifcXML>

            // -------------------------------------------------------------------------------

            //// jvTest
            //<ifcXML
            //xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
            //xmlns:xsd="http://www.w3.org/2001/XMLSchema"
            //xmlns="http://www.buildingsmart-tech.org/ifcXML/IFC4/final"
            //id="ifcXML4"
            //express=""
            //configuration="">
            //<!-- any content -->
            //</ifcXML>

            // -------------------------------------------------------------------------------

            System.Xml.Serialization.XmlSerializerNamespaces namespaces = new System.Xml.Serialization.XmlSerializerNamespaces();
            //namespaces.Add("", "http://www.buildingsmart-tech.org/ifcXML/IFC4/final");
            //namespaces.Add("ifc", "http://www.buildingsmart-tech.org/ifcXML/IFC4/final");
            //namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            //namespaces.Add("schemaLocation", "http://www.buildingsmart-tech.org/ifcXML/IFC4/final http://www.buildingsmart-tech.org/ifcXML/IFC4/final/ifcXML4.xsd");

            // Type type = o.GetType();
            string xml = String.Empty;

            //namespaces = new XmlSerializerNamespaces(new System.Xml.XmlQualifiedName[]{
            //    new System.Xml.XmlQualifiedName("ifc", "http://www.buildingsmart-tech.org/ifcXML/IFC4/final"),
            //    new System.Xml.XmlQualifiedName("xsi", "http://www.w3.org/2001/XMLSchema-instance"),
            //    new System.Xml.XmlQualifiedName("schemaLocation", "http://www.buildingsmart-tech.org/ifcXML/IFC4/final http://www.buildingsmart-tech.org/ifcXML/IFC4/final/ifcXML4.xsd")
            //});

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    var xmlWriterSettings = new System.Xml.XmlWriterSettings
                    {
                        Encoding = new UTF8Encoding(false),
                        OmitXmlDeclaration = false,
                        Indent = true,
                        NamespaceHandling = System.Xml.NamespaceHandling.OmitDuplicates,
                        CheckCharacters = true,
                    };

                    using (System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(memoryStream, xmlWriterSettings))
                    {
                        Ifc4.Workspace.CurrentWorkspace.RaiseMessageLogged("Serialize...");
                        if (namespaces.Count > 0)
                            this.Serialize(xmlWriter, t.GetType(), namespaces);
                        else
                            this.Serialize(xmlWriter, t);

                    }

                    xml = Encoding.UTF8.GetString(memoryStream.ToArray());

                    //// -------------------------------------------------------------------------------
                    //RaiseMessageLogged("Namespace correction...");
                    //xml = xml.Replace(
                    //                    " xmlns:schemaLocation=",
                    //                    " xsi:schemaLocation="
                    //                );
                    //// ---------------------------------------
                }
                return xml;
            }
            catch (Exception exc)
            {
                Ifc4.Workspace.CurrentWorkspace.RaiseMessageLogged(this, exc);
                return String.Empty;
            }
        }

        void IfcXmlSerializer_UnreferencedObject(object sender, UnreferencedObjectEventArgs e)
        {
        }

        void IfcXmlSerializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
        }

        void IfcXmlSerializer_UnknownElement(object sender, XmlElementEventArgs e)
        {
        }

        void IfcXmlSerializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
        }


        private static readonly Dictionary<Type, XmlSerializer> m_XmlSerializers = new Dictionary<Type, XmlSerializer>();
        private static long m_XmlSerializersCounter;
        public static T DeserializeObject<T>(string xmlString)
        {
            T xsDeserialize = default(T);
            if (!String.IsNullOrEmpty(xmlString))
            {
                Type type = typeof(T);
                XmlSerializer xmlSerializer;
                if (!m_XmlSerializers.TryGetValue(type, out xmlSerializer))
                {

                    try
                    {
                        xmlSerializer = new XmlSerializer(type);
                    }
                    catch (Exception exc)
                    {
                        System.Diagnostics.Debug.WriteLine(exc.Message);
                        Exception innerException = exc.InnerException;
                        while (innerException != null)
                        {
                            System.Diagnostics.Debug.WriteLine(innerException.Message);
                            innerException = innerException.InnerException;
                        }
                    }
                    //TODO prüfen ob man das wieder aktivieren kann
                    //m_XmlSerializers.Add(type, xmlSerializer);
                }

                if (xmlSerializer == null)
                    xmlSerializer = new XmlSerializer(type);

                using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(new StringReader(xmlString)))
                {
                    xsDeserialize = (T)xmlSerializer.Deserialize(reader);
                }
                m_XmlSerializersCounter++;
            }
            return xsDeserialize;
        }

        private string Path
        {
            get
            {
                string path;

                if (System.Environment.MachineName.IndexOf("toshiba", StringComparison.OrdinalIgnoreCase) != -1 || System.Environment.MachineName.IndexOf("pc", StringComparison.OrdinalIgnoreCase) != -1)
                    path = @"C:\SVNLocal\jvDevelopment_eTASK\etask.Windows.Ifc\DebugFolder";
                else
                    path = @"D:\jv\etask.Windows.Ifc\DebugFolder";

                return path;
            }
        }


        private Encoding m_Encoding;
        private string ConvertISO8859_1ToUTF8(string s)
        {
            if (String.IsNullOrEmpty(s))
                return String.Empty;

            // der string sieht mir schwer nach iso-8859-1 aus
            // Westeuropäisch (ISO)
            // iso-8859-1
            if (m_Encoding == null)
                m_Encoding = Encoding.GetEncoding(28591);

            // ASCII  ö = 228
            //var xxx = m_Encoding.GetString(Encoding.UTF8.GetBytes("ö"));

            byte[] ba = m_Encoding.GetBytes(s);
            return Encoding.UTF8.GetString(ba);
        }

        private string ConvertUTF8ToISO8859_1(string s)
        {
            if (String.IsNullOrEmpty(s))
                return String.Empty;

            if (m_Encoding == null)
                m_Encoding = Encoding.GetEncoding(28591);

            return m_Encoding.GetString(Encoding.UTF8.GetBytes(s));
        }

    }

}


namespace JV
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;


    public static class XmlProcessing<T> where T : class
    {
        public static void Write(T t, string fullName)
        {
            try
            {
                if (t == null)
                    return;

                if (String.IsNullOrEmpty(fullName))
                    return;

                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(fullName, false, Encoding.UTF8))
                {
                    serializer.Serialize(streamWriter, t);
                }
            }
            catch (Exception exc)
            {
                throw new Exception(String.Format("Cannot serialize '{0}' to file '{1}'.", t.GetType().FullName, fullName), exc);
            }
        }
        public static T Read(string fullName)
        {
            T t = default(T);
            try
            {
                if (String.IsNullOrEmpty(fullName))
                    return t;

                if (!System.IO.File.Exists(fullName))
                    return t;

                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                using (System.IO.StreamReader reader = new System.IO.StreamReader(fullName, Encoding.UTF8, true))
                {
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);


                    //// Stammelement mit angeben
                    //XmlRootAttribute xRoot = new XmlRootAttribute();
                    //xRoot.ElementName = "ifcXML";
                    //xRoot.Namespace = "http://www.buildingsmart-tech.org/ifcXML/IFC4/final";
                    //xRoot.IsNullable = true;

                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

                    //XmlDeserializationEvents events = new XmlDeserializationEvents();
                    //events.OnUnknownAttribute = OnUnknownAttribute;
                    //events.OnUnknownElement = OnUnknownElement;
                    //events.OnUnknownNode = OnUnknownNode;
                    //events.OnUnreferencedObject = OnUnreferencedObject;

                    //t = (T)serializer.Deserialize(System.Xml.XmlReader.Create(reader), events);

                    t = (T)serializer.Deserialize(System.Xml.XmlReader.Create(reader));

                }

                sw.Stop();
                System.Diagnostics.Debug.WriteLine(String.Format("Read '{0}': {1}ms", fullName, sw.ElapsedMilliseconds));

                return t;
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine(exc.Message);
                Exception innerException = exc.InnerException;
                while (innerException != null)
                {
                    System.Diagnostics.Debug.WriteLine(innerException.Message);
                    innerException = innerException.InnerException;
                }
                Ifc4.Workspace.CurrentWorkspace.RaiseMessageLogged(exc);
                throw new Exception(String.Format("Cannot deserialize file '{0}' to '{1}'.", fullName, t.GetType().FullName), exc);
            }
        }

        public static T ReadBinary(string fullName)
        {
            T t = default(T);
            try
            {
                if (String.IsNullOrEmpty(fullName))
                    return t;

                if (!System.IO.File.Exists(fullName))
                    return t;

                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                FileStream fs = new FileStream(fullName, FileMode.Open);
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    t = (T)formatter.Deserialize(fs);
                }
                catch (Exception exc)
                {
                    throw;
                }
                finally
                {
                    fs.Close();
                }

                sw.Stop();
                System.Diagnostics.Debug.WriteLine(String.Format("Read '{0}': {1}ms", fullName, sw.ElapsedMilliseconds));

                return t;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        public static bool SerializeBinary(string fullName, object obj)
        {
            if(obj == null)
                return false;

            FileStream fs = new FileStream(fullName, FileMode.Create);

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            try
            {
                binaryFormatter.Serialize(fs, obj);
                return true;
            }
            catch (Exception exc)
            {
                throw;
            }
            finally
            {
                fs.Close();
            }
        }


        public static T Read(Stream stream)
        {
            T t = default(T);
            try
            {
                if (stream == null)
                    return t;

                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8, true))
                {
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);

                    //// Stammelement mit angeben
                    //XmlRootAttribute xRoot = new XmlRootAttribute();
                    //xRoot.ElementName = "ifcXML";
                    //xRoot.Namespace = "http://www.buildingsmart-tech.org/ifcXML/IFC4/final";
                    //xRoot.IsNullable = true;

                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

//#if DEBUG
//                    XmlDeserializationEvents events = new XmlDeserializationEvents();
//                    events.OnUnknownAttribute = OnUnknownAttribute;
//                    events.OnUnknownElement = OnUnknownElement;
//                    events.OnUnknownNode = OnUnknownNode;
//                    events.OnUnreferencedObject = OnUnreferencedObject;
//                    t = (T)serializer.Deserialize(System.Xml.XmlReader.Create(reader), events);
//#else
//                    t = (T)serializer.Deserialize(System.Xml.XmlReader.Create(reader));
//#endif

                    t = (T)serializer.Deserialize(System.Xml.XmlReader.Create(reader));

                }
                return t;
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine(exc.Message);
                Exception innerException = exc.InnerException;
                while (innerException != null)
                {
                    System.Diagnostics.Debug.WriteLine(innerException.Message);
                    innerException = innerException.InnerException;
                }
                throw new Exception(String.Format("Cannot deserialize stream '{0}'.", t.GetType().FullName), exc);
            }
        }


        private static void OnUnknownAttribute(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("----------------------------------------------------------------");
            System.Diagnostics.Debug.WriteLine("OnUnknownAttribute " + e.GetType().FullName);
            ABC(sender, e);
        }

        private static void OnUnknownElement(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("----------------------------------------------------------------");
            System.Diagnostics.Debug.WriteLine("OnUnknownElement " + e.GetType().FullName);
            ABC(sender, e);
        }

        private static void OnUnknownNode(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("----------------------------------------------------------------");
            System.Diagnostics.Debug.WriteLine("OnUnknownNode " + e.GetType().FullName);
            ABC(sender, e);
        }

        private static void OnUnreferencedObject(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("----------------------------------------------------------------");
            System.Diagnostics.Debug.WriteLine("OnUnreferencedObject " + e.GetType().FullName);
            ABC(sender, e);
        }

        private static void ABC(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.GetType().FullName);
            foreach (var p in e.GetType().GetProperties())
            {
                var o = p.GetValue(e, new object[] { });
                System.Diagnostics.Debug.WriteLine(p.Name + ": " + (o == null ? "<null>" : o.ToString()));
            }
        }


        static void serializer_UnreferencedObject(object sender, UnreferencedObjectEventArgs e)
        {
            //throw new NotImplementedException();
        }

        static void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            //throw new NotImplementedException();
        }

        static void serializer_UnknownElement(object sender, XmlElementEventArgs e)
        {
            //throw new NotImplementedException();
        }

        static void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public static void Write(T t, System.IO.Stream stream)
        {
            try
            {
                if (t == null)
                    return;

                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
                using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(memoryStream, Encoding.UTF8))
                {
                    serializer.Serialize(streamWriter, t);
                    memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                    CopyStream(memoryStream, stream);
                }
            }
            catch (Exception exc)
            {
                throw new Exception(String.Format("Cannot serialize '{0}' to stream.", t.GetType().FullName), exc);
            }
        }
        public static T Read2(System.IO.Stream stream)
        {
            T t = default(T);
            try
            {
                if (stream.CanSeek)
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8, true))
                {
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    t = (T)serializer.Deserialize(reader);
                }
                return t;
            }
            catch (Exception exc)
            {
                throw new Exception(String.Format("Cannot deserialize stream to '{0}'.", t.GetType().FullName), exc);
            }
        }

        private static void CopyStream(System.IO.Stream source, System.IO.Stream target)
        {
            const int bufferSize = 0x1000; //4096
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;
            while ((bytesRead = source.Read(buffer, 0, bufferSize)) > 0)
                target.Write(buffer, 0, bytesRead);
        }

        public static void XmlWrite(object oClass, Type type, System.IO.Stream stream)
        {
            try
            {
                if (oClass == null)
                    return;

                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(type);
                System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
                using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(memoryStream, Encoding.UTF8))
                {
                    serializer.Serialize(streamWriter, oClass);
                    memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                    CopyStream(memoryStream, stream);
                }
            }
            catch (Exception exc)
            {
                throw new Exception(String.Format("Cannot serialize '{0}' to stream.", type.FullName), exc);
            }
        }

        public static object XmlRead(System.IO.Stream stream, Type type)
        {
            try
            {
                if (stream.CanSeek)
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                object result;
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8, true))
                {
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(type);
                    result = serializer.Deserialize(reader);
                }
                return result;
            }
            catch (Exception exc)
            {
                throw new Exception(String.Format("Cannot deserialize stream to '{0}'.", type.FullName), exc);
            }
        }


    }
}