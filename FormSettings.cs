using Microsoft.Win32;	//Registry
using System;
// using System.Collections.Generic;
// using System.ComponentModel;
// using System.Data;
// using System.Drawing;
// using System.Text;
using System.Windows.Forms;



namespace jkhFileSearch
{
	public partial class FormSettings : Form
	{
		public FormSettings()
		{
			InitializeComponent();
		}

		private void buttonRegistryContext_Click(object sender, EventArgs e)
		{
			try
			{
				RegistryKey ourKey = Registry.ClassesRoot.CreateSubKey(@"folder\shell\jkhFileSearch", RegistryKeyPermissionCheck.ReadWriteSubTree);
				ourKey.SetValue(null, "JKH Search");

				ourKey = ourKey.CreateSubKey("command", RegistryKeyPermissionCheck.ReadWriteSubTree);
				ourKey.SetValue(null, System.Reflection.Assembly.GetExecutingAssembly().Location + " \"%1\"", RegistryValueKind.ExpandString);
			}
			catch(UnauthorizedAccessException exc)
			{
				MessageBox.Show("Try running as administrator\n" + exc.Message);
			}
		}

		private void FormSettings_Load(object sender, EventArgs e)
		{
			using(CustomSettings settings = new CustomSettings())
			{
				settings.RestoreWindowPlacement(this);
			}
		}

		private void FormSettings_FormClosing(object sender, FormClosingEventArgs e)
		{
			using(CustomSettings settings = new CustomSettings())
			{
				settings.SaveWindowPlacement(this);
			}
		}
	}
}