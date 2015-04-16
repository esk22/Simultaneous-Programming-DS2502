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
// Author                  : Arun Rai
// Date                    : 02/20/2015
// Reviewed by             : 
// Description: This software allows a user to read and program multiple DS2502 (aka ID Tags), upto 6 devices, 
//              through a Command Line Interface.
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
    class Program
    {
        // Current data 
        // after user makes changes
        private static string[] UserChangedBarcode;
        private static string[] UserChangedDSR;
        private static string[] UserChangedCatalog;
        // Data when loaded from the ID Chips
        private static string[] BarcodeBuffer;
        private static string[] DsrBuffer;
        private static string[] CatalogBuffer;
        private static string[] ExtraDsr;
        // Data as string
        private static string[] HexList;
        // Data buffer
        private static string[] TagsDataBuffer;
        // Other buffers
        private static bool[] IDChipExist;
        private static string[] DataRecord;
        private static string[] DataToWrite;
        private static string[] DataBuffer;
        private static int[]TagAvailable;
        private static int[] DataReplacedCount;
        private static int[] remainedBytes;
        private static string PORT;
        private static bool ReadingComplete;
        private static bool portDetected;
        private static string portName;

        DataIn input;
        DataOut output;
        crc _crc;

        SerialPort serialPort;
        
        // Main function
        static void Main(string[] args)
        {
            Program program = new Program();
            program.ProcessData(args);
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: public void  ProcessData(string portName, string[] InBytes)
        // Description: process data
        //-----------------------------------------------------------------------------------------------------------
        private void ProcessData(string [] args)
        {
            // Instantiate classes
            input = new DataIn();
            output = new DataOut();
            _crc = new crc();
            ReadingComplete = true;
            portDetected = false;
            int tag = 0;
            NewArrays();
            InitArrays();
            if (args.Length == 2)
            {
                if (args[0] == "Read" || args[0] == "read" || args[0] == "READ")
                {
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
                            tag = 0; // all tags
                            Read(tag, display);
                        }
                        else
                        {
                            Console.WriteLine("Error: invalid read command.");
                        }
                    }
                }
            }
            else if (args.Length == 1)
            {
                if (args[0] == "write" || args[0] == "Write" || args[0] == "WRITE") // write commmand
                {
                    if (Read(0, "all", "write"))
                    {
                        for (int m = 0; m < TagsDataBuffer.Length; m++)
                        {
                            if (TagsDataBuffer[m].Length > 1)
                            {
                                ComputeDataFields(TagsDataBuffer[m], m);
                                IDChipExist[m] = true;
                            }
                            else
                            {
                                IDChipExist[m] = false;
                            }
                        }

                        int tags = 0;
                        string AvailableChips = AvailableIDChips(ref tags);
                        if (tags == 1)
                            Console.WriteLine("\nID Chip " + AvailableChips + " is available for programming.");
                        else if (tags > 1)
                            Console.WriteLine("\nID Chips " + AvailableChips + " are available for programming.");

                        bool verified = false;
                        string number = "";
                        int tagId = 0;
                        Console.Write("Number of ID Chips to program : ");
                        while (!verified)
                        {
                            number = Console.ReadLine();
                            if (IsTagIDValid(number, ref tagId))
                            {
                                verified = true;
                            }
                            else
                            {
                                Console.Write("Enter the number of ID Chips : ");
                            }
                        }
                        WriteCommand(tagId);
                    }
                }
            }
            else if (args.Length > 2)
            {
                if (args[0] == "erase" || args[0] == "Erase" || args[0] == "ERASE") // erase commmand
                {
                    if ((args.Length % 2) == 1)
                    {
                        // Errase command recognized.
                        // => erase tagx data type(s) - separated by '\' [ x => {1, 6} ]
                        // Example:
                        // $ program.exe erase 1 b\d\c, 2 b, 3 d\c
                        // b -> Barcode, d -> D.S.R, c -> Catalog number
                        ErraseCommand(args);
                    }
                    else
                    {
                        // Command is incomplete or has some input error.
                        Console.WriteLine("Error: unrecognized error");
                    }
                }
            }
            else
            {
                Console.WriteLine("Error: no input command.");
            }
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private string AvailableIDChips(ref int count)
        // Description: Return the number of ID Chips available for programming
        //-----------------------------------------------------------------------------------------------------------
        private string AvailableIDChips(ref int count)
        {
            string tags = "";
            for (int i = 0; i < TagsDataBuffer.Length; i++)
            {
                if (TagsDataBuffer[i].Length > 0)
                {
                    if (i == TagsDataBuffer.Length - 1)
                    {
                        tags += " and " + (i + 1).ToString();
                    }
                    else if (count > 0 && i < (TagsDataBuffer.Length - 1))
                    {
                        tags += ", " + (i + 1).ToString();
                    }
                    else
                    {
                        tags += (i + 1).ToString();
                    }
                    count++;
                }
            }
            return tags;
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
            UserChangedBarcode = new string[6];
            UserChangedDSR = new string[6];
            UserChangedCatalog = new string[6];
            ExtraDsr = new string[6];
            HexList = new string[128];
            BarcodeBuffer = new string[6];
            DsrBuffer = new string[6];
            CatalogBuffer = new string[6];
            IDChipExist = new bool[6];
            TagAvailable = new int[6];
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void InitArrays()
        // Description: Initialize arrays
        //-----------------------------------------------------------------------------------------------------------
        private void InitArrays()
        {
            for (int i = 0; i < 3; i++)
                DataRecord[i] = "a";
            for (int j = 0; j < 6; j++)
            {
                remainedBytes[j] = 127;
                TagAvailable[j] = 0;
                TagsDataBuffer[j] = "";
                BarcodeBuffer[j] = "";
                DsrBuffer[j] = "";
                CatalogBuffer[j] = "";
                ExtraDsr[j] = "";
            }
            for (int m = 0; m <= 127; m++)
                HexList[m] = "FF";
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void WriteCommand(int chips)
        // Description: Excecuate user entered write command
        //-----------------------------------------------------------------------------------------------------------
        private void WriteCommand(int chips)
        {
            int chip = 1;
            while (chips-- > 0)
            {
                bool error = true;
                while (error)
                {
                    Console.Write("\n" + chip.ToString() + ") Enter command : ");
                    string command = Console.ReadLine();
                    Console.WriteLine(command);
                    if (IsWriteCommandValid(command))
                    {
                         Console.WriteLine("Command is valid");
                         error = false;
                    }
                }
                chip++;
            }

            for (int i = 0; i < 6; i++)
            {
                Console.Write("Tag : " + (i + 1).ToString() + " " + UserChangedBarcode[i] + "  " + UserChangedDSR[i] + "  " + UserChangedCatalog[i]);
                Console.WriteLine("  extra dsr " + ExtraDsr[i]);
            }
            // Write command validated
            // Execuate writing operation
           ExecuateCommand();
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private bool IsWriteCommandValid(string command)
        // Description: Validate write command, return true or false
        //-----------------------------------------------------------------------------------------------------------
        private bool IsWriteCommandValid(string command)
        {
            string[] CmdArray = command.Split(' ');
            bool InputIsValid = true;
            int tag = 0;
            if(IsTagIDValid(CmdArray[0], ref tag))
            {
                if (IDChipExist[tag - 1])
                {
                    for (int a = 1; a < CmdArray.Length; a++)
                    {
                        if (a == 1)
                        {
                            if ((CmdArray.Length <= 7) && (!CmdArray[a].Contains('|')))
                            {
                                if (!IsBarcodeValid(CmdArray[a], tag - 1))
                                {
                                    InputIsValid = false;
                                    break;
                                }
                            }
                            else if (CmdArray[a].Length > 1 && CmdArray[a].Contains('|'))
                            {
                                if (CmdArray[a].Contains('|'))
                                {
                                    if (!IsDsrValid(CmdArray[a], tag - 1))
                                    {
                                        Console.WriteLine("hello where");
                                        InputIsValid = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (!IsBarcodeValid(CmdArray[a], tag - 1))
                                    {
                                        InputIsValid = false;
                                        break;
                                    }
                                }
                            }
                            else if(CmdArray[a].Length > 8)
                            {
                                Console.WriteLine("hello where " + CmdArray[a].Length.ToString() + " " + CmdArray[a]);
                                if (!IsCatalogValid(CmdArray[a], tag - 1))
                                {
                                    InputIsValid = false;
                                    break;
                                }
                            }
                        }
                        if (a == 2)
                        {
                            if (CmdArray[a].Length > 7 && (!CmdArray[a].Contains('|')))
                            {
                                if (!IsCatalogValid(CmdArray[a], tag - 1))
                                {
                                    InputIsValid = false;
                                    break;
                                }
                            }
                            else
                            {
                                if (!IsDsrValid(CmdArray[a], tag - 1))
                                {
                                    InputIsValid = false;
                                    break;
                                }
                            }
                        }
                        if (a == 3)
                        {
                            if (!IsCatalogValid(CmdArray[a], tag - 1))
                            {
                                InputIsValid = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    // ID Chip does not exist.
                    Console.WriteLine("Error: ID Chip " + tag.ToString() + " does not exist.");
                    return false;
                }
            }
            if (InputIsValid)
                return true;
            return false;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private bool IsCatalogValid(string catalog, int tag)
        // Description: Validate user entered Catalog number, return true or false
        //              If the Catalog number is valide, place it in the buffer.
        //-----------------------------------------------------------------------------------------------------------
        private bool IsCatalogValid(string catalog, int tag)
        {
            bool IsCatalongNotValid = true;
            while (IsCatalongNotValid)
            {
                if (catalog.Length > 7)
                {
                    if (catalog.All(c => Char.IsLetterOrDigit(c)))
                    {
                        UserChangedCatalog[tag] = catalog.ToUpper();
                        IsCatalongNotValid = false;
                        return true;
                    }
                    else
                    {
                        Console.WriteLine(catalog + " and " + catalog.Length.ToString());
                        Console.WriteLine("\nError: in ID Chip " + (tag + 1).ToString() + " the Catalog number must catain upper case letters and digits only.");
                        Console.Write("Enter the Catalog number again : ");
                        catalog = Console.ReadLine();
                    }
                }
                else
                {
                    Console.WriteLine("\nError: in ID Chip " + (tag + 1).ToString() + " the Catalog number must be 8 or more characters long.");
                    Console.Write("Enter the Catalog number again : ");
                    catalog = Console.ReadLine();
                }
            }
            return false;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private bool IsDsrValid(string dsr, int tag)
        // Description: Validate user entered DSR, return true or false
        //              If the DSR is valide, place it in the buffer.
        //-----------------------------------------------------------------------------------------------------------
        private bool IsDsrValid(string dsr, int tag)
        {
            bool IsDsrNotValid = true, extraDsr = false;
            string dsrs = "";
            while (IsDsrNotValid)
            {
                string[] DsrArray = dsr.Split('|');
                int a = DsrArray.Length - 1;
                int DsrCount = 0;
                bool error = false;
                while (a >= 0 && !error)
                {
                    if (DsrArray[a].Length < 6 && DsrArray[a].Length > 0)
                    {
                        if (DsrArray[a].All(c => Char.IsLetterOrDigit(c)))
                        {
                            DsrCount++;
                            //if (a == DsrArray.Length - 1)
                            if (DsrCount == 1)
                            {
                                UserChangedDSR[tag] = DsrArray[a].ToUpper();
                            }
                            else
                            {
                                int[] array = { 1, 2, 3 };
                                DsrArray[a] = DsrArray[a].ToUpper();
                                dsrs = output.CharToHex(DsrArray[a], ref array) + _crc.crc8(array) + dsrs;
                                extraDsr = true;
                            }
                            if (a == 0)
                                IsDsrNotValid = false;
                        }
                        else
                        {
                            Console.WriteLine(DsrArray[a] + " Deubg");
                            Console.WriteLine("\nError: in ID Chip " + (tag + 1).ToString() + " the DSR must catain upper case letters and digits only.");
                            Console.Write("Enter the DSR again : ");
                            dsr = Console.ReadLine();
                            error = true;
                        }
                    }
                    else if (DsrArray[a].Length > 6)
                    {
                        Console.WriteLine(DsrArray[a] + " Deubg");
                        Console.WriteLine("\nError: in ID Chip " + (tag + 1).ToString() + " the DSR must be 5 or less characters long.");
                        Console.Write("Enter the DSR again : ");
                        dsr = Console.ReadLine();
                        error = true;
                    }
                    a--;
                }
            }
            if (extraDsr)
                ExtraDsr[tag] = dsrs;
            if (!IsDsrNotValid)
                return true;
            return false;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private bool IsBarcodeValid(string barcode, int tag)
        // Description: Validate user entered barcode, return true or false
        //              If the barcode is valide, place it in the buffer.
        //-----------------------------------------------------------------------------------------------------------
        private bool IsBarcodeValid(string barcode, int tag)
        {
            bool IsBarcodNotValid = true;
            while (IsBarcodNotValid)
            {
                if (barcode.Length == 6 || barcode.Length == 7)
                {
                    if (barcode.All(c => Char.IsLetterOrDigit(c)))
                    {
                        UserChangedBarcode[tag] = barcode.ToUpper();
                        IsBarcodNotValid = false;
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("\nError: in ID Chip " + (tag + 1).ToString() + " the barcode must catain upper case letters and digits only.");
                        Console.Write("Enter the barcode again : ");
                        barcode = Console.ReadLine();
                    }
                }
                else if (barcode.Length > 0 && barcode.Length < 6)
                {
                    Console.WriteLine("\nIn ID Chip " + (tag + 1).ToString() + " the barcode with 5 or less characters will be padded with leading space(s).");
                    Console.Write("Do you want to continue ? (y/n) : ");
                    string response = Console.ReadLine();
                    if (response == "y")
                    {
                        barcode = barcode.ToUpper();
                        for (int i = barcode.Length; i < 6; i++)
                            barcode = " " + barcode;
                        UserChangedBarcode[tag] = barcode;
                    }
                    IsBarcodNotValid = false;
                    return true;
                }
                else
                {
                    Console.WriteLine("\nError: in ID Chip " + (tag + 1).ToString() + " the barcode must be 6 or 7 characters long.");
                    Console.Write("Enter the barcode again : ");
                    barcode = Console.ReadLine();
                }
            }
            return false;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void Read(int tag, string display)
        // Description: Read data bytes from the ID Chip(s)
        //-----------------------------------------------------------------------------------------------------------
        private bool Read(int tag, string display, string operation = "read")
        {
            Console.WriteLine(" \n Searching for hardware...............................");
            Console.WriteLine("\n|***************************************************************************|");
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
                    if (operation == "read")
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
                }
                else
                {
                    Console.WriteLine(" No ID Chip recognized.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("No Hardware is detected.");
                return false;
            }
            return true;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void MarkEraseType(char c, ref string type)
        // Description: Mark data types
        // Data Type        Mark
        // Barcode           x
        // D.S.R             y
        // Catalog number    z
        //-----------------------------------------------------------------------------------------------------------
        private void MarkEraseType(char c, ref string type)
        {
            switch(c)
            {
                case('b'):
                    type += 'x';
                    break;
                case ('B'):
                    type += 'x';
                    break;
                case ('c'):
                    type += 'z';
                    break;
                case ('C'):
                    type += 'z';
                    break;
                case ('d'):
                    type += 'y';
                    break;
                case ('D'):
                    type += 'y';
                    break;
                default:
                    break;
                    // None
            }
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private bool IsDataTypeValid(string type, int tag, ref string erase_types)
        // Description: Validate the user input data type(s)
        //-----------------------------------------------------------------------------------------------------------
        private bool IsDataTypeValid(string type, int tag, ref string erase_types)
        {
            if (type.Length > 0 && type.Length < 6)
            {
                foreach (char c in type)
                {
                    if (Char.IsLetter(c))
                    {
                        if (c == 'b' || c == 'c' || c == 'd' || c == 'B' || c == 'C' || c == 'D')
                        {
                            MarkEraseType(c, ref erase_types);
                        }
                        else
                        {
                            Console.WriteLine("Error: invalid data type(s) in Tag ID " + tag.ToString());
                            return false;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Error: invalid data type(s) in Tag ID " + tag.ToString());
            }
            return true;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private bool IsTagIDValid(string Tag, ref int tagNumber)
        // Description: Validate the user input ID Tag number(s)
        //-----------------------------------------------------------------------------------------------------------
        private bool IsTagIDValid(string Tag, ref int tagNumber)
        {
            int TagID = 0;
            bool error = false;
            if (Tag.Length == 1)
            {
                try
                {
                    TagID = Convert.ToInt32(Tag);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Error:Tag ID number must be a digit.");
                    return false;
                }
                catch (OverflowException)
                {
                    Console.WriteLine("Error: unrecognized Tag ID Number.");
                    return false;
                }
                finally
                {
                    if (TagID > 0 && TagID < 7)
                    {
                        tagNumber = TagID;
                        error = false;
                    }
                    else
                    {
                        if (!Char.IsLetter(Tag[0]))
                        {
                            Console.WriteLine("Error: ID Tag number must be between 1 and 6.");
                            error = true;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Error: ID Tag number must be a single digit.");
            }

            if (!error)
                return true;
            return false;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void ErraseCommand(string[] command)
        // Description: Validate user input errase command(s)
        //-----------------------------------------------------------------------------------------------------------
        private void ErraseCommand(string[] command)
        {
            string[] Commands = new string[6];
            for (int a = 0; a < 6; a++)
                Commands[a] = "";
            bool Error = false;
            for (int a = 1; a < command.Length; a += 2)
            {
                int TagID = 0;
                if (IsTagIDValid(command[a], ref TagID))
                {
                    if (!IsDataTypeValid(command[a + 1], TagID, ref Commands[TagID - 1]))
                    {
                        Error = true;
                        break;
                    }
                }
                else
                {
                    Error = true;
                    break;
                }
            }
            if (!Error)
            {
                ExecuateErraseCommands(Commands);
            }
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name:  private void ExecuateErraseCommands(string[] Commands)
        // Description: Implement errase commands
        //-----------------------------------------------------------------------------------------------------------
        private void ExecuateErraseCommands(string[] Commands)
        {
            if (Read(0, "all", "erase"))
            {
                for (int m = 0; m < TagsDataBuffer.Length; m++)
                {
                    if (TagsDataBuffer[m].Length > 1)
                    {
                        ComputeDataFields(TagsDataBuffer[m], m);
                        IDChipExist[m] = true;
                    }
                    else
                    {
                        IDChipExist[m] = false;
                    }
                    if (Commands[m].Length >= 1)
                    {
                        // Console.WriteLine(Commands[m] + " and " + m.ToString());
                        foreach (char c in Commands[m])
                        {
                            if (c == 'x')
                                UserChangedBarcode[m] = "";
                            if (c == 'y')
                                UserChangedDSR[m] = "";
                            if (c == 'z')
                                UserChangedCatalog[m] = "";
                        }
                    }
                }
                bool NoChip = false;
                for (int n = 0; n < TagsDataBuffer.Length; n++)
                {
                    if (TagsDataBuffer[n].Length < 1 && Commands[n].Length >= 1)
                    {
                        Console.WriteLine("Error: ID Tag " + (n + 1).ToString() + " does not exist.");
                        NoChip = true;
                        break;
                    }
                }
                if(!NoChip)
                    ExecuateCommand();
            }
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name:  private void ExecuateCommand(string[] Commands)
        // Description: Execuate command
        //-----------------------------------------------------------------------------------------------------------
        private void ExecuateCommand()
        {
            DataToWrite = new string[6];
            for (int i = 0; i < 6; i++)
            {
                DataToWrite[i] = "";
            }
            // This function sends commands to the Hardware
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
            for (int i = 0; i < IDChipExist.Length; i++)
            {
                if (IDChipExist[i])
                {
                    // Console.WriteLine(i.ToString());
                    string tag = output.GetTag(i);
                    // An alphabetic code is assinged for erasing a data field
                    // Erase Barcode = x
                    // Erase DSR = y
                    // Erase Catalog number = z
                    string eraseFlag = "";
                    // Console.WriteLine(BarcodeBuffer[i] + " " + UserChangedBarcode[i]);
                    bool EraseBarcode = output.EraseData(BarcodeBuffer[i], UserChangedBarcode[i], "Barcode", ref eraseFlag);
                    bool EraseDsr = output.EraseData(DsrBuffer[i], UserChangedDSR[i], "DSR", ref eraseFlag);
                    bool EraseCatalog = output.EraseData(CatalogBuffer[i], UserChangedCatalog[i], "Catalog-number", ref eraseFlag);
                    // Console.WriteLine(i.ToString());
                    if (EraseBarcode || EraseDsr || EraseCatalog)
                    {
                        
                        string warning = "Do you want to erase " + output.EraseDataFields(eraseFlag) + " \r\n";
                        warning += "from ID Tag " + (i + 1).ToString() + " ? (y/n) : ";
                        Console.Write(warning);
                        string response = Console.ReadLine();
                        string barcode = "", dsr = "", catalog = "";
                        if (response == "y")
                        {
                            barcode = (EraseBarcode == true) ? "x" : output.GetDataToWrite(BarcodeBuffer[i], UserChangedBarcode[i]);
                            dsr = (EraseDsr == true) ? "y" : output.GetDataToWrite(DsrBuffer[i], UserChangedDSR[i]);
                            if (barcode.Length / 2 >= 7)
                                catalog = output.GetDataToWrite(CatalogBuffer[i], UserChangedCatalog[i], 1, DataReplacedCount[i]);
                            else if (dsr.Length / 2 >= 3 && dsr.Length / 2 <= 7)
                                catalog = output.GetDataToWrite(CatalogBuffer[i], UserChangedCatalog[i], 1, DataReplacedCount[i]);
                            else
                                catalog = (EraseCatalog == true) ? "z" : output.GetDataToWrite(CatalogBuffer[i],
                                    UserChangedCatalog[i], 0, DataReplacedCount[i]);
                        }

                        if (ExtraDsr[i].Length >= 3)
                            dsr = ExtraDsr[i] + dsr;
                        DataToWrite[i] = tag + barcode + dsr + catalog;
                        if ((barcode + dsr + catalog).Length > remainedBytes[i])
                        {
                            MemoryEnough = false;
                            Console.WriteLine("No enough memory in ID tag " + (i + 1).ToString() + ".\n");
                            break;
                        }
                    }
                    else
                    {
                        string barcode = output.GetDataToWrite(BarcodeBuffer[i], UserChangedBarcode[i]);
                        string dsr = output.GetDataToWrite(DsrBuffer[i], UserChangedDSR[i]);
                        string catalog = output.GetDataToWrite(CatalogBuffer[i], UserChangedCatalog[i], (barcode + dsr).Length, DataReplacedCount[i]);
                        DataToWrite[i] = tag + dataToWrite(barcode, dsr, catalog, UserChangedBarcode[i], UserChangedDSR[i], UserChangedCatalog[i], ExtraDsr[i]);
                        if ((DataToWrite[i].Length - 1) / 2 > remainedBytes[i])
                        {
                            MemoryEnough = false;
                            Console.WriteLine("No enough memory in ID tag " + (i + 1).ToString() + ".\n");
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
            // output = new DataOut();
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
                                Thread.Sleep(1);
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
                            Thread.Sleep(2);
                            // Console.WriteLine(" debuging ");
                            // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                            // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                            // For Debugging porpose only
                            while (true)
                            {
                                string get = serialPort.ReadExisting();
                                if(get.Contains("Tag Number"))
                                    Console.WriteLine("");
                                Console.Write(get);
                                //string str = DebugStr(get);
                                //if (str.Length > 1)
                                  //  Console.Write(str);
                                if (get.Contains("ended"))
                                {
                                    Console.WriteLine("");
                                    break;
                                }
                                Thread.Sleep(1);
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

        private string DebugStr(string str)
        {
            string output = "";
            foreach (char c in str)
            {
                if (c != ' ')
                {
                    output += c;
                }
            }
            return output;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name:private string dataToWrite(string barcode, string dsr, string catalog, string currentBarcode,
        //                                          string currentDsr, string currentCatalog)
        // Description: Determine and return data bytes to write into EPROM of an ID Tag
        //-----------------------------------------------------------------------------------------------------------
        private string dataToWrite(string barcode, string dsr, string catalog, string currentBarcode,
                                  string currentDsr, string currentCatalog, string extraDsr)
        {
            int[] array = { 1, 2 }; // dummy initialization
            if (barcode.Length > 0)
            {
                // Console.WriteLine("Extra DSR print debug : " + dsr);
                if (extraDsr.Length >= 3)
                {
                    barcode += extraDsr;
                }
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
                    {
                        dsr += output.CharToHex(currentCatalog, ref array) + _crc.crc8(array);
                    }
                    return dsr;
                }
            }
            return catalog;
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
                    TagsDataBuffer[i - 1] = input.SpaceDelimitor(input.RemoveExtraWords(IDTagData[i - 1]));
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
            Console.WriteLine(" ID Chip: " + i.ToString());
            Console.WriteLine(" ");
            // Parse data string to get Record List
            input.GetRecordList(TagsDataBuffer[i - 1], ref DataRecord);
            PrintSerialChipNumber(TagsDataBuffer[i - 1]);
            // Print tecord Texts
            PrintDataRecord(DataRecord);
            // Print data bytes
            PrintBytes(TagsDataBuffer[i - 1], i);
            Console.WriteLine("");
            Console.WriteLine("|***************************************************************************|");
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private static void serialPortInit(SerialPort serialPort)
        // Description: Serial Port intialization
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
        // Function name: private void PrintSerialChipNumber(string Data)
        // Description: Print the ID Chip Serial Number
        //-----------------------------------------------------------------------------------------------------------
        private void PrintSerialChipNumber(string Data)
        {
            int SerialCount = 0;
            string SerialNumber = "";
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
                        break;
                    }
                }
            }
            Console.WriteLine(" Chip Serial No.: " + SerialNumber);
            Console.WriteLine(" ");
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: private void PrintBytes(string Data)
        // Description: Print the ID Chip EPROM contents as hex.
        //              Each data byte contains two hex values, and each data byte is
        //              stored in an EPROM address of the ID Tag. 
        //-----------------------------------------------------------------------------------------------------------
        private void PrintBytes(string Data, int tag = -1)
        {
            string coloumn = "  |  Addrs 0x00";
            for (int a = 1; a < 8; a++)
                coloumn += "  0x" + a.ToString("X2");
            int used = 0, remaining = 0;
            int count1 = 0, count2 = 0, SerialCount = 0, AddCounter = 0;
            string st = "", DataRow = "  |  ";
            bool DisplayColoumn = true;
            DataRow += "0x00";
            DataRow += "   ";
            for (int i = 0; i < Data.Length - 3; i++)
            {
                if (char.IsLetterOrDigit(Data[i]))
                {
                    SerialCount++;
                    if(SerialCount > 16)
                    {
                        if (DisplayColoumn)
                        {
                            Console.WriteLine(" => Hex Data: ");
                            Console.WriteLine(" ");
                            Console.WriteLine("  |-------------------------------------------------------|");
                            Console.WriteLine(coloumn + " |");
                            DisplayColoumn = false;
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
                                if(count2 < 8)
                                    DataRow += "    "; 
                                st = "";
                                if (count2 == 8)
                                {
                                    Console.WriteLine(DataRow + "  |");
                                    DataRow = "  |  0x" + AddCounter.ToString("X2");
                                    // Console.WriteLine("");
                                    DataRow += "   ";
                                    count2 = 0;
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("  |-------------------------------------------------------|");
            PrintUsedRemainingBytes(used, remaining);
        }

        // Print used and remaining data bytes
        private void PrintUsedRemainingBytes(int used, int remaining)
        {
            string usedStr = (used > 9) ? used.ToString() + " bytes used" : used.ToString() + " byte used";
            string remainStr = (remaining > 9) ? remaining.ToString() + " bytes remaining" : remaining.ToString() + " byte remaining";
            Console.WriteLine(" ");
            Console.WriteLine("            " + usedStr + " (" + remainStr + ")");
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name:  private void PrintRecordTextBar(string[] DataRecord)
        // Description: Print coloumn bar
        //-----------------------------------------------------------------------------------------------------------
        private void PrintRecordTextBar(string[] DataRecord)
        {
            Console.WriteLine("");
            Console.WriteLine(" => Record List: ");
            Console.WriteLine("");
            Console.WriteLine("  |--------|-----------|------------------------------|------|");
            //Console.WriteLine("  |========|===========|=====================================|");
            Console.WriteLine("  | Addr   | Length    | Record Text                  | CRC  |");
            Console.WriteLine("  |--------|-----------|------------------------------|------|");
            //Console.WriteLine("  |========|===========|=====================================|");
            //Console.WriteLine("  |        |           |                                     |");
        }

        private void DataRecordText(string[] splitData, int j)
        {
            Console.Write("  | ");
            // Address
            Console.Write("0x" + splitData[j]);
            Console.Write("   | ");
            // Length
            Console.Write("0x" + splitData[j + 1]);
            Console.Write("      | ");
            // Data
            if (splitData[2].Contains("#"))
                splitData[2] = splitData[2].Replace("#", " ");
            Console.Write(splitData[j + 2]);
            for (int m = splitData[2].Length; m < 29; m++)
                Console.Write(" ");
            // CRC
            Console.Write("| 0x" + splitData[j + 3]);
            Console.Write(" |");
            Console.WriteLine(" ");
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name:  private void PrintDataRecord(string[] DataRecord)
        // Description: Print data record 
        //-----------------------------------------------------------------------------------------------------------
        private void PrintDataRecord(string[] DataRecord)
        {
            if(DataRecord.Length > 0)
                PrintRecordTextBar(DataRecord);
            for (int i = 0; i < DataRecord.Length; i++)
            {
                if (DataRecord.Length > 0)
                {
                    string[] splitData = DataRecord[i].Split(' ');
                    if (splitData.Length == 4)
                    {
                        DataRecordText(splitData, 0);
                    }
                    else if (splitData.Length == 8)
                    {
                        for (int j = 0; j < 5; j = j + 4)
                        {
                            DataRecordText(splitData, j);
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
            Console.WriteLine("  |--------|-----------|------------------------------|------|");
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
            ParseStrToData(GetData, tag);
            int barcodePos = 0, DsrPos = 0, CatalogPos = 0;
            int barCodeLen = 0, DsrLen = 0, CatalogLen = 0;
            // bool BarcodeDsrEnded = false, BarcodeFound = false, DsrPreceded = false;
            int crc = 0;
            ComputeDatalenPos(ref barcodePos, ref DsrPos, ref CatalogPos, ref barCodeLen, ref DsrLen, ref CatalogLen, ref crc);
            //Console.WriteLine(CatalogPos.ToString());
            string barcodeStr = "", DsrStr = "", CatalogStr = "";
            // int barcodeCounter = 0, DsrCounter = 0, CatalogCounter = 0;
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
            BarcodeBuffer[tag] = barcodeStr;
            UserChangedBarcode[tag] = barcodeStr;
            DsrBuffer[tag] = DsrStr;
            UserChangedDSR[tag] = DsrStr;
            // Console.WriteLine(DsrStr);
            string DataReplaced = "";
            // Console.WriteLine(CatalogStr);
            CatalogBuffer[tag] = input.GetCatalogNumber(CatalogStr, ref DataReplaced);
            UserChangedCatalog[tag] = CatalogBuffer[tag];
            ReplacedDataCountInCatalog(DataReplaced, tag + 1);
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
            if ((UserChangedBarcode[i].Length > 0) && (BarcodeBuffer[i] != UserChangedBarcode[i]) &&
                (UserChangedBarcode[i].Length < 6 || UserChangedBarcode[i].Length > 7))
            {
                if (UserChangedBarcode[i].Length == 5)
                {
                    string msg = "The 5 or less characters barcode will be padded with ";
                    msg += (UserChangedBarcode[i].Length == 5) ? " a space. \n" : " spaces. \n";
                    msg += "Do you want to continue? (y/n) :";
                    string response = Console.ReadLine();
                    if (response == "N" || response == "n")
                    {
                        return false;
                    }
                    else
                    {
                        for (int a = UserChangedBarcode[i].Length; a < 6; a++)
                            UserChangedBarcode[i] = " " + UserChangedBarcode[i];
                    }
                }
                else
                {
                    Console.WriteLine(ErrString + "\nBarcode must be 6-7 characters long.");
                    return false;
                }
            }
            else if(!UserChangedBarcode[i].All(c => char.IsLetterOrDigit(c)))
            {
                if(UserChangedBarcode[i].Contains(" "))
                {
                    // The barcode is 5 or characters long and
                    // it is padded with a space9s). -- do nothing
                }
                else
                {
                    Console.WriteLine(UserChangedBarcode[i] + " and " + UserChangedBarcode[i].Length.ToString());
                    Console.WriteLine(ErrString + "\nBarcode must contain upper case letters and digits only.");
                    return false;
                }
            }

            if ((UserChangedDSR[i].Length > 0) && (DsrBuffer[i] != UserChangedDSR[i]) && (UserChangedDSR[i].Length > 5))
            {
                Console.WriteLine(ErrString + "\nDSR value must contain 5 or less characters.");
                return false;
            }
            else if (!UserChangedDSR[i].All(c => char.IsLetterOrDigit(c)))
            {
                Console.WriteLine(ErrString + "\nDSR value must contain upper case letters and digits only.");
                return false;
            }
            
            if ((UserChangedCatalog[i].Length > 0) && (CatalogBuffer[i] != UserChangedCatalog[i]) && 
                (UserChangedCatalog[i].Length < 8))
            {
                Console.WriteLine(ErrString + "\nCatalog number must be 8 or more characters long.");
                return false;
            }
            else if (!UserChangedCatalog[i].All(c => char.IsLetterOrDigit(c)))
            {
                Console.WriteLine(ErrString + "\nCatalog number must contain capital letters and digits only.");
                return false;
            }
            return true;
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
                Thread.Sleep(2);
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
