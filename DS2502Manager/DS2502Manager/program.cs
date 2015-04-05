//****************************************************************************************************************************||
//----------------------------------------------------------------------------------------------------------------------------||
// Project name            : Senior Capstone Design
// Project term            : Fall 2014 - Spring 2015
// Title                   : Simultaneous Programming of Multiple ID Tags
// Sponsor                 : General Electric
// Customer name           : Mr. Michael Austin

// Instructor              : Prof. Gino manzo
// Subject Matter Expert   : Dr. William Plymale
// Team name               : BridgeBuilders 
// Members                 : Arun Rai, Danny Mota, Mohammad Islam, and Xin Gan
// Author(s)               : Arun Rai
// Date                    : 02/20/2015
// Reviewed by             : 
// Description: This software allows a user to read and program multiple DS2502 (aka ID Tags), upto 6 devices, 
//              through a Command line Interface.
//----------------------------------------------------------------------------------------------------------------------------||
//****************************************************************************************************************************||

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO.Ports;
using System.Threading;
using System.Text.RegularExpressions;

namespace DS2502Manager
{
    class program
    {
        private static string[] BarcodeList;
        private static string[] DsrList;
        private static string[] CatalogNoList;
        private static string[] BarcodeBuffer;
        private static string[] DsrBuffer;
        private static string[] CatalogNoBuffer;
        private static bool[] CheckBoxBuffer;
        private static string[] HexList;
        private static string[] TagsDataBuffer;
        private static string[] DataRecord;
        private static string[] DataToWrite;
        private static string[] DataBuffer;
        private static int[]TagAvailable;
        private static int[] DataReplacedCount;
        private static int[] remainedBytes;
        private static string PORT;
        private static bool ReadingComplete;
        DataIn input;
        DataOut output;
        crc _crc;
        SerialPort serialPort;
        private static bool portDetected;
        private static string portName;

