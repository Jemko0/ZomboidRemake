using System;
using System.IO;
using System.Text;
using System.Xml;
using Iso.Engine.Core.Logging;
using Iso.Engine.Core.UI.Extensions;
using Iso.Engine.Core.XML;

namespace Iso.Engine.Core.UI.Elements.XML
{
    public class DeclarativeElement : Panel
    {
        public string XMLString = "";
        public DeclarativeElement()
        {

        }

        public DeclarativeElement(string xml)
        {
            XMLString = xml;
            LoadFromXMLString();
        }

        public DeclarativeElement(FileStream fs)
        {
            using StreamReader reader = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            XMLString = reader.ReadToEnd();

            LoadFromXMLString();
        }

        public DeclarativeElement SetXML(string xml)
        {
            XMLString = xml;
            return this;
        }

        private void LoadFromXMLString()
        {
            IsoLog.Log("LogXML", "Loading from XML String");
            IsoLog.Logf("LogXML", "XML content: {0}", args: XMLString);

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(XMLString);

                XmlNode root = doc.DocumentElement;

                if (root == null)
                {
                    IsoLog.Log("LogXML", "root is NULL - XML parsed but no document element");
                    return;
                }

                IsoLog.Logf("LogXML", "root: {0}", args: root.Name);
                UIElement element = IsoXML.XMLToUI.ParseNode(root);
                this.Attach(element);
            }
            catch (XmlException ex)
            {
                IsoLog.Logf("LogXML", "XML parse failed: {0}", args: ex.Message);
            }
            catch (Exception ex)
            {
                IsoLog.Logf("LogXML", "Unexpected error: {0}", args: ex.Message);
            }
        }
    }
}