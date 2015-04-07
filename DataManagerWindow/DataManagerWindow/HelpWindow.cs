// Filename: HelpWindow.cs
// Author: Arun Rai - Virginia Tech
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DataManagerWindow
{
    public partial class HelpWindow : Form
    {
        public HelpWindow(int tagPage)
        {
            InitializeComponent();
            this.Text = "Help";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.HelpText.ReadOnly = true;
            this.HelpText.Font = new System.Drawing.Font("Arial", 8F, FontStyle.Regular);
            DisplayHelpText(tagPage);
        }

        //----------------------------------------------------------------------------------
        // Function name: private void DisplayHelpText(int tagPage)
        // Description: Display appropriate help instruction for a particular task/operation.
        //----------------------------------------------------------------------------------
        private void DisplayHelpText(int tagPage)
        {
            if (tagPage == 1)
            {
                this.HelpText.AppendText("- Available Tag Numbers are checked marked.");
                this.HelpText.AppendText(" Uncheck the Tag Number if data fields need not be changed.\r\n");
                this.HelpText.AppendText("\r\n");
                this.HelpText.AppendText("- To change data fields, select the Tag Number(s) and change values in the fields.\r\n");
                this.HelpText.AppendText("\r\n");
                this.HelpText.AppendText("- For each Tag Number, Barcode value must be 6-7 characters long.");
                this.HelpText.AppendText(" A Barcode with less than 6 characters will be padded with leading spaces.");                                   
                this.HelpText.AppendText(" The characters must include upper case letters and digits only.\r\n");
                this.HelpText.AppendText("\r\n");
                this.HelpText.AppendText("- For each Tag Number, DSR value must have 5 or less characters.");
                this.HelpText.AppendText(" The characters must include upper case letters and digits only.\r\n");
                this.HelpText.AppendText("\r\n");
                this.HelpText.AppendText("- For each Tag Number, Catalog Number must be 8 or more characters long.");
                this.HelpText.AppendText(" The characters must include upper case letters and digits only.\r\n");
                this.HelpText.AppendText("\r\n");
                this.HelpText.AppendText("- Click OK to continue or Cancel to cancel.");
                
            }
            else if (tagPage == 2)
            {
                this.HelpText.AppendText("Addr - Address of the first byte of data. \r\n");
                this.HelpText.AppendText("\r\n");
                this.HelpText.AppendText("Length - Length of data. \r\n");
                this.HelpText.AppendText("\r\n");
                this.HelpText.AppendText("Record Text - List of data in the memory. \r\n");
                this.HelpText.AppendText("\r\n");
                this.HelpText.AppendText("CRC - Cyclic Redundency Checksum of the data. \r\n");
                this.HelpText.AppendText("\r\n");
                this.HelpText.AppendText("Unavailalbe tag(s) is/are disableed. \r \n");
            }
            else if (tagPage == 3)
            {
                this.HelpText.AppendText("- Select the Tag Number to display EPROM contents.\r\n");
                // this.HelpText.AppendText("\r\n");
                this.HelpText.AppendText("\r\n");
                this.HelpText.AppendText("- There are 128 different addresses, and each address can store.");
                this.HelpText.AppendText(" one byte of data.\r\n");
                this.HelpText.AppendText("\r\n");
                this.HelpText.AppendText("- The memory addresses contain FF before programming. \r\n");
                this.HelpText.AppendText("\r\n");
                this.HelpText.AppendText("Unavailalbe tag(s) is/are disableed. \r \n");
            }
        }

        // Do not delete the functions below.
        // The program may give error or the UI designer form may not open if
        // any of the functions below is deleted.
        private void HelpWindow_Load(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void HelpText_TextChanged(object sender, EventArgs e)
        {
            // Not implemented
        }
    }
}
