using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ifc4.Interfaces;

namespace Ifc4
{
    public class Documents : BaseObjects<Document>
    {

        internal Documents(IBaseObject parent)
            : base(parent)
        {
        }

        internal void CloseAll()
        {
            foreach (Document document in this.ToArray())
            {
                if (document != null)
                    document.Close();
            }
            ((System.Collections.IList)((Interfaces.IBaseObjects<Document>)this)).Clear();
        }

        internal Document CreateDocument(string fullName, bool overwriteIfExists)
        {
            ActiveDocument = null;
            if (overwriteIfExists && System.IO.File.Exists(fullName))
            {
                try { System.IO.File.Delete(fullName); }
                catch (Exception exc)
                {
                    Workspace.CurrentWorkspace.RaiseMessageLogged(this, exc);
                    return null;
                }
            }

            try
            {
                Document document = new Document(this);
                ((IBaseObjects<Document>)this).Add(document);
                document.FullName = fullName;
                ActiveDocument = document;

                //document.InitializeDefaultValues();
                //document.Classifications.InitializeDefaultValues();

                document.ResetDirty();
                return document;
            }
            catch (Exception exc)
            {
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, exc);
                return null;
            }
        }

        protected Document _ActiveDocument;
        internal Document ActiveDocument
        {
            get { return _ActiveDocument; }
            set
            {
                if (_ActiveDocument != value)
                {
                    _ActiveDocument = value;
                    Workspace.CurrentWorkspace.RaiseActiveDocumentChanged(this, new Workspace.ActiveDocumentChangedEventArgs(_ActiveDocument));
                }
            }
        }

        //internal void ConvertToIso8859_1(string fullName)
        //{
        //    if (string.IsNullOrEmpty(fullName))
        //        return;

        //    string s = System.IO.File.ReadAllText(fullName);
        //    s = this.SpecialCharacterToUnicode(s);
        //    System.IO.File.WriteAllText(fullName, s, Encoding.ASCII);
        //}

        internal Document Open(string fullName, Ifc4.Document.IfcFileType ifcFileType = Document.IfcFileType.Auto)
        {
            EventType eventType = BaseObject.LockEvents();

            Document document = null;
            try
            {
                Workspace.CurrentWorkspace.RaiseBeforeCommandExceuted(this, "OpenDocument");

                document = this.Where(item => item.FullName.Equals(fullName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (document != null)
                {
                    ActiveDocument = document;
                    return document;
                }

                // Sicherheitshalber alle Dokumente schließen
                Workspace.CurrentWorkspace.CloseDocument();

                //ConvertToIso8859_1(fullName);

                Ifc4.CcFacility.ResetVariables();

                document = AddNew() as Document;
                if (document == null)
                    return null;

                document.IsInOpenProcess = true;
                string message;

                Workspace.CurrentWorkspace.RaiseMessageLogged(String.Format("Open document '{0}'...", fullName));

                //document.RaiseMessageLogged(String.Format("Open document '{0}'...", fullName));

                document.Open(fullName, out message, ifcFileType);
                document.Read(this);
                document.ResetDirty();

                Workspace.CurrentWorkspace.RaiseDocumentOpened(this, new Workspace.DocumentOpenedEventArgs(document));
                ActiveDocument = document;

                return document;
            }
            catch (Exception exc)
            {
                Workspace.CurrentWorkspace.RaiseMessageLogged(this, exc);
                return null;
            }
            finally
            {
                if (document != null)
                    document.IsInOpenProcess = false;
                BaseObject.UnlockEvents(eventType);
                Workspace.CurrentWorkspace.RaiseAfterCommandExceuted(this, "OpenDocument");
            }
        }

        public override object AddNew()
        {
            return AddNewDocument();
        }

        public Document AddNewDocument()
        {
            //if (this.Count() > 1)
            //    return null; // zur Zeit nur ein offenes Dokument zulässig!

            Document document = new Document(this);
            ((IBaseObjects<Document>)this).Add(document);
            return document;
        }



    }
}

