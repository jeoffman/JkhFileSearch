using System;
using System.Windows.Forms;
//using CustomSettingsXML;

namespace jkhFileSearch
{
	public partial class FormLog : Form
	{
		public FormLog()
		{
			InitializeComponent();
		}

		private void FormLog_Load(object sender, EventArgs e)
		{
			CustomSettings settings = new CustomSettings();
			settings.RestoreWindowPlacement(this);
		}

		private void FormLog_FormClosing(object sender, FormClosingEventArgs e)
		{
			CustomSettings settings = new CustomSettings();
			settings.SaveWindowPlacement(this);
		}
	}
}
