using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using Microsoft.Win32;
using System.Net.Sockets;

namespace Temp
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class TempForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.NotifyIcon tray;
		private System.Windows.Forms.ContextMenu menu;
		private System.Windows.Forms.MenuItem exitMenuItem;
		private System.Windows.Forms.Timer tempTimer;
		private System.ComponentModel.IContainer components;

		private int _zipCode = 28223;
		private int _interval = 5;
		private float _lastTemp = -1;
		private float _temp = 0;
		private bool _wasError = false;
		private bool _showErrors = true;
		private bool _runAtStartup = false;		
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox zipTextBox;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button applyButton;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem aboutMenuItem;
		private System.Windows.Forms.MenuItem optionsMenuItem;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox startupCheckBox;
		private System.Windows.Forms.CheckBox errorsCheckBox;
		private System.Windows.Forms.TextBox intervalTextBox;		
		private TemperatureService service;

		public TempForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Send it to the tray on startup.
			this.ToTray();			

			// Load the options from the registry.
			this.LoadOptions();

			// Update the dialog for the new options.
			this.UpdateDialog();

			// Set the icon to ?? until it updates the temperature.
			this.UpdateIcon("??");

			// Initialize the service to get the temperature.
			service = new TemperatureService();			

			// Update the temperature.
			this.GetTemp();						
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (tray != null)
				{
					DestroyIcon(tray.Icon.Handle);
					tray.Icon.Dispose();
					tray.Dispose();
				}

				if (service != null)
					service.Dispose();

				if (components != null) 
				{
					components.Dispose();
				}
			}					

			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TempForm));
			this.tray = new System.Windows.Forms.NotifyIcon(this.components);
			this.menu = new System.Windows.Forms.ContextMenu();
			this.optionsMenuItem = new System.Windows.Forms.MenuItem();
			this.aboutMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.exitMenuItem = new System.Windows.Forms.MenuItem();
			this.tempTimer = new System.Windows.Forms.Timer(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.zipTextBox = new System.Windows.Forms.TextBox();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.applyButton = new System.Windows.Forms.Button();
			this.intervalTextBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.startupCheckBox = new System.Windows.Forms.CheckBox();
			this.errorsCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// tray
			// 
			this.tray.ContextMenu = this.menu;
			this.tray.Icon = ((System.Drawing.Icon)(resources.GetObject("tray.Icon")));
			this.tray.Text = "??";
			this.tray.Visible = true;
			this.tray.DoubleClick += new System.EventHandler(this.tray_DoubleClick);
			// 
			// menu
			// 
			this.menu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				 this.optionsMenuItem,
																				 this.aboutMenuItem,
																				 this.menuItem1,
																				 this.exitMenuItem});
			// 
			// optionsMenuItem
			// 
			this.optionsMenuItem.Index = 0;
			this.optionsMenuItem.Text = "Options";
			this.optionsMenuItem.Click += new System.EventHandler(this.optionsMenuItem_Click);
			// 
			// aboutMenuItem
			// 
			this.aboutMenuItem.Index = 1;
			this.aboutMenuItem.Text = "About";
			this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 2;
			this.menuItem1.Text = "-";
			// 
			// exitMenuItem
			// 
			this.exitMenuItem.Index = 3;
			this.exitMenuItem.Text = "Exit";
			this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
			// 
			// tempTimer
			// 
			this.tempTimer.Enabled = true;
			this.tempTimer.Interval = 300000;
			this.tempTimer.Tick += new System.EventHandler(this.tempTimer_Tick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Zip Code:";
			// 
			// zipTextBox
			// 
			this.zipTextBox.Location = new System.Drawing.Point(136, 12);
			this.zipTextBox.Name = "zipTextBox";
			this.zipTextBox.Size = new System.Drawing.Size(104, 20);
			this.zipTextBox.TabIndex = 1;
			this.zipTextBox.Text = "28223";
			this.zipTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.zipTextBox.TextChanged += new System.EventHandler(this.zipTextBox_TextChanged);
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(8, 96);
			this.okButton.Name = "okButton";
			this.okButton.TabIndex = 2;
			this.okButton.Text = "&OK";
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(88, 96);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.TabIndex = 3;
			this.cancelButton.Text = "&Cancel";
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// applyButton
			// 
			this.applyButton.Enabled = false;
			this.applyButton.Location = new System.Drawing.Point(168, 96);
			this.applyButton.Name = "applyButton";
			this.applyButton.TabIndex = 4;
			this.applyButton.Text = "&Apply";
			this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
			// 
			// intervalTextBox
			// 
			this.intervalTextBox.Location = new System.Drawing.Point(136, 40);
			this.intervalTextBox.Name = "intervalTextBox";
			this.intervalTextBox.Size = new System.Drawing.Size(104, 20);
			this.intervalTextBox.TabIndex = 5;
			this.intervalTextBox.Text = "5";
			this.intervalTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.intervalTextBox.TextChanged += new System.EventHandler(this.intervalTextBox_TextChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 44);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(120, 16);
			this.label2.TabIndex = 6;
			this.label2.Text = "Update Interval (Min):";
			// 
			// startupCheckBox
			// 
			this.startupCheckBox.Location = new System.Drawing.Point(8, 72);
			this.startupCheckBox.Name = "startupCheckBox";
			this.startupCheckBox.Size = new System.Drawing.Size(104, 20);
			this.startupCheckBox.TabIndex = 7;
			this.startupCheckBox.Text = "Run at startup";
			this.startupCheckBox.CheckedChanged += new System.EventHandler(this.startupCheckBox_CheckedChanged);
			// 
			// errorsCheckBox
			// 
			this.errorsCheckBox.Location = new System.Drawing.Point(136, 72);
			this.errorsCheckBox.Name = "errorsCheckBox";
			this.errorsCheckBox.Size = new System.Drawing.Size(104, 20);
			this.errorsCheckBox.TabIndex = 8;
			this.errorsCheckBox.Text = "Show Errors";
			this.errorsCheckBox.CheckedChanged += new System.EventHandler(this.errorsCheckBox_CheckedChanged);
			// 
			// TempForm
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(250, 123);
			this.Controls.Add(this.errorsCheckBox);
			this.Controls.Add(this.startupCheckBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.intervalTextBox);
			this.Controls.Add(this.applyButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.zipTextBox);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TempForm";
			this.ShowInTaskbar = false;
			this.Text = "Temp";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.TempForm_Closing);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new TempForm());
		}

		private void exitMenuItem_Click(object sender, System.EventArgs e)
		{
			// When the tray icon is told to exit destroy the current icon.
			DestroyIcon(tray.Icon.Handle);

			// Exit the application.
			Application.Exit();
		}

		private void tempTimer_Tick(object sender, System.EventArgs e)
		{			
			// When the interation timer is up then update the temperature.
			this.GetTemp();
		}

		// Get a Win32 function to free the resources of an icon.
		[DllImport("user32.dll", EntryPoint="DestroyIcon")]
		static extern bool DestroyIcon(IntPtr hIcon);


		private void UpdateIcon(string text)
		{
			Bitmap tempImage;						
			Font font;
			SizeF size;

			// Make a new bitmap the size of the notify icon.
			tempImage = new Bitmap(16, 16);

			// Get a graphics device from the image.
			Graphics g = Graphics.FromImage(tempImage);						
			
			font = new Font("Arial Narrow", 8);

			size = g.MeasureString(text, font);

			// Draw the font with the correct size and location on the icon.
			g.DrawString(text, font, Brushes.Black, (16 / 2) - (size.Width / 2), (16 / 2) - (size.Height / 2));
			
			// Handle the memory from the previous icon.
			DestroyIcon(tray.Icon.Handle);
			tray.Icon.Dispose();			
			
			// Set the tray icon to the new icon by converting it to an icon from a bitmap.
			tray.Icon = Icon.FromHandle(tempImage.GetHicon());			

			// Dispose the resources used draw the icon.
			g.Dispose();
			tempImage.Dispose();						
		}

		/// <summary>
		/// Gets the temperature and updates the tray icon, the tray text, and displays when appropriate.
		/// </summary>
		private void GetTemp()
		{						
			// Keep up with the last temperature.
			this._lastTemp = this._temp;

			try
			{
				// Try to use the webservice to get the temperature.
				this._temp = service.getTemp(this._zipCode.ToString());

				// Clear the error flag.
				this._wasError = false;
			}
			catch (WebException webException)
			{
				// If an exception is thrown then make the icon and text question marks.
				this.UpdateIcon("??");
				this.tray.Text = "??";
				
				// Then display a message and set the error flag if the option is enabled.
				if (this._showErrors == true)
				{					
					MessageBox.Show(this, webException.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

					this._wasError = true;
				}
			}
			catch (SocketException socketException)
			{	
				// If an exception is thrown then make the icon and text question marks.
				this.UpdateIcon("??");
				this.tray.Text = "??";

				// Then display a message and set the error flag if the option is enabled.
				if (this._showErrors == true)
				{
					MessageBox.Show(this, socketException.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

					this._wasError = true;
				}
			}
			catch (Exception exception)
			{
				// If an exception is thrown then make the icon and text question marks.
				this.UpdateIcon("??");
				this.tray.Text = "??";

				// Then display a message and set the error flag if the option is enabled.
				if (this._showErrors == true)
				{
					MessageBox.Show(this, exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

					this._wasError = true;
				}
			}
			
			// If the temperature is the same as the last time it checked, no need to update the icon.
			if (_temp != _lastTemp)
			{				
				// Change the icon to the new temperature.
				this.UpdateIcon(_temp.ToString() + "°");

				// Change the tray text to the new temperature.
				this.tray.Text = _temp.ToString() + "°";
			}								
		}

		private void aboutMenuItem_Click(object sender, System.EventArgs e)
		{
			// If the about menu item is clicked then show a message dialog with the about information.
			string message = Application.ProductName + " " + Application.ProductVersion + "\r\n©2004 " + Application.CompanyName + ". All Rights Reserved.";

			MessageBox.Show(this, message, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void optionsMenuItem_Click(object sender, System.EventArgs e)
		{
			// If the options menu item in the notify icon's context menu is clicked
			// then show the options dialog.
			this.FromTray();
		}

		/// <summary>
		/// Hides the options window.
		/// </summary>
		private void ToTray()
		{			
			// When the options dialog is sent to the tray hide the dialog.
			this.Hide();			
		}

		/// <summary>
		/// Unhides the options window.
		/// </summary>
		private void FromTray()
		{							
			// When the options dialog opened from the tray make it visible.
			this.Show();
			// Also focus it to bring it to the front.
			this.Focus();
		}

		private void TempForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// If the option dialog is closed cancel it.
			e.Cancel = true;

			// Then send it to the system tray.
			this.ToTray();
		}

		/// <summary>
		/// Updates the dialog with the current option values.
		/// </summary>
		private void UpdateDialog()
		{
			this.zipTextBox.Text = this._zipCode.ToString();
			this.intervalTextBox.Text = this._interval.ToString();
			this.startupCheckBox.Checked = this._runAtStartup;
			this.errorsCheckBox.Checked = this._showErrors;
		}

		/// <summary>
		/// Checks if the options are valid.
		/// </summary>
		/// <returns>If the options are valid.</returns>
		private bool ValidateOptions()
		{
			int zip, interval;

			// If the zip code text box is empty then display an error message.
			if (this.zipTextBox.Text == "")
			{
				MessageBox.Show(this, "You must enter a zipcode.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			// If the number is too small or not a number then display the appropriate error message.
			try
			{
				zip = Int32.Parse(this.zipTextBox.Text);
			}
			catch (FormatException)
			{
				MessageBox.Show(this, "Zipcode must be a integer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
			catch (OverflowException)
			{
				MessageBox.Show(this, "That value is too large or too small.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
		
			// If the number is definatly not a zip code then display an error.
			if ((zip < 10000) || (zip >= 100000))
			{
				MessageBox.Show(this, "Invalid value for a zip code.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			// If the interval text box is empty then display an error.
			if (this.intervalTextBox.Text == "")
			{
				MessageBox.Show(this, "You must enter an interval.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			// If the interval is not a number or is too small or too large then display an error.
			try
			{
				interval = Int32.Parse(this.intervalTextBox.Text);
			}
			catch (FormatException)
			{
				MessageBox.Show(this, "The interval must be a integer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
			catch (OverflowException)
			{
				MessageBox.Show(this, "That value is too large or too small.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
		
			// If the interval is less than zero it is definatly not a valid interval so an error will be displayed.
			if (interval < 0)
			{
				MessageBox.Show(this, "Invalid value for the interval.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			// Update the interval with the new valid value.
			this._interval = interval;
			// Set the timer interval to the new value.
			this.tempTimer.Interval = this._interval * 60000;

			// Set the zipcode, run at startup, and show errors settings to the new valid values.
			this._zipCode = zip;			
			this._runAtStartup = this.startupCheckBox.Checked;
			this._showErrors = this.errorsCheckBox.Checked;
			
			// Make it so you can't use the apply button again until settings are changed.
			this.applyButton.Enabled = false;			

			// Return true indicating the values are valid.
			return true;
		}

		private void OptionsChanged()
		{			
			// Make it so you can use the apply button to save the settings.
			this.applyButton.Enabled = true;
		}		
		
		/// <summary>
		/// Loads the options from the registry.
		/// </summary>
		private void LoadOptions()
		{
			object runAtStartup, showErrors;
			RegistryKey tempKey = Application.CommonAppDataRegistry;
			
			runAtStartup = tempKey.GetValue("LoadOnStartUp");			
			showErrors = tempKey.GetValue("ShowErrors");

			if (runAtStartup != null)
				this._runAtStartup = (bool)runAtStartup;
			if (showErrors != null)
				this._showErrors = (bool)showErrors;

			tempKey.Close();
		}

		/// <summary>
		/// Saves the options to the registry.
		/// </summary>
		private void SaveOptions()
		{
			RegistryKey tempKey = Application.CommonAppDataRegistry;

			tempKey.SetValue("LoadOnStartUp", this._runAtStartup);
			tempKey.SetValue("ShowErrors", this._showErrors);

			tempKey.Close();
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			// If the OK button is clicked validate the options.
			if (this.ValidateOptions() == true)
			{
				// If the options were valid send it to the system tray.
				this.ToTray();

				// Save the newly changed options.
				this.SaveOptions();

				// Get the temp because the zip code might have changed.
				this.GetTemp();
			}
		}

		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			// When the cancel button is clicked send it to the system tray.
			this.ToTray();
			// Restore the previous settings.
			this.UpdateDialog();
		}

		private void applyButton_Click(object sender, System.EventArgs e)
		{
			// When the apply button is clicked validate all the options.
			this.ValidateOptions();			

			// Save the newly changed options.
			this.SaveOptions();

			// Update the temperature if the zipcode was changed.
			this.GetTemp();
		}

		private void zipTextBox_TextChanged(object sender, System.EventArgs e)
		{
			// When then zip code is changed handle the options being changed.
			this.OptionsChanged();
		}

		private void intervalTextBox_TextChanged(object sender, System.EventArgs e)
		{
			// When the interval is changed handle the options being changed.
			this.OptionsChanged();
		}

		private void startupCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			// When the startup check box handle the options being changed.
			this.OptionsChanged();					
		}

		private void errorsCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			// When the error check box handle the options being changed.
			this.OptionsChanged();
		}

		private void tray_DoubleClick(object sender, System.EventArgs e)
		{
			// When the notifity icon is double clicked show the options dialog.
			this.FromTray();
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
		}
	}
}
