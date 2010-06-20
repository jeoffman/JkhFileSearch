using System;
using System.Collections;	//ArrayList
using System.Diagnostics;	//Debug
using System.Drawing;	//Size
using System.Globalization;	//CultureInfo
using System.Text;	//StringBuilder
using System.Windows.Forms;
using System.Xml;




namespace CustomSettingsXML
{
	public class CustomSettings
	{
		XmlDocument xmlDocument = new XmlDocument();
		string documentPath = Application.StartupPath + "//settings.xml";

		public CustomSettings()
		{
			try { xmlDocument.Load(documentPath); }
			catch { xmlDocument.LoadXml("<settings></settings>"); }
		}

		private XmlNode createMissingNode(string xPath)
		{
			string[] xPathSections = xPath.Split('/');
			StringBuilder currentXPath = new StringBuilder();
			XmlNode testNode = null;
			XmlNode currentNode = xmlDocument.SelectSingleNode("settings");
			foreach(string xPathSection in xPathSections)
			{
				currentXPath.Append(xPathSection);
				testNode = xmlDocument.SelectSingleNode(currentXPath.ToString());
				if(testNode == null)
				{
					currentNode.InnerXml += "<" +
								xPathSection + "></" +
								xPathSection + ">";
				}
				currentNode = xmlDocument.SelectSingleNode(currentXPath.ToString());
				currentXPath.Append("/");
			}
			return currentNode;
		}

		#region Get/Put
		public string GetSetting(string xPath, string defaultValue)
		{
			XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
			if(xmlNode != null) { return xmlNode.InnerText; }
			else { return defaultValue; }
		}

		public void PutSetting(string xPath, string value)
		{
			XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
			if(xmlNode == null) { xmlNode = createMissingNode("settings/" + xPath); }
			xmlNode.InnerText = value;
			xmlDocument.Save(documentPath);
		}

		public int GetSetting(string xPath, int defaultValue)
		{
			return Convert.ToInt16(GetSetting(xPath, Convert.ToString(defaultValue, CultureInfo.CurrentCulture)), CultureInfo.CurrentCulture);
		}

		public bool GetSetting(string xPath, bool defaultValue)
		{
			return Convert.ToBoolean(GetSetting(xPath, Convert.ToString(defaultValue, CultureInfo.CurrentCulture)), CultureInfo.CurrentCulture);
		}

		public void PutSetting(string xPath, int value)
		{
			PutSetting(xPath, Convert.ToString(value, CultureInfo.CurrentCulture));
		}

		public void PutSetting(string xPath, bool value)
		{
			PutSetting(xPath, Convert.ToString(value, CultureInfo.CurrentCulture));
		}

		public Size GetSetting(string xPath, Size defaultValue)
		{
			Size retval = defaultValue;
			try
			{
				XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
				if(xmlNode != null)
				{
					string[] sizes = xmlNode.InnerText.Split(',');
					if(sizes.Length == 2)
					{
						try
						{
							retval.Width = int.Parse(sizes[0], CultureInfo.CurrentCulture);
							retval.Height = int.Parse(sizes[1], CultureInfo.CurrentCulture);
						}
						catch(Exception exc)
						{
							Debug.Assert(exc != null);
							throw;
						}
					}
				}
			}
			catch(Exception exc)
			{
				Debug.Assert(exc != null);
				throw;
			}
			return retval;
		}

		public void PutSetting(string xPath, Size value)
		{
			XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
			if(xmlNode == null)
				xmlNode = createMissingNode("settings/" + xPath);
			if(xmlNode != null)
				xmlNode.InnerText = value.Width + "," + value.Height;
			xmlDocument.Save(documentPath);
		}

		public Point GetSetting(string xPath, Point defaultValue)
		{
			Point retval = defaultValue;
			XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
			if(xmlNode != null)
			{
				string[] points = xmlNode.InnerText.Split(',');
				if(points.Length == 2)
				{
					try
					{
						retval.X = int.Parse(points[0], CultureInfo.CurrentCulture);
						retval.Y = int.Parse(points[1], CultureInfo.CurrentCulture);
					}
					catch(Exception exc)
					{
						Debug.Assert(exc != null);
						throw;
					}
				}
			}
			return retval;
		}

		public void PutSetting(string xPath, Point value)
		{
			XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
			if(xmlNode == null)
				xmlNode = createMissingNode("settings/" + xPath);
			if(xmlNode != null)
				xmlNode.InnerText = value.X + "," + value.Y;
			xmlDocument.Save(documentPath);
		}

