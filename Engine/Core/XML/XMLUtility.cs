using System.Data.Common;
using System.Reflection;
using System.Xml;
using Iso.Engine.Core.Converters;
using Iso.Engine.Core.Logging;
using Iso.Engine.Core.UI;
using Iso.Engine.Core.UI.Elements;
using Iso.Engine.Core.UI.Elements.Templates;
using Iso.Engine.Core.UI.Extensions;

namespace Iso.Engine.Core.XML
{
    public class IsoXML
    {
        public class XMLToUI
        {
            public static UIElement ParseNode(XmlNode node)
            {
                UIElement element = CreateElement(node.Name);

                ApplyAttributes(element, node.Attributes);

                IsoLog.Logf("LogXML", "Parsing XML Node: {0}", ELogVerbosity.Debug, node.Name);

                foreach (XmlNode child in node.ChildNodes)
                {
                    if (child.NodeType != XmlNodeType.Element)
                        continue;

                    if (child.Name == "slot")
                    {
                        HandleSlot(element, child);
                        continue;
                    }

                    UIElement childElement = ParseNode(child);
                    UIHierarchy.Attach(element, childElement);
                }

                return element;
            }

            private static void HandleSlot(UIElement element, XmlNode child)
            {
                string slotName = child.Attributes?["name"]?.Value;
                if (string.IsNullOrEmpty(slotName))
                    return;

                if (element is Wrapper wrapper && slotName == "background")
                {
                    XmlNode inner = child.FirstChild;

                    while (inner != null && inner.NodeType != XmlNodeType.Element)
                        inner = inner.NextSibling;

                    if (inner != null)
                    {
                        UIElement slotContent = ParseNode(inner);

                        if (slotContent is Image img)
                            wrapper.SetBackground(img);
                    }

                    return;
                }

                // unknown slot
                IsoLog.Log("LogXML", $"Unknown slot '{slotName}' on element {element.name}");
            }

            public static void ApplyAttributes(UIElement element, XmlAttributeCollection attributes)
            {

                if (attributes == null)
                {
                    return;
                }

                foreach (XmlAttribute attr in attributes)
                {
                    string name = attr.Name.ToLower();
                    string value = attr.Value;

                    switch (name)
                    {
                        case "name":
                            element.name = value;
                            break;

                        case "width":
                            if (value == "fill")
                            {
                                element.FillWidth(int.Parse(attributes["margin"].Value ?? "0"));
                                break;
                            }
                            element.bounds.Width = int.Parse(value);
                            break;

                        case "height":
                            element.bounds.Height = int.Parse(value);
                            break;

                        case "x":
                            element.bounds.X = int.Parse(value);
                            break;

                        case "y":
                            element.bounds.Y = int.Parse(value);
                            break;

                        case "anchormin":
                            element.anchorMin = IsoConvert.ParseVector2(value);
                            break;

                        case "anchormax":
                            element.anchorMax = IsoConvert.ParseVector2(value);
                            break;

                        case "pivot":
                            element.pivot = IsoConvert.ParseVector2(value);
                            break;

                        case "fillparent":
                            element.FillParent(int.Parse(value ?? "0"));
                            break;
                    }
                }
            }

            public static UIElement CreateElement(string name)
            {
                return name switch
                {
                    "window" => new Window(),
                    "hstack" => new HStack(),
                    "wrapper" => new Wrapper(),
                    "img" => new Image(),
                    "vstack" => new VStack(),
                    "scrollbox" => new ScrollBox(),
                    "button" => new Button(),
                    "windowtitlebar" => new WindowTitleBar(),
                    _ => new Panel()
                };
            }
        }
    }
}