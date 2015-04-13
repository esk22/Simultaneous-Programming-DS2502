// 
// Filename: DataManager.cs
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
using System.IO.Ports;
using System.Threading;
using System.Text.RegularExpressions;

namespace DataManagerWindow
{
    public partial class DataManager : Form
    {
        // Current data list (after user makes changes)
        private static string[] UserChangedBarcode;
        private static string[] UserChangedDSR;
        private static string[] UserChangedCatalog;
        // Data list when data is loaded from the ID Chips
        private static string[] BarcodeBuffer;
        private static string[] DsrBuffer;
        private static string[] CatalogBuffer;
        // Other arrays
        private static bool[] CheckBoxBuffer;
        private static string[] HexList;
        private static string[] TagsDataBuffer;
        private static string[] DataRecord;
        private static string[] DataToWrite;
        private static int CursorPosition;
        private static string[] DataBuffer;
        private static int ProgressMax;
        private static int[] DataReplacedCount;
        private static int[] BytesRemained;
        private static string PORT; // Serial Port Name
        // Create objects
        BytesManager compute;
        ComputeCRC8 crc;
        // Main function
        public DataManager(SerialPort serialPort, string portName, string[] StrDataBytes)
        {
            InitializeComponent();
            InitLocalComponent();
            // Set the window title
            this.Text = "ID Tag Content";
            // ==== Serial port =============/ 
            serialPort = new SerialPort(portName);
            serialPort1 = serialPort;
            // ===== Serial Port name ========/
            PORT = portName;
            // Fill the ID Tag data buffer
            InitDataBuffer(StrDataBytes);
            // Read data contents from the appropriate text boxes
            // - Barcode, DSR, and Catalog Number
            ReadDataFields();
            // Init Hex data column bar
            HexDataCountBar();
            tabControl1.Selecting += new TabControlCancelEventHandler(tabControl1_Selecting);
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void InitLocalComponent()
        // Description: initialize the components
        //-----------------------------------------------------------------------------------------------------------
        private void InitLocalComponent()
        {
            this.RefreshButton.Font = new System.Drawing.Font("Arial", 10F);
            this.OkBotton.Font = new System.Drawing.Font("Arial", 10F);
            this.CancelBotton.Font = new System.Drawing.Font("Arial", 10F);
            this.HelpButton.Font = new System.Drawing.Font("Arial", 10F, FontStyle.Underline);
            this.Tag_Number.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Bold);
            this.Barcode.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Bold);
            this.DSR.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Bold);
            this.Catalog_Number.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Bold);
            this.label1.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Bold);     
            this.HexDataTextBox.ScrollBars = ScrollBars.Vertical;
            this.HexDataTextBox.ReadOnly = true;
            this.HexDataTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.DisplayHexRadioButtonList.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Bold);
            this.RecordSelectTagList.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Bold);
            // Customize barcode text fields
            this.BarcodeText1.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Regular);
            this.BarcodeText2.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Regular);
            this.BarcodeText3.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Regular);
            this.BarcodeText4.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Regular);
            this.BarcodeText5.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Regular);
            this.BarcodeText6.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Regular);
            // Customize DSR text fields
            this.DsrText1.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Regular);
            this.DsrText2.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Regular);
            this.DsrText3.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Regular);
            this.DsrText4.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Regular);
            this.DsrText5.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Regular);
            this.DsrText6.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Regular);
            // Customize Catalog number text fields
            this.CatalogText1.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Regular);
            this.CatalogText2.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Regular);
            this.CatalogText3.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Regular);
            this.CatalogText4.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Regular);
            this.CatalogText5.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Regular);
            this.CatalogText6.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.SerialNoLabel.Visible = false;
           
            // Readonly Text boxes
            this.RecordList.ReadOnly = true;
            this.RecordListCRC.ReadOnly = true;
            // Set color
            this.RecordList.BackColor = System.Drawing.SystemColors.Window;
            this.RecordListCRC.BackColor = System.Drawing.SystemColors.Window;
            // Column list initialization
            // For displaying data record list in the text boxs
            this.ColumnList.AppendText("    | Addr");
            this.ColumnList.AppendText("     ");
            this.ColumnList.AppendText(" | Length");
            this.ColumnList.AppendText("     ");
            this.ColumnList.AppendText(" | Record Text      ");
            this.ColumnList.AppendText("                                                ");
            this.BarCRC.AppendText(" | CRC");
            this.ColumnList.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Bold);
            this.BarCRC.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Bold);
            this.ColumnList.ReadOnly = true;
            this.BarCRC.ReadOnly = true;
            // this.RecordList.ScrollBars = ScrollBars.Vertical;

            // Instantiate objects
            compute = new BytesManager();
            crc = new ComputeCRC8();

            UserChangedBarcode = new string[6];
            UserChangedDSR = new string[6];
            UserChangedCatalog = new string[6];
            HexList = new string[128];
            BarcodeBuffer = new string[6];
            DsrBuffer = new string[6];
            CatalogBuffer = new string[6];
            CheckBoxBuffer = new bool[6];
            // Define the size of the array that copies and holds
            // the ID Tag data
            TagsDataBuffer = new string[6];
            // Init data record
            DataRecord = new string[3];
            BytesRemained = new int[6];
            for (int i = 0; i < 3; i++)
                DataRecord[i] = "a";
            for (int j = 0; j < 6; j++)
                BytesRemained[0] = 127;
            // Progressbar maximum
            ProgressMax = 1900;
            this.progressBar1.Visible = false;
            this.DataBytes.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Regular);
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void InitDataBuffer(string[] StrDataBytes)
        // Description: Fill the data buffer
        //-----------------------------------------------------------------------------------------------------------
        private void InitDataBuffer(string[] StrDataBytes)
        {
            DataFiledsEnable();
            bool DefaultText = false;
            // Fill data contents in the appropriate
            // text boxes - Barcode, DSR, and Catalog Number
            for (int i = 1; i <= StrDataBytes.Length; i++)
            {
                if (StrDataBytes[i - 1].Length > 1)
                {
                    TagsDataBuffer[i - 1] = compute.SpaceDelimitor(compute.RemoveExtraWords(StrDataBytes[i - 1]));
                    if (!DefaultText)
                    {
                        // MessageBox.Show(StrDataBytes[i - 1]);
                        DisplayHexDataTextBox(TagsDataBuffer[i - 1]);
                        compute.GetRecordList(TagsDataBuffer[i - 1], ref DataRecord);
                        DisplayDataRecord(DataRecord);
                        AutoChckRecordListTag(i);
                        AutoCheckHexListTag(i);
                        DefaultText = true;
                        this.SerialNoLabel.Visible = true;
                    }
                    ComputeDataFields(TagsDataBuffer[i - 1], i);
                }
                else
                {
                    TagsDataBuffer[i - 1] = "";
                    DisableEditDataTextBox(i);
                    // Disable tag select radio buttons
                    rTagSelectButtonDisable(i);
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------
        // function name: private void AutoChckRecordListTag(int tag)
        // Description: Auto check Tag list in the record data list
        //-----------------------------------------------------------------------------------------------------------
        private void AutoChckRecordListTag(int tag)
        {
            switch (tag)
            {
                case 1:
                    this.rSelectTag1.Checked = true;
                    break;
                case 2:
                    this.rSelectTag2.Checked = true;
                    break;
                case 3:
                    this.rSelectTag3.Checked = true;
                    break;
                case 4:
                    this.rSelectTag4.Checked = true;
                    break;
                case 5:
                    this.rSelectTag5.Checked = true;
                    break;
                case 6:
                    this.rSelectTag6.Checked = true;
                    break;
                default:
                    break;
            }
            return;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void AutoCheckHexListTag(int tag)
        // Description: Auto check the tag select button in the HexData tab page
        //-----------------------------------------------------------------------------------------------------------
        private void AutoCheckHexListTag(int tag)
        {
            switch (tag)
            {
                case 1:
                    this.sRecordTag1.Checked = true;
                    break;
                case 2:
                    this.sRecordTag2.Checked = true;
                    break;
                case 3:
                    this.sRecordTag3.Checked = true;
                    break;
                case 4:
                    this.sRecordTag4.Checked = true;
                    break;
                case 5:
                    this.sRecordTag5.Checked = true;
                    break;
                case 6:
                    this.sRecordTag6.Checked = true;
                    break;
                default:
                    break;
            }
            return;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void HexDataCountBar()
        // Description: Initialize and format the coloum list
        //              for displaying data bytes as hex values in the text box
        //-----------------------------------------------------------------------------------------------------------
        private void HexDataCountBar()
        {
            this.HexCountBar.Clear();
            this.HexCountBar.Font = new System.Drawing.Font("Arial", 9F, FontStyle.Bold);
            this.HexCountBar.AppendText(" ");
            this.HexCountBar.AppendText("  Addr");
            this.HexCountBar.AppendText("         ");
            for(int i = 0; i < 8; i++)
            {
                HexCountBar.AppendText("0x" + i.ToString("X2"));
                if (i == 1)
                    HexCountBar.AppendText("      ");
                else if (i == 2)
                    HexCountBar.AppendText("      ");
                else if (i == 3)
                    HexCountBar.AppendText("      ");
                else
                    HexCountBar.AppendText("     ");
            }
            this.HexCountBar.ReadOnly = true;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void rTagSelectButtonDisable()
        // Description: Disable the select button if the ID Tag does not exist.
        //-----------------------------------------------------------------------------------------------------------
        private void rTagSelectButtonDisable(int i)
        {
            if(i == 1)
            {
                this.sRecordTag1.Enabled = false;
                this.rSelectTag1.Enabled = false;
            }
            else if (i == 2)
            {
                this.sRecordTag2.Enabled = false;
                this.rSelectTag2.Enabled = false;
            }
            else if (i == 3)
            {
                this.sRecordTag3.Enabled = false;
                this.rSelectTag3.Enabled = false;
            }
            else if (i == 4)
            {
                this.sRecordTag4.Enabled = false;
                this.rSelectTag4.Enabled = false;
            }
            else if (i == 5)
            {
                this.sRecordTag5.Enabled = false;
                this.rSelectTag5.Enabled = false;
            }
            else if (i == 6)
            {
                this.sRecordTag6.Enabled = false;
                this.rSelectTag6.Enabled = false;
            }
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private static void serialPortInit(SerialPort serialPort1)
        // Description: Serial Port intialization
        //-----------------------------------------------------------------------------------------------------------
        private static void serialPortInit(SerialPort serialPort1)
        {
            serialPort1.BaudRate = 115200;
            serialPort1.Parity = Parity.None;
            serialPort1.StopBits = StopBits.One;
            serialPort1.DataBits = 8;
            serialPort1.Handshake = Handshake.None;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void ReadDataFields()
        // Description: Read data fields Barcode, DSR, and Catalog Number, and store
        //              the contents in temp buffers.
        //              The data placed in buffer will be used to check later if the user
        //              has changed the data fields. The buffer fields are filled with data when the
        //              application window opens for the first time.
        //-----------------------------------------------------------------------------------------------------------
        private void ReadDataFields()
        {
            // Read Barcode text boxes and store texts (data)
            // in the temp buffer
            BarcodeBuffer[0] = BarcodeText1.Text;
            BarcodeBuffer[1] = BarcodeText2.Text;
            BarcodeBuffer[2] = BarcodeText3.Text;
            BarcodeBuffer[3] = BarcodeText4.Text;
            BarcodeBuffer[4] = BarcodeText5.Text;
            BarcodeBuffer[5] = BarcodeText6.Text;

            // Read DSR text boxesand store texts (data)
            // in the temp buffer
            DsrBuffer[0] = DsrText1.Text;
            DsrBuffer[1] = DsrText2.Text;
            DsrBuffer[2] = DsrText3.Text;
            DsrBuffer[3] = DsrText4.Text;
            DsrBuffer[4] = DsrText5.Text;
            DsrBuffer[5] = DsrText6.Text;
                         
            // Read Catalog Number text boxesand store texts (data)
            // in the temp buffer
            CatalogBuffer[0] = CatalogText1.Text;
            CatalogBuffer[1] = CatalogText2.Text;
            CatalogBuffer[2] = CatalogText3.Text;
            CatalogBuffer[3] = CatalogText4.Text;
            CatalogBuffer[4] = CatalogText5.Text;
            CatalogBuffer[5] = CatalogText6.Text;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void ReadDataFieldsToValidate()
        // Description: Read and store data Barcode, DSR, and Catalog Number from the
        //              text fields. These data will be matched against the originally
        //              stored data in the buffers to check if the user has made 
        //              changes in the fields.
        //-----------------------------------------------------------------------------------------------------------
        private void ReadDataFieldsToValidate()
        {
            // Read and store the barcode values
            UserChangedBarcode[0] = BarcodeText1.Text;
            UserChangedBarcode[1] = BarcodeText2.Text;
            UserChangedBarcode[2] = BarcodeText3.Text;
            UserChangedBarcode[3] = BarcodeText4.Text;
            UserChangedBarcode[4] = BarcodeText5.Text;
            UserChangedBarcode[5] = BarcodeText6.Text;

            // Read and store DSR values
            UserChangedDSR[0] = DsrText1.Text;
            UserChangedDSR[1] = DsrText2.Text;
            UserChangedDSR[2] = DsrText3.Text;
            UserChangedDSR[3] = DsrText4.Text;
            UserChangedDSR[4] = DsrText5.Text;
            UserChangedDSR[5] = DsrText6.Text;

            // Read and store Catalog number values
            UserChangedCatalog[0] = CatalogText1.Text;
            UserChangedCatalog[1] = CatalogText2.Text;
            UserChangedCatalog[2] = CatalogText3.Text;
            UserChangedCatalog[3] = CatalogText4.Text;
            UserChangedCatalog[4] = CatalogText5.Text;
            UserChangedCatalog[5] = CatalogText6.Text;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void DisplayHexDataTextBox(string Data)
        // Description: Display the ID Tag EPROM contents as hex values in the text box
        //              Each data byte contains two hex values, and each data byte is
        //              stored in an EPROM address of the ID Tag. The text box displays 128
        //              data bytes in the text box.
        //-----------------------------------------------------------------------------------------------------------
        private void DisplayHexDataTextBox(string Data, int tag = -1)
        {
            int used = 0, remaining = 0;
            int HexCount = 0, ByteCount = 0, SerialHexCount = 0, LeftBytePosition = 0;
            string st = "", SerialNumber = "";
            string EightDataBytes = "   ";
            EightDataBytes += "0x00";
            EightDataBytes += "           ";
            for (int i = 0; i < Data.Length - 3; i++)
            {
                if (char.IsLetterOrDigit(Data[i]))
                {
                    SerialHexCount++;
                    if (SerialHexCount <= 16)
                    {
                        SerialNumber += Data[i];
                    }
                    else
                    {
                        HexCount++;
                        if (HexCount <= 2)
                        {
                            st += Data[i];
                            if (HexCount == 2)
                            {
                                if (st == "FF")
                                    remaining++;
                                else
                                    used++;
                                ByteCount++; HexCount = 0;
                                EightDataBytes += st; LeftBytePosition++;
                                EightDataBytes += "           "; st = "";
                                if (ByteCount == 8)
                                {
                                    HexDataTextBox.AppendText(EightDataBytes);
                                    this.HexDataTextBox.AppendText("\n");
                                    EightDataBytes = "   ";
                                    EightDataBytes += "0x" + LeftBytePosition.ToString("X2");
                                    EightDataBytes += "           ";
                                    ByteCount = 0;
                                }
                            }
                        }
                    }
                }
            }
            DisplaySerialNumber(SerialNumber);
            DisplayUsedRemainingBytes(used, remaining);           
        }

        // Display ID Chip Serial Number
        private void DisplaySerialNumber(string SerialNumber)
        {
            // Auto scroll up
            HexDataTextBox.SelectionStart = 0;
            HexDataTextBox.ScrollToCaret();
            // Display the ID Chip Serial Number
            this.SerialNoLabel.Text = "ID Chip Serial No.: " + SerialNumber;
        }

        // Display used and remaining bytes
        private void DisplayUsedRemainingBytes(int used, int remaining)
        {
            string usedStr = (used > 9) ? used.ToString() + " bytes used" : used.ToString() + " byte used";
            string remainStr = (remaining > 9) ? remaining.ToString() + " bytes remaining" : remaining.ToString() + " byte remaining";
            this.DataBytes.Text = "|      " + usedStr + " (" + remainStr + ")";
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name:  private void ParseStrToData(string Data)
        // Description: Parse data string to appropriate data bytes
        //              - Each data byte comes from a specific EPRM addresss
        //                of the ID Tag (Example: 09 is data byte)
        //-----------------------------------------------------------------------------------------------------------
        private void ParseStrToData(string Data, int tag = -1)
        {
            // MessageBox.Show(Data);
            int HexCount = 0, SerialHexCount = 0, BytePos = 0;
            int remaining = 0;
            string StrByte = "";
            for (int i = 0; i < Data.Length - 4; i++)
            {
                if (char.IsLetterOrDigit(Data[i]))
                {
                    SerialHexCount++;
                    if (SerialHexCount > 16)
                    {
                        HexCount++;
                        if (HexCount <= 2)
                        {
                            StrByte += Data[i];
                            if (HexCount == 2)
                            {
                                if (StrByte == "FF")
                                    remaining++;
                                if (BytePos < 128)
                                    HexList[BytePos] = StrByte;
                                BytePos++;
                                StrByte = "";
                                HexCount = 0;
                            }
                        }
                    }
                }
            }
            if (tag > -1)
                BytesRemained[tag] = remaining;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void ComputeDatalenPos(ref int barcodePos, ref int DsrPos, ref int CatalogPos,
        //                             ref int barCodeLen, ref int DsrLen, ref int CatalogLen, ref int crc)
        // Description: Compute the length and position of data in the EPROM
        //-----------------------------------------------------------------------------------------------------------
        private void ComputeDatalenPos(ref int barcodePos, ref int DsrPos, ref int CatalogPos,
                                        ref int barCodeLen, ref int DsrLen, ref int CatalogLen, ref int crc)
        {
            bool BarcodeDsrEnded = false, BarcodeFound = false, DsrPreceded = false;
            int crcPos = 0;
            for (int i = 0; i < 127; i++)
            {
                if (!BarcodeDsrEnded)
                {
                    if ((Convert.ToInt32(HexList[i], 16)) == 8 || (Convert.ToInt32(HexList[i], 16)) == 9)
                    {
                        if (!BarcodeFound && !DsrPreceded) // to be added 
                        {
                            barCodeLen = (Convert.ToInt32(HexList[i], 16));
                            barcodePos = i + 1;
                            crcPos = i + (Convert.ToInt32(HexList[i], 16)) - 1;
                            crc = Convert.ToInt32(HexList[crcPos], 16);
                            BarcodeFound = true;
                        }
                    }
                    else if ((Convert.ToInt32(HexList[i], 16)) >= 3 && (Convert.ToInt32(HexList[i], 16)) <= 7 && i > crcPos)
                    {
                        //  
                        if (Convert.ToInt32(HexList[i], 16) != crc)
                        {
                            DsrLen = (Convert.ToInt32(HexList[i], 16));
                            DsrPos = i + 1;
                            DsrPreceded = true;
                            crcPos = i + (Convert.ToInt32(HexList[i], 16)) - 1;
                            crc = Convert.ToInt32(HexList[crcPos], 16);
                        }
                        // (Convert.ToInt32(HexList[i + (Convert.ToInt32(HexList[i], 16)) - 1], 16));
                    }
                }

                if ((Convert.ToInt32(HexList[i], 16)) >= 10 && (Convert.ToInt32(HexList[i], 16)) <= 47 && i > crcPos)
                {
                    // MessageBox.Show(Convert.ToInt32(HexList[i], 16).ToString());
                    if (Convert.ToInt32(HexList[i], 16) != crc && HexList[i] != " ")
                    {
                        BarcodeDsrEnded = true;
                        CatalogLen = (Convert.ToInt32(HexList[i], 16));
                        CatalogPos = i + 1;
                        break;
                    }

                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void ComputeDataFields()
        // Description: - Call function to parse string to data values: one data byte
        //                contains two hex values that comes from an address of the 
        //                ID Tag EPROM ( Example: 09 is a data byte)
        //              - Identify and combine the data bytes to appropriate data types
        //                such as Barcode, DSR, and Catalog Number
        //-----------------------------------------------------------------------------------------------------------
        private void ComputeDataFields(string GetData, int tag)
        {
            ParseStrToData(GetData, tag - 1);
            int barcodePos = 0, DsrPos = 0, CatalogPos = 0;
            int barCodeLen = 0, DsrLen = 0, CatalogLen = 0;
            int crc = 0;
            ComputeDatalenPos(ref barcodePos, ref DsrPos, ref CatalogPos, ref barCodeLen, ref DsrLen, ref CatalogLen, ref crc);
            //MessageBox.Show(CatalogPos.ToString());
            string barcodeStr = "", DsrStr = "", CatalogStr = "";
            int barcodeCounter = 0, DsrCounter = 0, CatalogCounter = 0;
            for (int i = barcodePos; i < barcodePos + (barCodeLen - 2); i++)
            {
                barcodeStr += Convert.ToChar(Convert.ToInt32(HexList[i], 16)).ToString();
            }

            for (int j = DsrPos; j < DsrPos + (DsrLen - 2); j++)
            {
                DsrStr += Convert.ToChar(Convert.ToInt32(HexList[j], 16)).ToString();
            }

            if (CatalogPos > 2)
            {
                crc = CatalogPos + (Convert.ToInt32(HexList[CatalogPos - 1], 16)) - 2;
                for (int k = CatalogPos; k < HexList.Length - 1; k++)
                {
                    if (HexList[k + 1] != "FF" &&  (crc != k))
                    {
                        if (HexList[k] == "00")
                            CatalogStr += '*';
                        else if (Convert.ToInt32(HexList[k], 16) > 47) // && Convert.ToInt32(HexList[k], 16) < 91)
                            CatalogStr += Convert.ToChar(Convert.ToInt32(HexList[k], 16)).ToString();
                        else if (Convert.ToInt32(HexList[k], 16) > 2 && Convert.ToInt32(HexList[k], 16) < 47)
                                CatalogStr += '*';
                    }
                    else if (HexList[k + 1] == "FF")
                        break;
                }
            }
                
            UserChangedBarcode[barcodeCounter] = barcodeStr;
            UserChangedDSR[DsrCounter] = DsrStr;
            // MessageBox.Show(DsrStr);
            string DataReplaced = "";
            // MessageBox.Show(CatalogStr);
            UserChangedCatalog[CatalogCounter] = compute.GetCatalogNumber(CatalogStr, ref DataReplaced);
            ReplacedDataCountInCatalog(DataReplaced, tag);
            // Display data in the Edit Data text boxes
            DisplayEditDataTextBox(barcodeStr, DsrStr, UserChangedCatalog[CatalogCounter], tag);
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void ReplacedDataCountInCatalog(string str, int tag)
        // Description:
        // Count the data bytes replaced most recently at the endo of a Catalog number
        // Example: Previous Catalong number : IS200BJH234BBH
        // Recent catalog number: IS200BJH234AAB
        // In this case, the most recent catalog number can be obtained simply
        // by replacing the last three bytes in the previous catalog number.
        // In the EPROM, the last three bytes ( five bytes with data lenth and the CRC)
        // are added to the EPROM. While reading the software simply omits the last three bytes
        // (and the CRC) from the previously recorded catalog number.
        //-----------------------------------------------------------------------------------------------------------
        private void ReplacedDataCountInCatalog(string str, int tag)
        {
            DataReplacedCount = new int[6];
            switch (tag)
            {
                case 1:
                    DataReplacedCount[0] = str.Length;
                    break;
                case 2:
                    DataReplacedCount[1] = str.Length;
                    break;
                case 3:
                    DataReplacedCount[2] = str.Length;
                    break;
                case 4:
                    DataReplacedCount[3] = str.Length;
                    break;
                case 5:
                    DataReplacedCount[4] = str.Length;
                    break;
                case 6:
                    DataReplacedCount[5] = str.Length;
                    break;
                default:
                    break;
            }
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name:  private void Fill(string barcode, string dsr, string catalog, int tag)
        // Description: Fill the data fields - Barcode, DSR, and Catalog Number in the 
        //              appropriate text boxes.
        //-----------------------------------------------------------------------------------------------------------
        private void DisplayEditDataTextBox(string barcode, string dsr, string catalog, int tag)
        {
            if (tag == 1)
            {
                this.SelectTag1.Checked = true;
                this.BarcodeText1.Clear();
                this.BarcodeText1.Text = barcode;
                this.BarcodeText1.TextAlign = HorizontalAlignment.Left;
                this.BarcodeText1.SelectionStart = this.BarcodeText1.Text.Length;
                this.BarcodeText1.DeselectAll();

                this.DsrText1.Text = dsr;
                this.DsrText1.TextAlign = HorizontalAlignment.Left;
                this.DsrText1.SelectionStart = this.DsrText1.Text.Length;
                this.DsrText1.DeselectAll();

                this.CatalogText1.Text = catalog;
                this.CatalogText1.TextAlign = HorizontalAlignment.Left;
                this.CatalogText1.SelectionStart = this.CatalogText1.Text.Length;
                this.CatalogText1.DeselectAll();
            }
            else if (tag == 2)
            {
                this.SelectTag2.Checked = true;
                this.BarcodeText2.Clear();
                this.BarcodeText2.Text = barcode;
                this.BarcodeText2.TextAlign = HorizontalAlignment.Left;
                this.BarcodeText2.SelectionStart = this.BarcodeText2.Text.Length;
                this.BarcodeText2.DeselectAll();

                this.DsrText2.Text = dsr;
                this.DsrText2.TextAlign = HorizontalAlignment.Left;
                this.DsrText2.SelectionStart = this.DsrText2.Text.Length;
                this.DsrText2.DeselectAll();

                this.CatalogText2.Text = catalog;
                this.CatalogText2.TextAlign = HorizontalAlignment.Left;
                this.CatalogText2.SelectionStart = this.CatalogText2.Text.Length;
                this.CatalogText2.DeselectAll();
            }
            else if (tag == 3)
            {
                this.SelectTag3.Checked = true;
                this.BarcodeText3.Clear();
                this.BarcodeText3.Text = barcode;
                this.BarcodeText3.TextAlign = HorizontalAlignment.Left;
                this.BarcodeText3.SelectionStart = this.BarcodeText3.Text.Length;
                this.BarcodeText3.DeselectAll();

                this.DsrText3.Text = dsr;
                this.DsrText3.TextAlign = HorizontalAlignment.Left;
                this.DsrText3.SelectionStart = this.DsrText3.Text.Length;
                this.DsrText3.DeselectAll();

                this.CatalogText3.Text = catalog;
                this.CatalogText3.TextAlign = HorizontalAlignment.Left;
                this.CatalogText3.SelectionStart = this.CatalogText3.Text.Length;
                this.CatalogText3.DeselectAll();
            }
            else if (tag == 4)
            {
                this.SelectTag4.Checked = true;
                this.BarcodeText4.Clear();
                this.BarcodeText4.Text = barcode;
                this.BarcodeText4.TextAlign = HorizontalAlignment.Left;
                this.BarcodeText4.SelectionStart = this.BarcodeText4.Text.Length;
                this.BarcodeText4.DeselectAll();

                this.DsrText4.Text = dsr;
                this.DsrText4.TextAlign = HorizontalAlignment.Left;
                this.DsrText4.SelectionStart = this.DsrText4.Text.Length;
                this.DsrText4.DeselectAll();

                this.CatalogText4.Text = catalog;
                this.CatalogText4.TextAlign = HorizontalAlignment.Left;
                this.CatalogText4.SelectionStart = this.CatalogText4.Text.Length;
                this.CatalogText4.DeselectAll();
            }
            else if (tag == 5)
            {
                this.SelectTag5.Checked = true;
                this.BarcodeText5.Clear();
                this.BarcodeText5.Text = barcode;
                this.BarcodeText5.TextAlign = HorizontalAlignment.Left;
                this.BarcodeText5.SelectionStart = this.BarcodeText5.Text.Length;
                this.BarcodeText5.DeselectAll();

                this.DsrText5.Text = dsr;
                this.DsrText5.TextAlign = HorizontalAlignment.Left;
                this.DsrText5.SelectionStart = this.DsrText5.Text.Length;
                this.DsrText5.DeselectAll();

                this.CatalogText5.Text = catalog;
                this.CatalogText5.TextAlign = HorizontalAlignment.Left;
                this.CatalogText5.SelectionStart = this.CatalogText5.Text.Length;
                this.CatalogText5.DeselectAll();
            }
            else if (tag == 6)
            {
                this.SelectTag6.Checked = true;
                this.BarcodeText6.Clear();
                this.BarcodeText6.Text = barcode;
                this.BarcodeText6.TextAlign = HorizontalAlignment.Left;
                this.BarcodeText6.SelectionStart = this.BarcodeText6.Text.Length;
                this.BarcodeText6.DeselectAll();

                this.DsrText6.Text = dsr;
                this.DsrText6.TextAlign = HorizontalAlignment.Left;
                this.DsrText6.SelectionStart = this.DsrText6.Text.Length;
                this.DsrText6.DeselectAll();

                this.CatalogText6.Text = catalog;
                this.CatalogText6.TextAlign = HorizontalAlignment.Left;
                this.CatalogText6.SelectionStart = this.CatalogText6.Text.Length;
                this.CatalogText6.DeselectAll();
            }
        }

        // Text box control of the data fields
        private void DataFiledsEnable()
        {
            this.SelectTag1.Enabled = true;
            this.BarcodeText1.ReadOnly = false;
            this.DsrText1.ReadOnly = false;
            this.CatalogText1.ReadOnly = false;

            this.SelectTag2.Enabled = true;
            this.BarcodeText2.ReadOnly = false;
            this.DsrText2.ReadOnly = false;
            this.CatalogText2.ReadOnly = false;

            this.SelectTag3.Enabled = true;
            this.BarcodeText3.ReadOnly = false;
            this.DsrText3.ReadOnly = false;
            this.CatalogText3.ReadOnly = false;

            this.SelectTag4.Enabled = true;
            this.BarcodeText4.ReadOnly = false;
            this.DsrText4.ReadOnly = false;
            this.CatalogText4.ReadOnly = false;

            this.SelectTag5.Enabled = true;
            this.BarcodeText5.ReadOnly = false;
            this.DsrText5.ReadOnly = false;
            this.CatalogText5.ReadOnly = false;

            this.SelectTag6.Enabled = true;
            this.BarcodeText6.ReadOnly = false;
            this.DsrText6.ReadOnly = false;
            this.CatalogText6.ReadOnly = false;
            
            this.BarcodeText6.BackColor = System.Drawing.SystemColors.Window;
            this.DsrText6.BackColor = System.Drawing.SystemColors.Window;
            this.CatalogText6.BackColor = System.Drawing.SystemColors.Window;
        }

        // Text box control of the data fields
        private void DisableEditDataTextBox(int tag)
        {
            if (tag == 1)
            {
                this.SelectTag1.Enabled = false;
                this.BarcodeText1.ReadOnly = true;
                this.DsrText1.ReadOnly = true;
                this.CatalogText1.ReadOnly = true;
                
                this.BarcodeText1.BackColor = System.Drawing.SystemColors.Window;
                this.DsrText1.BackColor = System.Drawing.SystemColors.Window;
                this.CatalogText1.BackColor = System.Drawing.SystemColors.Window;
            }
            else if (tag == 2)
            {
                this.SelectTag2.Enabled = false;
                this.BarcodeText2.ReadOnly = true;
                this.DsrText2.ReadOnly = true;
                this.CatalogText2.ReadOnly = true;
                
                this.BarcodeText2.BackColor = System.Drawing.SystemColors.Window;
                this.DsrText2.BackColor = System.Drawing.SystemColors.Window;
                this.CatalogText2.BackColor = System.Drawing.SystemColors.Window;
            }
            else if (tag == 3)
            {
                this.SelectTag3.Enabled = false;
                this.BarcodeText3.ReadOnly = true;
                this.DsrText3.ReadOnly = true;
                this.CatalogText3.ReadOnly = true;
                
                this.BarcodeText3.BackColor = System.Drawing.SystemColors.Window;
                this.DsrText3.BackColor = System.Drawing.SystemColors.Window;
                this.CatalogText3.BackColor = System.Drawing.SystemColors.Window;                 
            }
            else if (tag == 4)
            {
                this.SelectTag4.Enabled = false;
                this.BarcodeText4.ReadOnly = true;
                this.DsrText4.ReadOnly = true;
                this.CatalogText4.ReadOnly = true;
                
                this.BarcodeText4.BackColor = System.Drawing.SystemColors.Window;
                this.DsrText4.BackColor = System.Drawing.SystemColors.Window;
                this.CatalogText4.BackColor = System.Drawing.SystemColors.Window;                 
            }
            else if (tag == 5)
            {
                this.SelectTag5.Enabled = false;
                this.BarcodeText5.ReadOnly = true;
                this.DsrText5.ReadOnly = true;
                this.CatalogText5.ReadOnly = true;
                
                this.BarcodeText5.BackColor = System.Drawing.SystemColors.Window;
                this.DsrText5.BackColor = System.Drawing.SystemColors.Window;
                this.CatalogText5.BackColor = System.Drawing.SystemColors.Window;                 
            }
            else if (tag == 6)
            {
                this.SelectTag6.Enabled = false;
                this.BarcodeText6.ReadOnly = true;
                this.DsrText6.ReadOnly = true;
                this.CatalogText6.ReadOnly = true;
               
                this.BarcodeText6.BackColor = System.Drawing.SystemColors.Window;
                this.DsrText6.BackColor = System.Drawing.SystemColors.Window;
                this.CatalogText6.BackColor = System.Drawing.SystemColors.Window;                 
            }
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private bool PadBarcode(string barcode, string ErrString)
        // Description: Pad barcode with leading space if the user input barcode is 5 or less
        //              or less characters long. Return true or false.
        //-----------------------------------------------------------------------------------------------------------
        private bool PadBarcode(ref string barcode, string ErrString)
        {
            if (barcode.Length <= 5 && barcode.Length > 0)
            {
                string msg = "The 5 or less characters barcode will be padded with ";
                msg += (barcode.Length == 5) ? " a space. \n" : " spaces. \n";
                msg += "Do you want to continue?";
                DialogResult dialogue = MessageBox.Show(msg, ErrString, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogue == DialogResult.No)
                {
                    return false;
                }
                else
                {
                    for (int a = barcode.Length; a < 6; a++)
                        barcode = " " + barcode;
                }
            }
            else
            {
                MessageBox.Show(ErrString + "\nBarcode must be 6-7 characters long.");
                return false;
            }
            return true;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: bool CheckInputs(int i)
        // Description: Verify user input data -
        //              Return true if the data are valid, return false otherwise.
        //-----------------------------------------------------------------------------------------------------------
        private bool CheckInputs(int i)
        {
            string ErrString = "In Tag Number " + (i + 1).ToString() + " :";
            if ((UserChangedBarcode[i].Length > 0) && (BarcodeBuffer[i] != UserChangedBarcode[i]) &&
                (UserChangedBarcode[i].Length < 6 || UserChangedBarcode[i].Length > 7))
            {
                if (!PadBarcode(ref UserChangedBarcode[i], ErrString))
                    return false;
            }
            else if(!UserChangedBarcode[i].All(c => char.IsLetterOrDigit(c)))
            {
                if(UserChangedBarcode[i].Contains(" "))
                {
                    // The barcode is 5 or characters long and
                    // it is padded with a space(s). -- do nothing
                }
                else
                {
                    // This case is handled automatically - not required 
                    // Implemented to hanndle unknown or exceptional cases
                    MessageBox.Show(UserChangedBarcode[i] + " and " + UserChangedBarcode[i].Length.ToString());
                    MessageBox.Show(ErrString + "\nBarcode must contain upper case letters and digits only.");
                    return false;
                }
            }

            if ((UserChangedDSR[i].Length > 0) && (DsrBuffer[i] != UserChangedDSR[i]) && (UserChangedDSR[i].Length > 5))
            {
                MessageBox.Show(ErrString + "\nDSR value must contain 5 or less characters.");
                return false;
            }
            else if (!UserChangedDSR[i].All(c => char.IsLetterOrDigit(c)))
            {
                MessageBox.Show(ErrString + "\nDSR value must contain upper case letters and digits only.");
                return false;
            }
            
            if ((UserChangedCatalog[i].Length > 0) && (CatalogBuffer[i] != UserChangedCatalog[i]) && 
                (UserChangedCatalog[i].Length < 8))
            {
                MessageBox.Show(ErrString + "\nCatalog number must be 8 or more characters long.");
                return false;
            }
            else if (!UserChangedCatalog[i].All(c => char.IsLetterOrDigit(c)))
            {
                MessageBox.Show(ErrString + "\nCatalog number must contain capital letters and digits only.");
                return false;
            }
            return true;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: void CheckInputContents()
        // Description: Check text inputs for Barcode, D.S.R, and Catalog number
        //-----------------------------------------------------------------------------------------------------------
        private bool CheckInputContents()
        {
            #pragma warning disable
            for (int i = 0; i < 2; i++)
            {
                if (SelectTag1.Checked)
                {
                    CheckBoxBuffer[0] = true;
                    if (CheckInputs(0) == false) break;
                }
                else
                {
                    CheckBoxBuffer[0] = false;
                }
                if (SelectTag2.Checked)
                {
                    CheckBoxBuffer[1] = true;
                    if (CheckInputs(1) == false) break;
                }
                else
                {
                    CheckBoxBuffer[1] = false;
                }
                if (SelectTag3.Checked)
                {
                    CheckBoxBuffer[2] = true;
                    if (CheckInputs(2) == false) break;
                }
                else
                {
                    CheckBoxBuffer[2] = false;
                }
                if (SelectTag4.Checked)
                {
                    CheckBoxBuffer[3] = true;
                    if (CheckInputs(3) == false) break;
                }
                else
                {
                    CheckBoxBuffer[3] = false;
                }
                if (SelectTag5.Checked)
                {
                    CheckBoxBuffer[4] = true;
                    if (CheckInputs(4) == false) break;
                }
                else
                {
                    CheckBoxBuffer[4] = false;
                }
                if (SelectTag6.Checked)
                {
                    CheckBoxBuffer[5] = true;
                    if (CheckInputs(5) == false) break;
                }
                else
                {
                    CheckBoxBuffer[5] = false;
                }
                // Passed all input tests - inputs are verified
                return true;
            }
            #pragma warning restore
            return false;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name:  private void AppendDataOnRecordText(int j, string [] splitData)
        // Description: Append data on the Record Text box
        //              Data are appened in the order - Barcode, D.S.R., and Catalog
        //              Each data contains start byte address, data length, data, and CRC
        //-----------------------------------------------------------------------------------------------------------
        private void AppendDataOnRecordText(int j, string [] splitData)
        {
            RecordList.AppendText("   ");
            // Address
            RecordList.AppendText("0x" + splitData[j]);
            RecordList.AppendText("        ");
            // Length
            RecordList.AppendText("0x" + splitData[j + 1]);
            RecordList.AppendText("            ");
            // Data
            if (splitData[2].Contains("#"))
                splitData[2] = splitData[2].Replace("#", " ");
            RecordList.AppendText(splitData[j + 2]);
            RecordList.AppendText("\n");
            // CRC
            RecordListCRC.AppendText(" 0x" + splitData[j + 3]);
            RecordListCRC.AppendText("\n");
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name:  private void DisplayDataRecord(string[] DataRecord)
        // Description: Display data record texts - Record List
        //-----------------------------------------------------------------------------------------------------------
        private void DisplayDataRecord(string[] DataRecord)
        {
            RecordList.Clear();
            RecordListCRC.Clear();
            this.RecordList.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.RecordListCRC.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            for (int i = 0; i < DataRecord.Length; i++)
            {
                if (DataRecord.Length > 0)
                {
                    string[] splitData = DataRecord[i].Split(' ');
                    // MessageBox.Show(DataRecord[i]);
                    if (splitData.Length == 4)
                    {
                        AppendDataOnRecordText(0, splitData);
                    }
                    else if (splitData.Length == 8)
                    {
                        for (int j = 0; j < 5; j = j + 4)
                            AppendDataOnRecordText(j, splitData);
                    }
                    else
                    {
                        // for debugging purpose
                        // MessageBox.Show(DataRecord[i]);
                        MessageBox.Show("Invalid data!");
                    }
                }
                else
                {
                    // for debugging purpose
                    MessageBox.Show("Data error!");
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name:  private void OkBotton_Click(object sender, EventArgs e)
        // Description: Press button - processs user input data
        //-----------------------------------------------------------------------------------------------------------
        private void OkBotton_Click(object sender, EventArgs e)
        {
            ReadDataFieldsToValidate();
            DataToWrite = new string[6];
            for (int i = 0; i < 6; i++)
            {
                DataToWrite[i] = "";
            }
            if (tabControl1.SelectedTab == tabControl1.Controls[0]) // Tab page - Edit Data
            {
                bool verified = CheckInputContents();
                // if the user input data bytes are in correct format continue
                if (verified)
                    SendDataToPropeller();
            }
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name:  private void SendDataToPropeller()
        // Description: Send data (Barcode, DSR, and Catalog no.) to the propeller
        //              - Receive user input data
        //              - Determine the data bytes to write to EPROM of ID Tags
        //              - Determine if there is enough memory in the EPROM of the Tags
        //              - Assign tags to the data bytes
        //              - Order data bytes in sequence
        //              - Write data bytes to the Serial Port
        //-----------------------------------------------------------------------------------------------------------
        private void SendDataToPropeller()
        {
            bool MemoryEnough = true;
            for (int i = 0; i < CheckBoxBuffer.Length; i++)
            {
                if (CheckBoxBuffer[i])
                {
                    // MessageBox.Show(i.ToString());
                    string tag = compute.GetTag(i);
                    // Erase Barcode = x
                    // Erase DSR = y
                    // Erase Catalog number = z
                    string eraseFlag = "";
                    bool EraseBarcode = compute.EraseData(BarcodeBuffer[i], UserChangedBarcode[i], "Barcode", ref eraseFlag);
                    bool EraseDsr = compute.EraseData(DsrBuffer[i], UserChangedDSR[i], "DSR", ref eraseFlag);
                    bool EraseCatalog = compute.EraseData(CatalogBuffer[i], UserChangedCatalog[i], "Catalog-number", ref eraseFlag);
                    if (EraseBarcode || EraseDsr || EraseCatalog)
                    {
                        string warning = "Do you want to erase " + compute.EraseDataFields(eraseFlag) + " \r\n";
                        warning += " from ID Tag " + (i + 1).ToString() + " ?";
                        DialogResult dialogue = MessageBox.Show(warning, "Erase data !", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dialogue == DialogResult.Yes)
                        {
                            string barcode = (EraseBarcode == true) ? "x" : compute.GetDataToWrite(BarcodeBuffer[i], UserChangedBarcode[i]);
                            string dsr = (EraseDsr == true) ? "y" : compute.GetDataToWrite(DsrBuffer[i], UserChangedDSR[i]);
                            string catalog; 
                            if (barcode.Length / 2 >= 7)
                                catalog = compute.GetDataToWrite(CatalogBuffer[i], UserChangedCatalog[i], 1, DataReplacedCount[i]);
                            else if (dsr.Length / 2 >= 3 && dsr.Length / 2 <= 7)
                                catalog = compute.GetDataToWrite(CatalogBuffer[i], UserChangedCatalog[i], 1, DataReplacedCount[i]);
                            else
                                catalog = (EraseCatalog == true) ? "z" : compute.GetDataToWrite(CatalogBuffer[i], 
                                    UserChangedCatalog[i],0, DataReplacedCount[i]);
                            DataToWrite[i] = tag + barcode + dsr + catalog;
                            if ((barcode + dsr + catalog).Length > BytesRemained[i])
                            {
                                MemoryEnough = false;
                                MessageBox.Show("No enough memory in ID tag " + (i + 1).ToString() + ".");
                                break;
                            }
                        }
                    }
                    else
                    {
                        string barcode = compute.GetDataToWrite(BarcodeBuffer[i], UserChangedBarcode[i]);
                        string dsr = compute.GetDataToWrite(DsrBuffer[i], UserChangedDSR[i]);
                        string catalog = compute.GetDataToWrite(CatalogBuffer[i], UserChangedCatalog[i], (barcode + dsr).Length, DataReplacedCount[i]);
                        DataToWrite[i] = tag + dataToWrite(barcode, dsr, catalog, UserChangedBarcode[i], UserChangedDSR[i], UserChangedCatalog[i]);
                        if ((DataToWrite[i].Length  - 1)/2 > BytesRemained[i])
                        {
                            MemoryEnough = false;
                            MessageBox.Show("No enough memory in ID tag " + (i + 1).ToString() + ".");
                            break;
                        }
                    }
                    // Mark data types
                    MarkDataTypes(i, tag);
                }
            }
            if (MemoryEnough)
            {
                if (compute.DataFieldChanged(DataToWrite))
                {
                    WriteDataToSerialPort(DataToWrite);
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name:  private void MarkDataTypes(int i, string tag)
        // Description: 1) Compute and mark the starting address, length, and/or ending address 
        //              of data bytes to be erased from and/or writien into the EPROM.
        //              2) Sequence the data bytes in order
        //              - Tag number, Erase data type, start address, end address, Data start, data
        // Example: ax0105y06A0sB0bA876912090A0B099
        //          a - tag 1
        //          x - Barcode code erase command
        //          First byte (01) - Erase start address
        //          Last byte (05) - Erase end address
        //          y - DSR code erase command
        //          First byte (06) - Erase start address
        //          Last byte (009) - Erase end address
        //          s - Start of data bytes
        //          If previous catalog number needs to be erased, erase that data in the range 
        //          of first two bytes (B0 - BA), where B0 is the start address and BA is the
        //          end address. 
        //          876912090A0B099 - data bytes to be written into EPROM
        //          if the end address is 00, the end byte will be computed in the
        //          hardware by reading data from the EPROM starting at address B0 until data byte reads "FF".
        //-----------------------------------------------------------------------------------------------------------
        private void MarkDataTypes(int i, string tag)
        {
            string bar_x = "", dsr_y = "", cat_z = "", data_bytes = "";
            string BYTES = DataToWrite[i];
            for (int a = 1; a < BYTES.Length; a++)
            {
                if (BYTES[a] == 'x')
                    bar_x = "x" + compute.GetDataPosLen(TagsDataBuffer[i], "Barcode");
                else if (BYTES[a] == 'y')
                    dsr_y = "y" + compute.GetDataPosLen(TagsDataBuffer[i], "DSR");
                else if (BYTES[a] == 'z')
                    cat_z = "z" + compute.GetDataPosLen(TagsDataBuffer[i], "Catalog");
                else
                {
                    data_bytes += BYTES[a];
                }
            }
            string type = "";
            if (data_bytes.Length > 1)
            {
                type = data_bytes[0].ToString() + data_bytes[1].ToString();
                if (Convert.ToInt32(type, 16) == 8 || Convert.ToInt32(type, 16) == 9)
                    data_bytes = compute.GetDataPosLen(TagsDataBuffer[i], "Zap-Barcode") + data_bytes;
                else if (Convert.ToInt32(type, 16) >= 3 && Convert.ToInt32(type, 16) <= 7)
                    data_bytes = compute.GetDataPosLen(TagsDataBuffer[i], "Zap-DSR") + data_bytes;
                else if (Convert.ToInt32(type, 16) >= 10 && Convert.ToInt32(type, 16) <= 47)
                    data_bytes = compute.GetDataPosLen(TagsDataBuffer[i], "Zap-Catalog") + data_bytes;
                // MessageBox.Show("here " + data_bytes);
            }
            DataToWrite[i] = tag + bar_x + dsr_y + cat_z + "s" + data_bytes;
            // if data_bytes[data_bytes.Length - 1] == 'j') simply ignore first byte
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void WriteDataToSerialPort(string[] DataBytes)
        // Description:  Write data bytes to the Serial Port (one byte a time)
        //               Process:
        //               1) Send a write signal - character 'w'
        //               2) Receive acknowledge signal from hardware - string 'ack'
        //               3) Send data bytes
        //               4) Send end signal of data bytes - character 'q'
        // Note: The signal characters and strings are unique, but arbitray choosen.
        //-----------------------------------------------------------------------------------------------------------
        private void WriteDataToSerialPort(string[] DataBytes)
        {
            string[] ports = SerialPort.GetPortNames();
            if (!ports.Contains(PORT))
            {
                MessageBox.Show("Hardware is diconnected \n Connect the hardware and try again.");
            }
            else
            {
                serialPort1 = new SerialPort(PORT);
                serialPortInit(serialPort1);
                if (!serialPort1.IsOpen)
                {
                    serialPort1.Open();
                    // Wait for a millisecond
                    Thread.Sleep(1);
                    // Send write command to hardware
                    serialPort1.Write("w");
                    while (true)
                    {
                        string response = serialPort1.ReadExisting();
                        if (response.Contains("ack"))
                        {
                            // MessageBox.Show(response); // For debugging only
                            int msg = 1;
                            foreach (string data in DataBytes)
                            {
                                if (data.Length > 3)
                                {
                                    MessageBox.Show("Tag " + msg.ToString() + ": " + data);
                                    foreach (char c in data)
                                        serialPort1.Write(c.ToString());
                                }
                                msg++;
                            }
                            Thread.Sleep(1);
                            // Send the end signal
                            serialPort1.Write("q");
                            Thread.Sleep(1);
                            // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                            // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                            // For Debuggin porpose only
                            while (true)
                            {
                                string get = serialPort1.ReadExisting();
                                MessageBox.Show(get);
                                if (get.Contains("ended"))
                                {
                                    MessageBox.Show("ended");
                                    break;
                                }       
                            }
                            // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                            // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                            break;
                        }
                        Thread.Sleep(1);
                    }
                    serialPort1.Close();
                    // ReloadData();
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name:private string dataToWrite(string barcode, string dsr, string catalog, string currentBarcode,
        //                                          string currentDsr, string currentCatalog)
        // Description: Compute and return data bytes 
        //-----------------------------------------------------------------------------------------------------------
        private string dataToWrite(string barcode, string dsr, string catalog, string currentBarcode,
                                                             string currentDsr, string currentCatalog)
        {
            int[] array = { 1, 2 }; // dummy initialization - size is overidden
            if (barcode.Length > 0)
            {
                if (currentDsr.Length > 0)
                    barcode += compute.CharToHex(currentDsr, ref array) + crc.crc8(array);
                if (currentCatalog.Length > 0)
                    barcode += compute.CharToHex(currentCatalog, ref array) + crc.crc8(array);
                return barcode;
            }
            else
            {
                if (dsr.Length > 0)
                {
                    if (currentCatalog.Length > 0)
                        dsr += compute.CharToHex(currentCatalog, ref array) + crc.crc8(array);
                    return dsr;
                }
            }
            return catalog;
        }
        
        //
        // Cancel the action and close the window
        // 
        private void CancelBotton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Abort ID Tag content?", "ID Tag content", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            if (result == DialogResult.Yes)
            { 
                this.Close(); 
            }
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void HelpBotton_Click(object sender, EventArgs e)
        // Description: Display Help text on the help window
        //-----------------------------------------------------------------------------------------------------------
        private void HelpBotton_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabControl1.Controls[0]) // Edit data
            {
                HelpWindow helpWindow = new HelpWindow(1);
                helpWindow.ShowDialog();
            }
            else if (tabControl1.SelectedTab == tabControl1.Controls[1]) // Record List
            {
                HelpWindow helpWindow = new HelpWindow(2);
                helpWindow.ShowDialog();
            }
            else if (tabControl1.SelectedTab == tabControl1.Controls[2]) // Hex Data
            {
                HelpWindow helpWindow = new HelpWindow(3);
                helpWindow.ShowDialog();
            }
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private string DataRequest(SerialPort serialPort1);
        // Description: 1) Request for data bytes from the ID Tags
        //              2) Receive data bytes from the ID Tags as character values and store
        //                 them as a string.
        //              3) Receive end signal of data bytes from the Hardware, and
        //                 terminate function.
        //-----------------------------------------------------------------------------------------------------------
        private void DataRequest(SerialPort serialPort1)
        {
            serialPort1 = new SerialPort(PORT);
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
                        if (response.Contains("finished"))
                        {
                            compute.SerialStrToBytes(data, ref DataBuffer);
                            //MessageBox.Show(data);
                            serialPort1.Close();
                            return;
                        }
                    }
                    Thread.Sleep(1);
                }
            }
            return;
        }

        //
        // Check if input contains lower case char(s)
        // 
        private bool InputContainsLowerChar(string InputText)
        {
            for (int i = 0; i < InputText.Length; i++)
            {
                if (InputText[i] >= 'a' && InputText[i] <= 'z')
                    return true;
            }
            return false;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void RefreshButton_Click(object sender, EventArgs e)
        // Description: Reload the data from the ID Tags
        //-----------------------------------------------------------------------------------------------------------
        private void RefreshButton_Click(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            if (!ports.Contains(PORT))
            {
                MessageBox.Show("Hardware is diconnected \n Please, connect the hardware and try again.");
            }
            else
            {
                ReloadData();
            }
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name:  private void ReloadData()
        // Description: Reload data from the memory
        //-----------------------------------------------------------------------------------------------------------
        private void ReloadData()
        {
            // Progress bar initialization
            this.progressBar1.Visible = true;
            this.progressBar1.Maximum = ProgressMax;
            this.progressBar1.Minimum = 0;
            this.progressBar1.Value = 0;
            this.progressBar1.ForeColor = Color.Blue;
            // Checkboxs init
            EnableRecordDataTagList();
            EnableHexDataTagList();
            DataBuffer = new string[6];
            for (int i = 0; i < 6; i++)
                DataBuffer[i] = " ";
            // Request data 
            DataRequest(serialPort1);
            this.progressBar1.Value = 1900;
            ClearAllDataFields();
            EnableEditDataTagList();
            // Fill the ID Tag data buffer
            InitDataBuffer(DataBuffer);
            // Read data contents from the appropriate text boxes
            // - Barcode, DSR, and Catalog Number
            ReadDataFields();
            // Init Hex data column bar
            HexDataCountBar();
            this.progressBar1.Visible = false;
            this.progressBar1.Value = 0;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name:  void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        // Description: Tab page control --
        //              Control push botton on the tab pages
        //              Display particular push button(s) on a tab page
        //-----------------------------------------------------------------------------------------------------------
        void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (tabControl1.SelectedTab == tabControl1.Controls[1] || tabControl1.SelectedTab == tabControl1.Controls[2])
            {
                //OkBotton.Text = "Display";
                OkBotton.Visible = false;
                RefreshButton.Visible = false;
            }
            else if (tabControl1.SelectedTab == tabControl1.Controls[0])
            {
                OkBotton.Visible = true;
                OkBotton.Text = "OK";
                RefreshButton.Visible = true;
            }
        }

        // The functions : functionX_TextChanged(object sender, EventArgs e)
        // below control the display on the text boxes.
        // Not requried, but implemented for design purpose only.
        private void BarcodeText1_TextChanged(object sender, EventArgs e)
        {
            if (InputContainsLowerChar(BarcodeText1.Text))
            {
                CursorPosition = BarcodeText1.SelectionStart;
                BarcodeText1.Text = BarcodeText1.Text.ToUpper();
                BarcodeText1.Select(CursorPosition, 0);
            }
        }

        private void BarcodeText2_TextChanged(object sender, EventArgs e)
        {
            if (InputContainsLowerChar(BarcodeText2.Text))
            {
                CursorPosition = BarcodeText2.SelectionStart;
                BarcodeText2.Text = BarcodeText2.Text.ToUpper();
                BarcodeText2.Select(CursorPosition, 0);
            }
        }

        private void BarcodeText3_TextChanged(object sender, EventArgs e)
        {
            if (InputContainsLowerChar(BarcodeText3.Text))
            {
                CursorPosition = BarcodeText3.SelectionStart;
                BarcodeText3.Text = BarcodeText3.Text.ToUpper();
                BarcodeText3.Select(CursorPosition, 0);
            }
        }

        private void BarcodeText4_TextChanged(object sender, EventArgs e)
        {
            if (InputContainsLowerChar(BarcodeText4.Text))
            {
                CursorPosition = BarcodeText4.SelectionStart;
                BarcodeText4.Text = BarcodeText4.Text.ToUpper();
                BarcodeText4.Select(CursorPosition, 0);
            }
        }

        private void BarcodeText5_TextChanged(object sender, EventArgs e)
        {
            if (InputContainsLowerChar(BarcodeText5.Text))
            {
                CursorPosition = BarcodeText5.SelectionStart;
                BarcodeText5.Text = BarcodeText5.Text.ToUpper();
                BarcodeText5.Select(CursorPosition, 0);
            }
        }

        private void BarcodeText6_TextChanged(object sender, EventArgs e)
        {
            if (InputContainsLowerChar(BarcodeText6.Text))
            {
                CursorPosition = BarcodeText6.SelectionStart;
                BarcodeText6.Text = BarcodeText6.Text.ToUpper();
                BarcodeText6.Select(CursorPosition, 0);
            }
        }

        private void DsrText1_TextChanged(object sender, EventArgs e)
        {
            if (InputContainsLowerChar(DsrText1.Text))
            {
                CursorPosition = DsrText1.SelectionStart;
                DsrText1.Text = DsrText1.Text.ToUpper();
                DsrText1.Select(CursorPosition, 0);
            }
        }

        private void DsrText2_TextChanged(object sender, EventArgs e)
        {
            if (InputContainsLowerChar(DsrText2.Text))
            {
                CursorPosition = DsrText2.SelectionStart;
                DsrText2.Text = DsrText2.Text.ToUpper();
                DsrText2.Select(CursorPosition, 0);
            }
        }

        private void DsrText3_TextChanged(object sender, EventArgs e)
        {
            if (InputContainsLowerChar(DsrText3.Text))
            {
                CursorPosition = DsrText3.SelectionStart;
                DsrText3.Text = DsrText3.Text.ToUpper();
                DsrText3.Select(CursorPosition, 0);
            }
        }
        private void DsrText4_TextChanged(object sender, EventArgs e)
        {
            if (InputContainsLowerChar(DsrText4.Text))
            {
                CursorPosition = DsrText4.SelectionStart;
                DsrText4.Text = DsrText4.Text.ToUpper();
                DsrText4.Select(CursorPosition, 0);
            }
        }

        private void DsrText5_TextChanged(object sender, EventArgs e)
        {
            if (InputContainsLowerChar(DsrText5.Text))
            {
                CursorPosition = DsrText5.SelectionStart;
                DsrText5.Text = DsrText5.Text.ToUpper();
                DsrText5.Select(CursorPosition, 0);
            }
        }

        private void DsrText6_TextChanged(object sender, EventArgs e)
        {
            if (InputContainsLowerChar(DsrText6.Text))
            {
                CursorPosition = DsrText6.SelectionStart;
                DsrText6.Text = DsrText6.Text.ToUpper();
                DsrText6.Select(CursorPosition, 0);
            }
        }

        private void CatalogText1_TextChanged(object sender, EventArgs e)
        {
            if (InputContainsLowerChar(CatalogText1.Text))
            {
                CursorPosition = CatalogText1.SelectionStart;
                CatalogText1.Text = CatalogText1.Text.ToUpper();
                CatalogText1.Select(CursorPosition, 0);
            }
        }

        private void CatalogText2_TextChanged(object sender, EventArgs e)
        {
            if (InputContainsLowerChar(CatalogText2.Text))
            {
                CursorPosition = CatalogText2.SelectionStart;
                CatalogText2.Text = CatalogText2.Text.ToUpper();
                CatalogText2.Select(CursorPosition, 0);
            }
        }

        private void CatalogText3_TextChanged(object sender, EventArgs e)
        {
            if (InputContainsLowerChar(CatalogText3.Text))
            {
                CursorPosition = CatalogText3.SelectionStart;
                CatalogText3.Text = CatalogText3.Text.ToUpper();
                CatalogText3.Select(CursorPosition, 0);
            }
        }

        private void CatalogText4_TextChanged(object sender, EventArgs e)
        {
            if (InputContainsLowerChar(CatalogText4.Text))
            {
                CursorPosition = CatalogText4.SelectionStart;
                CatalogText4.Text = CatalogText4.Text.ToUpper();
                CatalogText4.Select(CursorPosition, 0);
            }
        }

        private void CatalogText5_TextChanged(object sender, EventArgs e)
        {
            if (InputContainsLowerChar(CatalogText5.Text))
            {
                CursorPosition = CatalogText5.SelectionStart;
                CatalogText5.Text = CatalogText5.Text.ToUpper();
                CatalogText5.Select(CursorPosition, 0);
            }
        }

        private void CatalogText6_TextChanged(object sender, EventArgs e)
        {
            if (InputContainsLowerChar(CatalogText6.Text))
            {
                CursorPosition = CatalogText6.SelectionStart;
                CatalogText6.Text = CatalogText6.Text.ToUpper();
                CatalogText6.Select(CursorPosition, 0);
            }
        }

        // Tag 1 - Record list
        private void rSelectTag1_CheckedChanged_1(object sender, EventArgs e)
        {
            compute.GetRecordList(TagsDataBuffer[0], ref DataRecord);
            DisplayDataRecord(DataRecord);
            // MessageBox.Show(DataRecord[0]);
        }

        // Tag 2 - Record List
        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            compute.GetRecordList(TagsDataBuffer[1], ref DataRecord);
            DisplayDataRecord(DataRecord);
            // MessageBox.Show(DataRecord[0]);
        }

        // Tag 3 - Record List
        private void rSelectTag3_CheckedChanged_1(object sender, EventArgs e)
        {
            compute.GetRecordList(TagsDataBuffer[2], ref DataRecord);
            DisplayDataRecord(DataRecord);
        }

        // Tag 4 - Record List
        private void rSelectTag4_CheckedChanged_1(object sender, EventArgs e)
        {
            compute.GetRecordList(TagsDataBuffer[3], ref DataRecord);
            DisplayDataRecord(DataRecord);
        }

        // Tag 5 - Record List
        private void rSelectTag5_CheckedChanged_1(object sender, EventArgs e)
        {
            compute.GetRecordList(TagsDataBuffer[4], ref DataRecord);
            DisplayDataRecord(DataRecord);
        }

        // Tag 6 - Record List
        private void rSelectTag6_CheckedChanged(object sender, EventArgs e)
        {
            compute.GetRecordList(TagsDataBuffer[5], ref DataRecord);
            DisplayDataRecord(DataRecord);
        }

        private void sRecordTag6_CheckedChanged(object sender, EventArgs e)
        {
            HexDataTextBox.Clear();
            DisplayHexDataTextBox(TagsDataBuffer[5], 5);
        }

        private void rSelectTag5_CheckedChanged(object sender, EventArgs e)
        {
            HexDataTextBox.Clear();
            DisplayHexDataTextBox(TagsDataBuffer[4], 4);
        }

        private void rSelectTag4_CheckedChanged(object sender, EventArgs e)
        {
            HexDataTextBox.Clear();
            DisplayHexDataTextBox(TagsDataBuffer[3], 3);
        }

        private void rSelectTag2_CheckedChanged(object sender, EventArgs e)
        {
            HexDataTextBox.Clear();
            DisplayHexDataTextBox(TagsDataBuffer[1], 1);
        }

        private void rSelectTag3_CheckedChanged(object sender, EventArgs e)
        {
            HexDataTextBox.Clear();
            DisplayHexDataTextBox(TagsDataBuffer[2], 2);
        }

        private void rSelectTag1_CheckedChanged(object sender, EventArgs e)
        {
            HexDataTextBox.Clear();
            DisplayHexDataTextBox(TagsDataBuffer[0], 0);
        }

        private void sRecordTag6_CheckedChanged_1(object sender, EventArgs e)
        {
            HexDataTextBox.Clear();
            DisplayHexDataTextBox(TagsDataBuffer[5], 5);
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void EnableRecordDataTagList()
        // Description: Enable all radio buttons for selecting ID Tags
        //              in the Record List window
        //-----------------------------------------------------------------------------------------------------------
        private void EnableRecordDataTagList()
        {
            this.rSelectTag1.Checked = false;
            this.rSelectTag2.Checked = false;
            this.rSelectTag3.Checked = false;
            this.rSelectTag4.Checked = false;
            this.rSelectTag5.Checked = false;
            this.rSelectTag6.Checked = false;

            this.rSelectTag1.Enabled = true;
            this.rSelectTag2.Enabled = true;
            this.rSelectTag3.Enabled = true;           
            this.rSelectTag4.Enabled = true;
            this.rSelectTag5.Enabled = true;
            this.rSelectTag6.Enabled = true;
        }

        // Enable all radio buttons for selecting ID Tags
        // in the Hex Data list window
        private void EnableHexDataTagList()
        {
            this.sRecordTag1.Checked = false;
            this.sRecordTag2.Checked = false;
            this.sRecordTag3.Checked = false;
            this.sRecordTag4.Checked = false;
            this.sRecordTag5.Checked = false;
            this.sRecordTag6.Checked = false;

            this.sRecordTag1.Enabled = true;
            this.sRecordTag2.Enabled = true;
            this.sRecordTag3.Enabled = true;
            this.sRecordTag4.Enabled = true;
            this.sRecordTag5.Enabled = true;
            this.sRecordTag6.Enabled = true;
        }

        // Enable all check box for selecting ID Tags
        // in the Edit Data window
        private void EnableEditDataTagList()
        {
            this.SelectTag1.Checked = false;
            this.SelectTag2.Checked = false;
            this.SelectTag3.Checked = false;
            this.SelectTag4.Checked = false;
            this.SelectTag5.Checked = false;
            this.SelectTag6.Checked = false;

            this.SelectTag1.Enabled = true;
            this.SelectTag2.Enabled = true;
            this.SelectTag3.Enabled = true;
            this.SelectTag4.Enabled = true;
            this.SelectTag5.Enabled = true;
            this.SelectTag6.Enabled = true;
        }

        // Clear all data fields in the Edit Data window
        private void ClearAllDataFields()
        {
            this.BarcodeText1.Clear();
            this.BarcodeText2.Clear();
            this.BarcodeText3.Clear();
            this.BarcodeText4.Clear();
            this.BarcodeText5.Clear();
            this.BarcodeText6.Clear();

            this.DsrText1.Clear();
            this.DsrText2.Clear();
            this.DsrText3.Clear();
            this.DsrText4.Clear();
            this.DsrText5.Clear();
            this.DsrText6.Clear();

            this.CatalogText1.Clear();
            this.CatalogText2.Clear();
            this.CatalogText3.Clear();
            this.CatalogText4.Clear();
            this.CatalogText5.Clear();
            this.CatalogText6.Clear();
        }

        // Check box controls
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (SelectTag1.Checked == true)
            {
                BarcodeText1.ReadOnly = false;
                DsrText1.ReadOnly = false;
                CatalogText1.ReadOnly = false;
            }
            else
            {
                BarcodeText1.ReadOnly = true;
                DsrText1.ReadOnly = true;
                CatalogText1.ReadOnly = true;
            }
        }

        private void SelectTag2_CheckedChanged(object sender, EventArgs e)
        {
            if (SelectTag2.Checked == true)
            {
                BarcodeText2.ReadOnly = false;
                DsrText2.ReadOnly = false;
                CatalogText2.ReadOnly = false;
            }
            else
            {
                BarcodeText2.ReadOnly = true;
                DsrText2.ReadOnly = true;
                CatalogText2.ReadOnly = true;
            }
        }

        private void SelectTag3_CheckedChanged(object sender, EventArgs e)
        {
            if (SelectTag3.Checked == true)
            {
                BarcodeText3.ReadOnly = false;
                DsrText3.ReadOnly = false;
                CatalogText3.ReadOnly = false;
            }
            else
            {
                BarcodeText3.ReadOnly = true;
                DsrText3.ReadOnly = true;
                CatalogText3.ReadOnly = true;
            }
        }

        private void SelectTag4_CheckedChanged(object sender, EventArgs e)
        {
            if (SelectTag4.Checked == true)
            {
                BarcodeText4.ReadOnly = false;
                DsrText4.ReadOnly = false;
                CatalogText4.ReadOnly = false;
            }
            else
            {
                BarcodeText4.ReadOnly = true;
                DsrText4.ReadOnly = true;
                CatalogText4.ReadOnly = true;
            }
        }

        private void SelectTag5_CheckedChanged(object sender, EventArgs e)
        {
            if (SelectTag5.Checked == true)
            {
                BarcodeText5.ReadOnly = false;
                DsrText5.ReadOnly = false;
                CatalogText5.ReadOnly = false;
            }
            else
            {
                BarcodeText5.ReadOnly = true;
                DsrText5.ReadOnly = true;
                CatalogText5.ReadOnly = true;
            }
        }

        private void SelectTag6_CheckedChanged(object sender, EventArgs e)
        {
            if (SelectTag6.Checked == true)
            {
                BarcodeText6.ReadOnly = false;
                DsrText6.ReadOnly = false;
                CatalogText6.ReadOnly = false;
            }
            else
            {
                BarcodeText6.ReadOnly = true;
                DsrText6.ReadOnly = true;
                CatalogText6.ReadOnly = true;
            }
        }

        // Auto generated functions - Do not delete the functions below.
        // The program may give error or the UI designer form may not open if
        // any of the functions below is deleted.
        private void DataView_Load(object sender, EventArgs e)
        {
            // Not implemented
        }
        private void tabPage1_Click(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void tabPage1_Click_1(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void textRecordList_TextChanged(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void tabPage2_Click_1(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void tabPage6_Click(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void tabPage5_Click(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void DSR_Enter(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void tabPage4_Click(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void Tag_Number_Enter(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void label7_Click(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void RecordList_TextChanged(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void RecordSelectTagList_Enter(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void BarCRC_TextChanged(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void DataBytes_Click(object sender, EventArgs e)
        {
            // Not implemented
        }

        private void SerialNoLabel_Click(object sender, EventArgs e)
        {
            // Not implemented
        }
    }   
}