		public string[] GetSetting(string xPath, string[] defaultValue)
		{
			XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
			if(xmlNode != null)
			{
				ArrayList retval = new ArrayList();
				if(xmlNode.ChildNodes.Count > 0)
				{
					foreach(XmlNode childNode in xmlNode)
						retval.Add(childNode.InnerText);
					return (string[])retval.ToArray(typeof(string));
				}
				else
				{
					return new string[0];
				}
			}
			else
			{
				return defaultValue;
			}
		}

		public void PutSetting(string xPath, string[] values)
		{
			XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
			if(xmlNode == null)
				xmlNode = createMissingNode("settings/" + xPath);
			else
				xmlNode.RemoveAll();
			if(values != null)
			{
				foreach(string value in values)
				{
					XmlElement child = xmlDocument.CreateElement("value");
					child.InnerText = value;
					xmlNode.AppendChild(child);
				}
			}
			xmlDocument.Save(documentPath);
		}
		#endregion Get/Put

		public void SaveWindowPlacement(Form formTarget)
		{
			if(formTarget != null)
			{
				// Save formTarget position to file, so we can restore it later.
				RectangleConverter converter = new RectangleConverter();
				string formPosition = converter.ConvertToString(formTarget.Bounds);
				PutSetting(formTarget.Name + "_Bounds", formPosition);
			}
		}

		public void RestoreWindowPlacement(Form formTarget)
		{
			if(formTarget != null)
			{
				RectangleConverter converter = new RectangleConverter();

				Rectangle formBounds = (Rectangle)converter.ConvertFromString(GetSetting(formTarget.Name + "_Bounds", converter.ConvertToString(Rectangle.Empty)));
				if(!formBounds.IsEmpty)
				{
					// Get the working area of the monitor that contains this rectangle
					//  (in case it’s a multi-display system)
					Rectangle workingArea = Screen.GetWorkingArea(formBounds);

					// If the bounds are outside of the screen’s work area, move the
					// formTarget so it’s not outside of the work area. This can happen if the
					// user changes their resolution and we then restore the application
					// into its position — it may be off screen and then they can’t see it
					// or move it.
					if(formBounds.Left < workingArea.Left)
						formBounds.Location = new Point(workingArea.Location.X, formBounds.Location.Y);
					if(formBounds.Top < workingArea.Top)
						formBounds.Location = new Point(formBounds.Location.X, workingArea.Location.Y);
					if(formBounds.Right > workingArea.Right)
						formBounds.Location = new Point(formBounds.X - (formBounds.Right - workingArea.Right), formBounds.Location.Y);
					if(formBounds.Bottom > workingArea.Bottom)
						formBounds.Location = new Point(formBounds.X, formBounds.Y - (formBounds.Bottom - workingArea.Bottom));

					formTarget.Bounds = formBounds;
				}
			}
		}

		public static string GetColumnWidths(ListView lv)
		{
			StringBuilder retval = new StringBuilder();
			if(lv != null)
			{
				foreach(ColumnHeader column in lv.Columns)
				{
					if(retval.Length > 0)
						retval.Append(",");
					retval.Append(column.Width.ToString(CultureInfo.CurrentCulture));
				}
			}
			return retval.ToString();
		}

		static public string GetControlUniversalName(Control ctrl)
		{
			StringBuilder retval = new StringBuilder();
			if(ctrl != null)
			{
				Control parent = ctrl.Parent;
				retval.Append(ctrl.Name);
				while(parent != null)
				{
					if(!string.IsNullOrEmpty(parent.Name))
					{
						retval.Insert(0, '.');
						retval.Insert(0, parent.Name);
					}
					parent = parent.Parent;
				}
			}
			return retval.ToString();
		}

		public void SaveColumnWidths(ListView lv)
		{
			string columnWidths = GetColumnWidths(lv);
			PutSetting(GetControlUniversalName(lv) + "_ColumnWidths", columnWidths.ToString());
		}

		public bool RestoreColumnWidths(ListView lv)
		{
			bool retval = false;
			if(lv != null)
			{
				string columnWidths = GetSetting(GetControlUniversalName(lv) + "_ColumnWidths", "");

				string[] widths = columnWidths.Split(',');
				for(int count = 0; count < widths.Length; count++)
				{
					if(lv.Columns.Count > count)
					{
						int width;
						if(int.TryParse(widths[count].Trim(), out width))
							lv.Columns[count].Width = width;
					}
					else
					{
						Debug.Assert(false);
						break;	// too many initializers!
					}
				}
				retval = widths.Length > 0;
			}
			return retval;
		}
	}
}
