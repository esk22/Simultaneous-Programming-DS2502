
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
// Description: This software allows a user to read and program multiple DS2502 (aka ID Tags), upto 6 devices, 
//              through a Graphical User Interface (GUI).
// Filename - MainWindow.cs
//-----------------------------------------------------------------------------------------------------------||
//***********************************************************************************************************||

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.Threading;
using System.Text.RegularExpressions;

namespace DataManagerWindow
{
    public partial class MainWindow : Form
    {
        private static bool PortDetected;
        private static bool OpenDataView; // DataViewWindow
        private static bool PortOpen;
        private static string PortName;
        private static string[] HexDataStr;
        private static int ProgressMax;
        DataInManager BytesIn;

        public MainWindow()
        {
            // Initialize components - auto generated function
           InitializeComponent();
           // Initialize other components
           InitOtherComponents();
           InitPortSelectionList();
           // Auto detect the Serial Port
           PortAutoDetect(serialPort1);
           DisplayPortNo();
           // Set the Window Title
           this.Text = "1-Wire Net Port Selection";
        }

        // Components intilization
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Port_Selection = new System.Windows.Forms.GroupBox();
            this.PortSelectionList = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Driver_Info = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.OK_botton = new System.Windows.Forms.Button();
            this.Cancel_botton = new System.Windows.Forms.Button();
            this.Auto_botton = new System.Windows.Forms.Button();
            this.UserInstructionText = new System.Windows.Forms.Label();
            this.Adapter_Type = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.OS_Compatability = new System.Windows.Forms.Label();
            this.OneWireDevice = new System.Windows.Forms.Label();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.Progress = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Port_Selection.SuspendLayout();
            this.Driver_Info.SuspendLayout();
            this.Adapter_Type.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // Port_Selection
            // 
            this.Port_Selection.Controls.Add(this.PortSelectionList);
            this.Port_Selection.Controls.Add(this.label7);
            this.Port_Selection.Controls.Add(this.label3);
            this.Port_Selection.Controls.Add(this.label1);
            this.Port_Selection.Font = new System.Drawing.Font("Arial", 11F);
            this.Port_Selection.Location = new System.Drawing.Point(221, 13);
            this.Port_Selection.Name = "Port_Selection";
            this.Port_Selection.Size = new System.Drawing.Size(320, 105);
            this.Port_Selection.TabIndex = 0;
            this.Port_Selection.TabStop = false;
            this.Port_Selection.Text = "Port Selection";
            this.Port_Selection.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // PortSelectionList
            // 
            this.PortSelectionList.FormattingEnabled = true;
            this.PortSelectionList.Location = new System.Drawing.Point(169, 64);
            this.PortSelectionList.Name = "PortSelectionList";
            this.PortSelectionList.Size = new System.Drawing.Size(80, 25);
            this.PortSelectionList.TabIndex = 4;
            this.PortSelectionList.SelectedIndexChanged += new System.EventHandler(this.PortNumberDropList_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 10F);
            this.label7.Location = new System.Drawing.Point(184, 37);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(40, 16);
            this.label7.TabIndex = 3;
            this.label7.Text = "COM";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(31, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "USB Port:";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(31, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "PC Port Type:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // Driver_Info
            // 
            this.Driver_Info.Controls.Add(this.label10);
            this.Driver_Info.Controls.Add(this.label9);
            this.Driver_Info.Controls.Add(this.label5);
            this.Driver_Info.Controls.Add(this.label4);
            this.Driver_Info.Font = new System.Drawing.Font("Arial", 11F);
            this.Driver_Info.Location = new System.Drawing.Point(221, 128);
            this.Driver_Info.Name = "Driver_Info";
            this.Driver_Info.Size = new System.Drawing.Size(320, 102);
            this.Driver_Info.TabIndex = 1;
            this.Driver_Info.TabStop = false;
            this.Driver_Info.Text = "Driver Info";
            this.Driver_Info.Enter += new System.EventHandler(this.Driver_Info_Enter);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(182, 62);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(80, 17);
            this.label10.TabIndex = 4;
            this.label10.Text = "05/06/2015";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(183, 33);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 17);
            this.label9.TabIndex = 3;
            this.label9.Text = "V1.0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(32, 62);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(105, 16);
            this.label5.TabIndex = 1;
            this.label5.Text = "Release Date:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(32, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "Version:";
            // 
            // OK_botton
            // 
            this.OK_botton.Font = new System.Drawing.Font("Arial", 10F);
            this.OK_botton.Location = new System.Drawing.Point(312, 383);
            this.OK_botton.Name = "OK_botton";
            this.OK_botton.Size = new System.Drawing.Size(87, 32);
            this.OK_botton.TabIndex = 2;
            this.OK_botton.Text = "OK";
            this.OK_botton.UseVisualStyleBackColor = true;
            this.OK_botton.Click += new System.EventHandler(this.button1_Click);
            // 
            // Cancel_botton
            // 
            this.Cancel_botton.Font = new System.Drawing.Font("Arial", 10F);
            this.Cancel_botton.Location = new System.Drawing.Point(439, 383);
            this.Cancel_botton.Name = "Cancel_botton";
            this.Cancel_botton.Size = new System.Drawing.Size(86, 32);
            this.Cancel_botton.TabIndex = 3;
            this.Cancel_botton.Text = "Cancel";
            this.Cancel_botton.UseVisualStyleBackColor = true;
            this.Cancel_botton.Click += new System.EventHandler(this.Cancel_botton_Click);
            // 
            // Auto_botton
            // 
            this.Auto_botton.Font = new System.Drawing.Font("Arial", 10F);
            this.Auto_botton.Location = new System.Drawing.Point(24, 382);
            this.Auto_botton.Name = "Auto_botton";
            this.Auto_botton.Size = new System.Drawing.Size(97, 32);
            this.Auto_botton.TabIndex = 4;
            this.Auto_botton.Text = "Auto-Detect";
            this.Auto_botton.UseVisualStyleBackColor = true;
            this.Auto_botton.Click += new System.EventHandler(this.Auto_botton_Click);
            // 
            // UserInstructionText
            // 
            this.UserInstructionText.AutoSize = true;
            this.UserInstructionText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F);
            this.UserInstructionText.Location = new System.Drawing.Point(21, 268);
            this.UserInstructionText.Name = "UserInstructionText";
            this.UserInstructionText.Size = new System.Drawing.Size(143, 96);
            this.UserInstructionText.TabIndex = 5;
            this.UserInstructionText.Text = "Select the appropriate \r\nport number from the port list. \r\nWhen finished click \'OK\'. OR\r\nSelect Auto" +
    "-Detect \r\nbelow.\r\n\r\n";
            this.UserInstructionText.Click += new System.EventHandler(this.label12_Click);
            // 
            // Adapter_Type
            // 
            this.Adapter_Type.Controls.Add(this.label6);
            this.Adapter_Type.Controls.Add(this.label2);
            this.Adapter_Type.Controls.Add(this.OS_Compatability);
            this.Adapter_Type.Controls.Add(this.OneWireDevice);
            this.Adapter_Type.Font = new System.Drawing.Font("Arial", 11F);
            this.Adapter_Type.Location = new System.Drawing.Point(221, 241);
            this.Adapter_Type.Name = "Adapter_Type";
            this.Adapter_Type.Size = new System.Drawing.Size(320, 102);
            this.Adapter_Type.TabIndex = 7;
            this.Adapter_Type.TabStop = false;
            this.Adapter_Type.Text = "Misc Info";
            this.Adapter_Type.Enter += new System.EventHandler(this.Adapter_Type_Enter);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(184, 57);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(117, 17);
            this.label6.TabIndex = 1;
            this.label6.Text = "Windows 7/8/8.1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(184, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "DS2502";
            // 
            // OS_Compatability
            // 
            this.OS_Compatability.AutoSize = true;
            this.OS_Compatability.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.OS_Compatability.Location = new System.Drawing.Point(31, 57);
            this.OS_Compatability.Name = "OS_Compatability";
            this.OS_Compatability.Size = new System.Drawing.Size(126, 16);
            this.OS_Compatability.TabIndex = 0;
            this.OS_Compatability.Text = "OS Compatibility:";
            this.OS_Compatability.Click += new System.EventHandler(this.OS_Compatability_Click);
            // 
            // OneWireDevice
            // 
            this.OneWireDevice.AutoSize = true;
            this.OneWireDevice.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.OneWireDevice.Location = new System.Drawing.Point(31, 31);
            this.OneWireDevice.Name = "OneWireDevice";
            this.OneWireDevice.Size = new System.Drawing.Size(110, 16);
            this.OneWireDevice.TabIndex = 0;
            this.OneWireDevice.Text = "1-Wire Device:";
            this.OneWireDevice.Click += new System.EventHandler(this.OneWireDevice_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(15, 357);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(526, 17);
            this.progressBar1.TabIndex = 9;
            this.progressBar1.Visible = false;
            // 
            // Progress
            // 
            this.Progress.AutoSize = true;
            this.Progress.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.Progress.Location = new System.Drawing.Point(218, 391);
            this.Progress.Name = "Progress";
            this.Progress.Size = new System.Drawing.Size(0, 15);
            this.Progress.TabIndex = 10;
            this.Progress.Visible = false;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::DataManagerWindow.Properties.Resources.Icon_one_wire1;
            this.pictureBox1.Location = new System.Drawing.Point(6, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(207, 249);
            this.pictureBox1.TabIndex = 11;
            this.pictureBox1.TabStop = false;
            // 
            // MainWindow
            // 
            this.ClientSize = new System.Drawing.Size(562, 432);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Progress);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.Adapter_Type);
            this.Controls.Add(this.UserInstructionText);
            this.Controls.Add(this.Auto_botton);
            this.Controls.Add(this.Cancel_botton);
            this.Controls.Add(this.OK_botton);
            this.Controls.Add(this.Driver_Info);
            this.Controls.Add(this.Port_Selection);
            this.Name = "MainWindow";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Port_Selection.ResumeLayout(false);
            this.Port_Selection.PerformLayout();
            this.Driver_Info.ResumeLayout(false);
            this.Driver_Info.PerformLayout();
            this.Adapter_Type.ResumeLayout(false);
            this.Adapter_Type.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        //----------------------------------------------------------------------------------
        // Function name: private void InitOtherComponents()
        // Description: Intialize other components (includes customization and modification of windows)
        //----------------------------------------------------------------------------------
        private void InitOtherComponents()
        {
            PortDetected = false;
            OpenDataView = false;
            PortOpen = false;
            ProgressMax = 1900;
            PortName = "PORT";
            BytesIn = new DataInManager();
            HexDataStr = new string[6];
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        //----------------------------------------------------------------------------------
        // Function name: private static void PortAutoDetect(SerialPort serialPort1)
        // Description: Detect the USB port that the Hardware device is connected to.
        //----------------------------------------------------------------------------------
        private void PortAutoDetect(SerialPort serialPort1)
        {
            // port auto detection  ---- 
            string[] ports = SerialPort.GetPortNames();
            // InitPortSelectionList();
            for (int i = 0; i < ports.Length; i++)
            {
                string port = ports[i];
                serialPort1 = new SerialPort(port);
                serialPortInit(serialPort1);
                if (!serialPort1.IsOpen)
                {
                    if (SerialResponse(serialPort1) == true)
                    {
                        PortDetected = true;
                        PortName = port;
                        AppendPortToSelectionList(PortName);
                        break;
                    }
                }
                serialPort1.Close();
                // Delay before going back to check the next port
                Thread.Sleep(50);
            }
            if (ports.Contains(PortName) == false)
            {
                PortSelectionList.Text = "  ";
                PortDetected = false;
            }
        }

        // Append PortName to the port selection list 
        private void AppendPortToSelectionList(string PortName)
        {
            bool PortExist = false;
            foreach (string port in PortSelectionList.Items)
            {
                if(port == PortName)
                {
                    PortExist = true;
                    break;
                }
            }
            // MessageBox.Show(c.ToString());
            if (!PortExist)
                PortSelectionList.Items.Add(PortName);
        }

        //----------------------------------------------------------------------------------
        // Function name: private void InitPortSelectionList()
        // Description: Intialization of the PORT names drop down list.
        //----------------------------------------------------------------------------------
        private void InitPortSelectionList()
        {
            string[] ports = SerialPort.GetPortNames();
            // Add port numbers to the list
            foreach (string port in ports)
                PortSelectionList.Items.Add(port);
        }

        //----------------------------------------------------------------------------------
        // Function name: privatprivate void Auto_botton_Click(object sender, EventArgs e)
        // Description: Auto detect the port the hardware device is connected to.
        //              Display the port number (COMX) in the drop down text field.
        //----------------------------------------------------------------------------------
        private void Auto_botton_Click(object sender, EventArgs e)
        {
            PortAutoDetect(serialPort1);
            string message;
            if (PortDetected)
            {
                message = "Hardware device detected. \nUSB Port - " + PortName;
                string caption = "OK";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
                // intialize the flag to true to allow the user to
                // navigate to next page -----
                OpenDataView = true;
                PortOpen = true;
                // Display the port number
                PortSelectionList.Text = PortName;
            }
            else
            {
                message = "No Hardware device is detected!";
                string caption = "Error";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //----------------------------------------------------------------------------------
        // Function name: private void DisplayPortNo()
        // Description: Display the Port number on the text box.
        //----------------------------------------------------------------------------------
        private void DisplayPortNo()
        {
            if (PortDetected)
            {
                OpenDataView = true;
                PortOpen = true;
                // Display the port number
                PortSelectionList.Text = PortName;
                PortSelectionList.Select(0, 0);
                // Enable timer
                timer1.Enabled = true;
            }
        }

        //----------------------------------------------------------------------------------
        // Function name: private void timer1_Tick(object sender, EventArgs e)
        //                Timer tick function used to remove hightlight from the
        //                port drop down text box.
        //----------------------------------------------------------------------------------
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Set port selection to zero
            // thus remove the highlight
            PortSelectionList.Select(0, 0);
            // Disable timer
            timer1.Enabled = false;
        }

        //----------------------------------------------------------------------------------
        // Function name: private static bool SerialResponse(SerialPort serialPort1)
        // Description: Send specific bytes to the Hardware device and check to see if desired 
        //              response is received from the Hardware device via the serial port.
        //----------------------------------------------------------------------------------
        private static bool SerialResponse(SerialPort serialPort1)
        {
            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
                // write some bytes ("port") to the serial port
                // connected to the hardware device
                Thread.Sleep(1);
                serialPort1.Write("z");
                // read bytes from the serial port
                int counter = 1;
                while (true)
                {
                    string response = serialPort1.ReadExisting();
                    // MessageBox.Show(response);
                    if (response.Length >= 3)
                    {
                        // This is the response sent by the hardware device
                        // -- The response bytes are written to the serial port
                        //    which then are read from by the software
                        if (response.Contains("ack"))
                        {
                            // The hardware device is detected
                            serialPort1.Close();
                            return true;
                        }
                    }
                    Thread.Sleep(1);
                    counter++;
                    if (counter == 400)
                    {
                        serialPort1.Close();
                        return false;
                    }
                }
            }
            return false;
        }

        //----------------------------------------------------------------------------------
        // Function name: private static void serialPortInit(SerialPort serialPort1)
        // Description: Intialization of the serial port.
        //----------------------------------------------------------------------------------
        private static void serialPortInit(SerialPort serialPort1)
        {
            serialPort1.BaudRate = 115200;
            serialPort1.Parity = Parity.None;
            serialPort1.StopBits = StopBits.One;
            serialPort1.DataBits = 8;
            serialPort1.Handshake = Handshake.None;
            //serialPort1.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }
        
        //----------------------------------------------------------------------------------
        // Function name: private string DataRequest(SerialPort serialPort1);
        // Description: Request for data from the ID tag
        //----------------------------------------------------------------------------------
        private void DataRequest(SerialPort serialPort1)
        {
            serialPort1 = new SerialPort(PortName);
            string data = "";
            serialPortInit(serialPort1);
            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
                // Wait for a millisecond
                Thread.Sleep(1);
                // write some bytes ("port") to the serial port
                // connected to the hardware device
                serialPort1.Write("r");
                while (true)
                {
                    string response = serialPort1.ReadExisting();
                   // MessageBox.Show(response);
                    if (response.Length >= 1)
                    {
                        data = data + "  " + response;
                        this.progressBar1.Value = data.Length;
                        this.Progress.Text = ((progressBar1.Value/ProgressMax)*100).ToString() + " %";
                        if (response.Contains("finished"))
                        {
                            // SerialStrToBytes(data);
                            BytesIn.ParseSerialString(data, ref HexDataStr);
                            // MessageBox.Show(data);
                            serialPort1.Close();
                            break;
                        }
                    }
                   Thread.Sleep(1);
                }
            }
            return;
        }

        // Initialize data buffer
        private void InitHexDataStr()
        {
            for (int i = 0; i < 6; i++)
                HexDataStr[i] = "";
        }

        //----------------------------------------------------------------------------------
        // Function name: private void button1_Click(object sender, EventArgs e)
        // Description: Verify that the Hardware device is detected. If detected, 
        // call function to go to next window
        //----------------------------------------------------------------------------------
        private void button1_Click(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            // InitPortSelectionList();
            if (ports.Length > 0)
            {
                InitHexDataStr(); // (The function is just above this function)
                string selected = this.PortSelectionList.GetItemText(this.PortSelectionList.SelectedItem);
                if (selected != PortName) PortAutoDetect(serialPort1);
                //MessageBox.Show(selected);
                if (selected == "")
                {
                    MessageBox.Show("No USB Port selected! \nPlease, select a USB Port \nOR click Auto-Detect.");
                }
                else
                {
                    // MessageBox.Show(selected);
                    // MessageBox.Show(PortName);
                    if (selected == PortName)
                    {
                        // To next window -- 
                        // All functions are implemented in the next window
                        OpenDataViewWindow(PortName);
                    }
                    else
                    {
                        if (OpenDataView && PortOpen && (selected == PortName))
                        {
                            // To next window -- 
                            // All functions are implemented in the next window
                            OpenDataViewWindow(PortName);
                        }
                        else
                        {
                            MessageBox.Show("Can not connect to hardware device!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                PortSelectionList.Text = "   ";
                PortSelectionList.Items.Remove(PortName);
                MessageBox.Show("Hardware device is disconnected !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //----------------------------------------------------------------------------------
        // Function name: private void OpenDataViewWindow(string PortName)
        // Description: Go to the next window 
        //----------------------------------------------------------------------------------
        private void OpenDataViewWindow(string PortName)
        {
            this.progressBar1.Visible = true;
            this.progressBar1.Maximum = ProgressMax;
            this.progressBar1.Minimum = 0;
            this.progressBar1.Value = 0;
            this.Progress.Text = progressBar1.Value.ToString() + " %"; ;
            this.progressBar1.ForeColor = Color.Blue;
            DataRequest(serialPort1);
            bool TagsExist = BytesIn.IsTagAvailable(HexDataStr);
            
            // At least one ID Tag exists 
            // and reading data finished.
            int max = 0;
            foreach (string element in HexDataStr)
                max += element.Length;
            this.progressBar1.Value = 1900;
            if (max > 1)
                this.Progress.Text = ((progressBar1.Value / max) * 100).ToString() + " %";
            this.Progress.Text = ProgressMax.ToString();
            //MessageBox.Show("Yes Message reading complete");
            if (TagsExist)
            {
                DataViewWindow display = new DataViewWindow(serialPort1, PortName, HexDataStr);
                this.progressBar1.Visible = false;
                this.progressBar1.Value = 0;
                this.Progress.Text = "";
                // open DataViewWindow
                display.ShowDialog(); 
            }
            else
            {
                this.progressBar1.Visible = false;
                this.progressBar1.Value = 0;
                this.Progress.Text = "";
                MessageBox.Show("NO ID Tag is connected to the Hardware.");
            }
        }

        // Cancels the action and/or closes the current window
        private void Cancel_botton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Abort selecting a default 1-Wire Net port?", "1-Wire Net Port Selection", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            if (result == DialogResult.Yes)
                this.Close(); 
        }

        // Do not delete the functions below.
        // The program may give error or the UI designer form may not open if
        // any of the functions below is deleted.
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Not implemented
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void label12_Click(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void Adapter1_CheckedChanged(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void Adapter2_CheckedChanged(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void Adapter3_CheckedChanged(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void PortNumberDropList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void label11_Click(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void label3_Click(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void Adapter_Type_Enter(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void OneWireDevice_Click(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void OS_Compatability_Click(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void Driver_Info_Enter(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void pictureBox1_Click_2(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void pictureBox1_Click_3(object sender, EventArgs e)
        {
            // Not implemented
        }
    }
}
