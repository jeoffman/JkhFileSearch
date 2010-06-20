namespace jkhFileSearch
{
	partial class FormSettings
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.buttonRegistryContext = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonRegistryContext
			// 
			this.buttonRegistryContext.Location = new System.Drawing.Point(172, 12);
			this.buttonRegistryContext.Name = "buttonRegistryContext";
			this.buttonRegistryContext.Size = new System.Drawing.Size(108, 35);
			this.buttonRegistryContext.TabIndex = 0;
			this.buttonRegistryContext.Text = "Add To Right-Click Context Menu";
			this.buttonRegistryContext.UseVisualStyleBackColor = true;
			this.buttonRegistryContext.Click += new System.EventHandler(this.buttonRegistryContext_Click);
			// 
			// FormSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.buttonRegistryContext);
			this.Name = "FormSettings";
			this.Text = "FormSettings";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonRegistryContext;
	}
}