        // Main function
        static void Main(string[] args)
        {
            program self = new program();
            self.ProcessData(args);
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: public void  ProcessData(string portName, string[] InBytes)
        // Description: process data
        //-----------------------------------------------------------------------------------------------------------
        private void ProcessData(string [] args)
        {
            if (args.Length == 2)
            {
                if (args[0] == "Read" || args[0] == "read" || args[0] == "READ")
                {
                    ReadingComplete = true;
                    portDetected = false;
                    int tag = 0;
                    NewArrays();
                    InitArrays();
                    // Instantiate classes
                    input = new DataIn();
                    output = new DataOut();
                    _crc = new crc();
                    // InitDataBuffer(TagsDataBuffer);
                    // Read data contents from the appropriate text boxes
                    // - Barcode, DSR, and Catalog Number
                    if (args[1].Length == 1)
                    {
                        tag = Convert.ToInt32(args[1]);
                        if ( tag > 0 && tag < 7)
                        {
                            string display = "single";
                            Read(tag, display);
                        }
                        else
                        {
                            Console.WriteLine("Error: invalid tag number.");
                        }
                    }
                    else
                    {
                        if (args[1] == "all")
                        {
                            string display = "all";
                            tag = 0;
                            Read(tag, display);
                        }
                        else
                        {
                            Console.WriteLine("Error: invalid read command.");
                        }
                    }
                }
            }
            else if (args.Length > 2)
            {
                if (args[0] == "write" || args[0] == "Write" || args[0] == "WRITE") // write commmand
                {
                    // write command
                }
                else if (args[0] == "erase" || args[0] == "Erase" || args[0] == "ERASE") // erase commmand
                {
                    // Erase command
                }
            }
            else if (args.Length == 1)
            {
                Console.WriteLine("Error: unsufficient arguements.");
            }
            else
            {
                Console.WriteLine("Error: no input command.");
            }
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void InitComponents()
        // Description: initialize the components
        //-----------------------------------------------------------------------------------------------------------
        private void InitComponents()
        {
            ReadingComplete = true;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void NewArrays()
        // Description: Define arrays
        //-----------------------------------------------------------------------------------------------------------
        private void NewArrays()
        {
            TagsDataBuffer = new string[6];
            DataRecord = new string[3];
            remainedBytes = new int[6];
            BarcodeList = new string[6];
            DsrList = new string[6];
            CatalogNoList = new string[6];
            HexList = new string[128];
            BarcodeBuffer = new string[6];
            DsrBuffer = new string[6];
            CatalogNoBuffer = new string[6];
            CheckBoxBuffer = new bool[6];
            TagAvailable = new int[6];
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void Read(int tag, string display)
        // Description: Read data bytes from the ID Chip(s)
        //-----------------------------------------------------------------------------------------------------------
        private void Read(int tag, string display)
        {
            Console.WriteLine(" Processing...............................");
            DataBuffer = new string[6];
            for (int i = 0; i < 6; i++)
                DataBuffer[i] = "";
            PortAutoDetect();
            if(portDetected)
            {
                DataRequest();
                InitDataBuffer(DataBuffer, display, tag);
                if (TagsExist())
                {
                    if (display == "all")
                    {
                        for (int m = 1; m <= DataBuffer.Length; m++)
                        {
                            if (DataBuffer[m - 1].Length > 0)
                                PrintData(m);
                        }
                    }
                    else
                    {
                        PrintSingleChip(DataBuffer[tag - 1], display, tag);
                    }
                    Console.WriteLine("");
                    if (ReadingComplete)
                        Console.WriteLine("                 Reading completed. ");
                }
                else
                {
                    Console.WriteLine(" No ID Chip recognized.");
                }
            }
            else
            {
                Console.WriteLine("No Hardware detected.");
            }
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private bool TagsExist()
        // Description: return true if at least one ID Chip is connected, return false otherwise.
        //-----------------------------------------------------------------------------------------------------------
        private bool TagsExist()
        {
            foreach (string element in DataBuffer)
            {
                if(element.Contains("tag"))
                    return true;
            }
            return false;
        }

        
        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void InitArrays()
        // Description: Initialize arrays
        //-----------------------------------------------------------------------------------------------------------
        private void InitArrays()
        {
            for (int i = 0; i < 3; i++)
                DataRecord[i] = "a";
            for (int j = 0; j < 6; j++ )
            {
                remainedBytes[j] = 127;
                TagAvailable[j] = 0;
                TagsDataBuffer[j] = "";
            }
            for (int m = 0; m <= 127; m++)
                HexList[m] = "FF";
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void InitDataBuffer(string[] IDTagData)
        // Description: Fill data buffer and print data bytes on the console
        //-----------------------------------------------------------------------------------------------------------
        private void InitDataBuffer(string[] IDTagData, string display, int tag)
        {
            // Fill data contents in the appropriate
            // text boxes - Barcode, DSR, and Catalog Number
            for (int i = 1; i <= IDTagData.Length; i++)
            {
                if (IDTagData[i - 1].Length > 1)
                {
                    TagsDataBuffer[i - 1] = input.SpaceDelimitor(input.RemoveExraWords(IDTagData[i - 1]));
                }
                else
                {
                    TagsDataBuffer[i - 1] = "";
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void PrintSingleChip(string data, string display, int tag)
        // Description: Print data bytes from the EPROM of a single ID Chip
        // (when user wants to read data from a specific ID Chip)
        //-----------------------------------------------------------------------------------------------------------
        private void PrintSingleChip(string data, string display, int tag)
        {
            if (display == "single")
            {
                if (tag < 7 && tag > 0)
                {
                    if (data.Length > 0)
                    {
                        PrintData(tag);
                        display = "displayed";
                    }
                    else
                    {
                        Console.WriteLine(" The ID Chip " + tag.ToString() + " does not exist.");
                        display = "displayed";
                        ReadingComplete = false;
                    }
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void PrintData(int i)
        // Description: Print data bytes from the EPROM of an ID Chip
        //-----------------------------------------------------------------------------------------------------------
        private void PrintData(int i)
        {
            Console.WriteLine(" ");
            Console.WriteLine("ID Chip: " + i.ToString());
            Console.WriteLine(" ");
            // Parse data string to get Record List
            input.GetRecordList(TagsDataBuffer[i - 1], ref DataRecord);
            // Print tecord Texts
            PrintDataRecord(DataRecord);
            // Print data bytes
            PrintBytes(TagsDataBuffer[i - 1], i);
            Console.WriteLine("");
            Console.WriteLine("|***************************************************************************|");
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private static void serialPortInit(SerialPort serialPort)
        // Description: Intialization of the serial port.
        //-----------------------------------------------------------------------------------------------------------
        private void serialPortInit()
        {
            serialPort.BaudRate = 115200;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.DataBits = 8;
            serialPort.Handshake = Handshake.None;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void PrintBytes(string Data)
        // Description: Print the ID Chip EPROM contents as hex.
        //              Each data byte contains two hex values, and each data byte is
        //              stored in an EPROM address of the ID Tag. 
        //-----------------------------------------------------------------------------------------------------------
        private void PrintBytes(string Data, int tag = -1)
        {
            string coloumn = "   Addrs 0x00";
            for (int a = 1; a < 8; a++)
                coloumn += "  0x" + a.ToString("X2");
            int used = 0, remaining = 0;
            int count1 = 0, count2 = 0, SerialCount = 0, AddCounter = 0;
            string st = "", SerialNumber = "", DataRow = "   ";
            DataRow += "0x00";
            DataRow += "   ";
            bool DisplaySerialNo = true;
            for (int i = 0; i < Data.Length - 3; i++)
            {
                if (char.IsLetterOrDigit(Data[i]))
                {
                    SerialCount++;
                    if (SerialCount <= 16)
                    {
                        SerialNumber += Data[i];
                    }
                    else
                    {
                        if (DisplaySerialNo)
                        {
                            Console.WriteLine("ID Chip Serial No.: " + SerialNumber);
                            Console.WriteLine(" ");
                            Console.WriteLine(" => Hex Data: ");
                            Console.WriteLine(" ");
                            Console.WriteLine(coloumn);
                            DisplaySerialNo = false;
                        }
                        count1++;
                        if (count1 <= 2)
                        {
                            st += Data[i];
                            if (count1 == 2)
                            {
                                if (st == "FF")
                                    remaining++;
                                else
                                    used++;
                                count2++; count1 = 0;
                                DataRow += st; AddCounter++;
                                DataRow += "    "; st = "";
                                if (count2 == 8)
                                {
                                    Console.WriteLine(DataRow);
                                    DataRow = "   0x" + AddCounter.ToString("X2");
                                    // Console.WriteLine("");
                                    DataRow += "   ";
                                    count2 = 0;
                                }
                            }
                        }
                    }
                }
            }           
            string usedStr = (used > 9) ? used.ToString() + " bytes used" : used.ToString() + " byte used";
            string remainStr = (remaining > 9) ? remaining.ToString() + " bytes remaining" : remaining.ToString() + " byte remaining";
            Console.WriteLine(" ");
            Console.WriteLine("        " + usedStr + " (" + remainStr + ")");
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name:  private void PrintColoumnBar(string[] DataRecord)
        // Description: Print coloumn bar
        //-----------------------------------------------------------------------------------------------------------
        private void PrintColoumnBar(string[] DataRecord)
        {
            Console.WriteLine("");
            Console.WriteLine(" => Record List: ");
            Console.WriteLine("");
            Console.WriteLine("   Addr     Length      Record Text                    CRC");
            Console.WriteLine("   --------------------------------------------------------");
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name:  private void PrintDataRecord(string[] DataRecord)
        // Description: Print data record 
        //-----------------------------------------------------------------------------------------------------------
        private void PrintDataRecord(string[] DataRecord)
        {
            if(DataRecord.Length > 0)
                PrintColoumnBar(DataRecord);
            for (int i = 0; i < DataRecord.Length; i++)
            {
                if (DataRecord.Length > 0)
                {
                    string[] splitData = DataRecord[i].Split(' ');
                    if (splitData.Length == 4)
                    {
                        Console.Write("   ");
                        // Address
                        Console.Write("0x" + splitData[0]);
                        Console.Write("     ");
                        // Length
                        Console.Write("0x" + splitData[1]);
                        Console.Write("        ");
                        // Data
                        if (splitData[2].Contains("#"))
                            splitData[2] = splitData[2].Replace("#", " ");
                        Console.Write(splitData[2]);
                        for (int m = splitData[2].Length; m < 30; m++)
                            Console.Write(" ");
                        // CRC
                        Console.Write(" 0x" + splitData[3]);
                        Console.WriteLine("");

                    }
                    else if (splitData.Length == 8)
                    {
                        for (int j = 0; j < 5; j = j + 4)
                        {
                            Console.Write("   ");
                            // Address
                            Console.Write("0x" + splitData[j]);
                            Console.Write("     ");
                            // Length
                            Console.Write("0x" + splitData[j + 1]);
                            Console.Write("        ");
                            // Data
                            if (splitData[2].Contains("#"))
                                splitData[2] = splitData[2].Replace("#", " ");
                            Console.Write(splitData[j + 2]);
                            for (int m = splitData[2].Length; m < 30; m++)
                                Console.Write(" ");
                            // CRC
                            Console.Write(" 0x" + splitData[j + 3]);
                            Console.WriteLine(" ");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid data !");
                    }
                }
                else
                {
                    Console.WriteLine("Data error!");
                }
            }
            Console.WriteLine(" ");
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name:  private void ParseStrToData(string Data)
        // Description: Parse data string to appropriate data bytes
        //              - Each data byte comes from a specific EPRM addresss
        //                of the ID Tag (Example: 09 is data byte)
        //-----------------------------------------------------------------------------------------------------------
        private void ParseStrToData(string Data, int tag = -1)
        {
            // Console.WriteLine(Data);
            int count1 = 0, SerialCount = 0, j = 0, remaining = 0;
            string st = "";
            for (int i = 0; i < Data.Length - 4; i++)
            {
                if (char.IsLetterOrDigit(Data[i]))
                {
                    SerialCount++;
                    if (SerialCount > 16)
                    {
                        count1++;
                        if (count1 <= 2)
                        {
                            st += Data[i];
                            if (count1 == 2)
                            {
                                if (st == "FF")
                                    remaining++;
                                if(j < 128)
                                    HexList[j] = st; 
                                j++;
                                st = "";
                                count1 = 0;
                            }
                        }
                    }
                }
            }
            if (tag > -1)
                remainedBytes[tag] = remaining;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void FillDataFields()
        // Description: - Call function to parse string to data values: one data byte
        //                contains two hex values that comes from an address of the 
        //                ID Tag EPROM ( Example: 09 is a data byte)
        //              - Identify and combine the data bytes to appropriate data types
        //                such as Barcode, DSR, and Catalog Number
        //-----------------------------------------------------------------------------------------------------------
        private void FillDataFields(string GetData, int tag)
        {
            ParseStrToData(GetData, tag - 1);
            int barcodePos = 0, DsrPos = 0, CatalogPos = 0;
            int barCodeLen = 0, DsrLen = 0, CatalogLen = 0;
            bool BarcodeDsrEnded = false, BarcodeFound = false, DsrPreceded = false;
            int crc = 0;
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
                            crcPos = crc = i + (Convert.ToInt32(HexList[i], 16)) - 1;
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
                    // Console.WriteLine(Convert.ToInt32(HexList[i], 16).ToString());
                    if (Convert.ToInt32(HexList[i], 16) != crc && HexList[i] != " ")
                    {
                        BarcodeDsrEnded = true;
                        CatalogLen = (Convert.ToInt32(HexList[i], 16));
                        CatalogPos = i + 1;
                        break;
                    }
                    
                }
            }
            //Console.WriteLine(CatalogPos.ToString());
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
                
            BarcodeList[barcodeCounter] = barcodeStr;
            DsrList[DsrCounter] = DsrStr;
            // Console.WriteLine(DsrStr);
            string DataReplaced = "";
            // Console.WriteLine(CatalogStr);
            CatalogNoList[CatalogCounter] = input.GetCatalogNumber(CatalogStr, ref DataReplaced);
            ReplacedDataCountInCatalog(DataReplaced, tag);
            // Call function to fill the appropriate data fields
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
        // Function name: bool CheckInputs(int i)
        // Description: Verify user input data -
        //              Return true if the data are valid, return false otherwise.
        //-----------------------------------------------------------------------------------------------------------
        private bool CheckInputs(int i)
        {
            string ErrString = "In Tag Number " + (i + 1).ToString() + " :";
            if ((BarcodeList[i].Length > 0) && (BarcodeBuffer[i] != BarcodeList[i]) &&
                (BarcodeList[i].Length < 6 || BarcodeList[i].Length > 7))
            {
                if (BarcodeList[i].Length == 5)
                {
                    string msg = "The 5 or less characters barcode will be padded with ";
                    msg += (BarcodeList[i].Length == 5) ? " a space. \n" : " spaces. \n";
                    msg += "Do you want to continue? (Y/N) :";
                    string response = Console.ReadLine();
                    if (response == "N" || response == "n")
                    {
                        return false;
                    }
                    else
                    {
                        for (int a = BarcodeList[i].Length; a < 6; a++)
                            BarcodeList[i] = " " + BarcodeList[i];
                    }
                }
                else
                {
                    Console.WriteLine(ErrString + "\nBarcode must be 6-7 characters long.");
                    return false;
                }
            }
            else if(!BarcodeList[i].All(c => char.IsLetterOrDigit(c)))
            {
                if(BarcodeList[i].Contains(" "))
                {
                    // The barcode is 5 characters long and
                    // it is padded with a space. -- do nothing
                }
                else
                {
                    Console.WriteLine(BarcodeList[i] + " and " + BarcodeList[i].Length.ToString());
                    Console.WriteLine(ErrString + "\nBarcode must contain upper case letters and digits only.");
                    return false;
                }
            }

            if ((DsrList[i].Length > 0) && (DsrBuffer[i] != DsrList[i]) && (DsrList[i].Length > 5))
            {
                Console.WriteLine(ErrString + "\nDSR value must contain 5 or less characters.");
                return false;
            }
            else if (!DsrList[i].All(c => char.IsLetterOrDigit(c)))
            {
                Console.WriteLine(ErrString + "\nDSR value must contain upper case letters and digits only.");
                return false;
            }
            
            if ((CatalogNoList[i].Length > 0) && (CatalogNoBuffer[i] != CatalogNoList[i]) && 
                (CatalogNoList[i].Length < 8))
            {
                Console.WriteLine(ErrString + "\nCatalog number must be 8 or more characters long.");
                return false;
            }
            else if (!CatalogNoList[i].All(c => char.IsLetterOrDigit(c)))
            {
                Console.WriteLine(ErrString + "\nCatalog number must contain capital letters and digits only.");
                return false;
            }
            return true;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: void CheckInputContents()
        // Description: Check and verify user inputs: Barcode, DSR, and Catalog number
        //-----------------------------------------------------------------------------------------------------------
        private bool CheckInputContents()
        {
            #pragma warning disable
            for (int i = 0; i <= 1; i++)
            {
                if (TagAvailable[0] == 1) 
                {
                    CheckBoxBuffer[0] = true;
                    if (CheckInputs(0) == false) break;
                }
                else
                {
                    CheckBoxBuffer[0] = false;
                }
                if (TagAvailable[1] == 1)
                {
                    CheckBoxBuffer[1] = true;
                    if (CheckInputs(1) == false) break;
                }
                else
                {
                    CheckBoxBuffer[1] = false;
                }
                if (TagAvailable[2] == 1)
                {
                    CheckBoxBuffer[2] = true;
                    if (CheckInputs(2) == false) break;
                }
                else
                {
                    CheckBoxBuffer[2] = false;
                }
                if (TagAvailable[3] == 1)
                {
                    CheckBoxBuffer[3] = true;
                    if (CheckInputs(3) == false) break;
                }
                else
                {
                    CheckBoxBuffer[3] = false;
                }
                if (TagAvailable[4] == 1)
                {
                    CheckBoxBuffer[4] = true;
                    if (CheckInputs(4) == false) break;
                }
                else
                {
                    CheckBoxBuffer[4] = false;
                }
                if (TagAvailable[5] == 1)
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


        //----------------------------------------------------------------------------------
        // Function name: private static bool SerialResponse(SerialPort serialPort)
        // Description: Send an arbitrary byte (but specific) to the Hardware device and check to see if desired 
        //              response is received.
        //----------------------------------------------------------------------------------
        private bool SerialResponse()
        {
            if (!serialPort.IsOpen)
            {
                serialPort.Open();
                Thread.Sleep(1);
                // write a byte to the serial port
                // connected to the hardware device
                serialPort.Write("z");
                // read bytes from the serial port
                int counter = 1;
                while (true)
                {
                    string response = serialPort.ReadExisting();
                    // MessageBox.Show(response);
                    if (response.Length >= 3)
                    {
                        // This is the response sent by the hardware device
                        // -- The response bytes are written to the serial port
                        //    which then are read by by the software
                        if (response.Contains("ack"))
                        {
                            // The hardware device is detected
                            serialPort.Close();
                            return true;
                        }
                    }
                    Thread.Sleep(1);
                    counter++;
                    if (counter == 400)
                    {
                        serialPort.Close();
                        return false;
                    }
                }
            }
            return false;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private string DataRequest(SerialPort serialPort);
        // Description: 1) Send request for data bytes from the ID Chips
        //              2) Receive data bytes from the ID Chips as character values and store
        //                 as a string.
        //              3) Receive the end signal of data bytes from the Hardware, and
        //                  terminate function.
        //-----------------------------------------------------------------------------------------------------------
        private void DataRequest()
        {
            serialPort = new SerialPort(PORT);
            string data = "";
            serialPortInit();
            if (!serialPort.IsOpen)
            {
                serialPort.Open();
                // Wait for a millisecond
                Thread.Sleep(1);
                // write some bytes ("port") to the serial port
                // connected to the hardware device
                serialPort.Write("r");
                while (true)
                {
                    string response = serialPort.ReadExisting();
                    // Console.WriteLine(response);
                    if (response.Length >= 1)
                    {
                        data = data + "  " + response;
                        // this.Progress.Text = ((progressBar1.Value / ProgressMax) * 100).ToString() + " %";
                        if (response.Contains("finished"))
                        {
                            // ParseSerialString(data);
                            input.ParseSerialString(data, ref DataBuffer);
                            //Console.WriteLine(data);
                            serialPort.Close();
                            return;
                        }
                    }
                    Thread.Sleep(1);
                }
            }
            return;
        }

        // Check if the input data contains lower case characters
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
        // Function name:  private void WriteData()
        // Description: Press button
        //-----------------------------------------------------------------------------------------------------------
        private void WriteData()
        {
            DataToWrite = new string[6];
            for (int i = 0; i < 6; i++)
            {
                DataToWrite[i] = "";
            }
            bool verified = CheckInputContents();
            if (verified)
                SendDataToPropeller();
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
                    // Console.WriteLine(i.ToString());
                    string tag = output.GetTag(i);
                    // An alphabetic code is assinged for erasing a data field
                    // Erase Barcode = x
                    // Erase DSR = y
                    // Erase Catalog number = z
                    string eraseFlag = "";
                    bool EraseBarcode = output.EraseData(BarcodeBuffer[i], BarcodeList[i], "Barcode", ref eraseFlag);
                    bool EraseDsr = output.EraseData(DsrBuffer[i], DsrList[i], "DSR", ref eraseFlag);
                    bool EraseCatalog = output.EraseData(CatalogNoBuffer[i], CatalogNoList[i], "Catalog-number", ref eraseFlag);
                    if (EraseBarcode || EraseDsr || EraseCatalog)
                    {
                        string warning = "Do you want to erase " + output.EraseDataFields(eraseFlag) + " \r\n";
                        warning += " from ID Tag " + (i + 1).ToString() + " ? (Y/N) :";
                        string response = Console.ReadLine();
                        if (response == "Y" || response == "n")
                        {
                            string barcode = (EraseBarcode == true) ? "x" : output.GetDataToWrite(BarcodeBuffer[i], BarcodeList[i]);
                            string dsr = (EraseDsr == true) ? "y" : output.GetDataToWrite(DsrBuffer[i], DsrList[i]);
                            string catalog; 
                            if (barcode.Length / 2 >= 7)
                                catalog = output.GetDataToWrite(CatalogNoBuffer[i], CatalogNoList[i], 1, DataReplacedCount[i]);
                            else if (dsr.Length / 2 >= 3 && dsr.Length / 2 <= 7)
                                catalog = output.GetDataToWrite(CatalogNoBuffer[i], CatalogNoList[i], 1, DataReplacedCount[i]);
                            else
                                catalog = (EraseCatalog == true) ? "z" : output.GetDataToWrite(CatalogNoBuffer[i], 
                                    CatalogNoList[i],0, DataReplacedCount[i]);
                            DataToWrite[i] = tag + barcode + dsr + catalog;
                            if ((barcode + dsr + catalog).Length > remainedBytes[i])
                            {
                                MemoryEnough = false;
                                Console.WriteLine("No enough memory in ID tag " + (i + 1).ToString() + ".");
                                break;
                            }
                        }
                    }
                    else
                    {
                        string barcode = output.GetDataToWrite(BarcodeBuffer[i], BarcodeList[i]);
                        string dsr = output.GetDataToWrite(DsrBuffer[i], DsrList[i]);
                        string catalog = output.GetDataToWrite(CatalogNoBuffer[i], CatalogNoList[i], (barcode + dsr).Length, DataReplacedCount[i]);
                        DataToWrite[i] = tag + dataToWrite(barcode, dsr, catalog, BarcodeList[i], DsrList[i], CatalogNoList[i]);
                        if ((DataToWrite[i].Length  - 1)/2 > remainedBytes[i])
                        {
                            MemoryEnough = false;
                            Console.WriteLine("No enough memory in ID tag " + (i + 1).ToString() + ".");
                            break;
                        }
                    }
                    // Mark data types
                    MarkDataTypes(i, tag);
                }
            }
            if (MemoryEnough)
            {
                if (output.DataFieldChanged(DataToWrite))
                {
                    WriteDataToSerialPort(DataToWrite);
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name:  private void MarkDataTypes(int i, string tag)
        // Description: 1) output and mark the starting address, length, and/or ending address 
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
        //          if the end address is 00, the end byte will be outputd in the
        //          hardware by reading data from the EPROM starting at address B0 until data byte reads "FF".
        //-----------------------------------------------------------------------------------------------------------
        private void MarkDataTypes(int i, string tag)
        {
            string bar_x = "", dsr_y = "", cat_z = "", data_bytes = "";
            string BYTES = DataToWrite[i];
            for (int a = 1; a < BYTES.Length; a++)
            {
                if (BYTES[a] == 'x')
                    bar_x = "x" + output.GetDataPosLen(TagsDataBuffer[i], "Barcode");
                else if (BYTES[a] == 'y')
                    dsr_y = "y" + output.GetDataPosLen(TagsDataBuffer[i], "DSR");
                else if (BYTES[a] == 'z')
                    cat_z = "z" + output.GetDataPosLen(TagsDataBuffer[i], "Catalog");
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
                    data_bytes = output.GetDataPosLen(TagsDataBuffer[i], "Zap-Barcode") + data_bytes;
                else if (Convert.ToInt32(type, 16) >= 3 && Convert.ToInt32(type, 16) <= 7)
                    data_bytes = output.GetDataPosLen(TagsDataBuffer[i], "Zap-DSR") + data_bytes;
                else if (Convert.ToInt32(type, 16) >= 10 && Convert.ToInt32(type, 16) <= 47)
                    data_bytes = output.GetDataPosLen(TagsDataBuffer[i], "Zap-Catalog") + data_bytes;
                
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
                Console.WriteLine("Hardware is diconnected \n Please, connect the hardware and try again.");
            }
            else
            {
                serialPort = new SerialPort(PORT);
                serialPortInit();
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                    // Wait for a millisecond
                    Thread.Sleep(1);
                    // Send write command to hardware
                    serialPort.Write("w");
                    while (true)
                    {
                        string response = serialPort.ReadExisting();
                        if (response.Contains("ack"))
                        {
                            // Console.WriteLine(response); // For debugging only
                            int msg = 1;
                            foreach (string data in DataBytes)
                            {
                                if (data.Length > 3)
                                {
                                    Console.WriteLine("Tag " + msg.ToString() + ": " + data);
                                    foreach (char c in data)
                                        serialPort.Write(c.ToString());
                                }
                                msg++;
                            }
                            Thread.Sleep(1);
                            // Send the end signal
                            serialPort.Write("q");
                            Thread.Sleep(1);
                            // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                            // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                            // For Debugging porpose only
                            while (true)
                            {
                                string get = serialPort.ReadExisting();
                                Console.WriteLine(get);
                                if (get.Contains("ended"))
                                {
                                    Console.WriteLine("ended");
                                    break;
                                }       
                            }
                            // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                            // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                            break;
                        }
                        Thread.Sleep(1);
                    }
                    serialPort.Close();
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name:private string dataToWrite(string barcode, string dsr, string catalog, string currentBarcode,
        //                                          string currentDsr, string currentCatalog)
        // Description: Determine and return data bytes to write into EPROM of an ID Tag
        //-----------------------------------------------------------------------------------------------------------
        private string dataToWrite(string barcode, string dsr, string catalog, string currentBarcode,
                                                             string currentDsr, string currentCatalog)
        {
            int[] array = { 1, 2 }; // dummy initialization
            if (barcode.Length > 0)
            {
                if (currentDsr.Length > 0)
                    barcode += output.CharToHex(currentDsr, ref array) + _crc.crc8(array);
                if (currentCatalog.Length > 0)
                    barcode += output.CharToHex(currentCatalog, ref array) + _crc.crc8(array);
                return barcode;
            }
            else
            {
                if (dsr.Length > 0)
                {
                    if (currentCatalog.Length > 0)
                        dsr += output.CharToHex(currentCatalog, ref array) + _crc.crc8(array);
                    return dsr;
                }
            }
            return catalog;
        }

        //----------------------------------------------------------------------------------
        // Function name: private static void PortAutoDetect(SerialPort serialPort)
        // Description: Detect the USB port that the Hardware device is connected to.
        //----------------------------------------------------------------------------------
        private void PortAutoDetect()
        {
            // port auto detection  ---- 
            string[] ports = SerialPort.GetPortNames();
            // PortList();
            for (int i = 0; i < ports.Length; i++)
            {
                string port = ports[i];
                serialPort = new SerialPort(port);
                serialPortInit();
                if (!serialPort.IsOpen)
                {
                    if (SerialResponse() == true)
                    {
                        portDetected = true;
                        portName = port;
                        PORT = port;
                        break;
                    }
                }
                serialPort.Close();
                // Delay before going back to check the next port
                Thread.Sleep(50);
            }
            if (ports.Contains(portName) == false)
            {
                portDetected = false;
            }
        }
    }
}
