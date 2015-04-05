
//***********************************************************************************************************||
//-----------------------------------------------------------------------------------------------------------||
// Project name            : Senior Capstone Design
// Project term            : Fall 2014 - Spring 2015
// Title                   : Simultaneous Programming of Multiple ID Tags
// Sponsor                 : General Electric
// Customer name           : Mr. Michael Austin

// Instructor              : Prof. Gino manzo
// Subject Matter Expert   : Dr. William Plymale
// Team name               : BridgeBuilders 
// Members                 : Arun Rai, Danny Mota, Mohammad Islam, and Xin Gan
// Author                  : Arun Rai
// Date                    : 02/20/2015
// Reviewed by             : 
// Description: This software allows a user to read and program multiple DS2502 (aka ID Tags), upto 6 devices, 
//              through a Graphical User Interface (GUI).
//-----------------------------------------------------------------------------------------------------------||
//***********************************************************************************************************||

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DataManagerWindow
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
