// 
// Filename: BytesManager.cs
// Author: Arun Rai - Virginia Tech
// This file is splitted into two files: DataIn.cs and DataOut.cs in Command Line Interface

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DataManagerWindow
{
    class BytesManager
    {
        private static string[] HexList;
        ComputeCRC8 crc;
        //------------------------------------------------------------------------------------------------------------
        // Function name:  private void ParseStrToData(string Data)
        // Description: Parse data string to appropriate data values
        //              - Each data value comes from one specific addresss
        //                in the ID Tag EPROM memory (Example: 09 is data value)
        //------------------------------------------------------------------------------------------------------------
        private void ParseStrToData(string Data)
        {
            int count1 = 0, SerialCount = 0, j = 0;
            string st = "";
            for (int i = 0; i < Data.Length - 3; i++)
            {
                if (Data[i] != ' ')
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
                                HexList[j] = st; j++;
                                st = "";
                                count1 = 0;
                            }
                        }
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------
        // Function name: public string GetCatalogNumber(string inputStr)
        // Description: Parse a string data to an appropriate catalog number
        //------------------------------------------------------------------------------------------------------------
        public string GetCatalogNumber(string inputStr, ref string DataReplaced)
        {
            if (!inputStr.Contains("*"))
                return inputStr;
            string part1 = "", part2 = "";
            for (int i = 0; i < inputStr.Length; i++)
            {
                if (inputStr[i] == '*')
                    break;
                part1 += inputStr[i];
            }
            for (int j = inputStr.Length - 1; j >= 0; j--)
            {
                if (inputStr[j] == '*')
                    break;
                part2 = inputStr[j] + part2;
                DataReplaced = part2;
            }
            string outputStr = part2;
            for (int i = part1.Length - part2.Length - 1; i >= 0; i--)
                outputStr = part1[i] + outputStr;
            return outputStr;
        }

        //------------------------------------------------------------------------------------------------------------
        // Function name: public string GetDataPosLen(string Data, string type)
        // Description: Return the position of starting byte and length of a data type - Barcode,
        //              DSR, or Catlog number.
        //------------------------------------------------------------------------------------------------------------
        public string GetDataPosLen(string Data, string type)
        {
            string PosLen;
            HexList = new string[128];
            ParseStrToData(Data);
            if (HexList[0] == "FF")
                return "0000";
            int crcPos = 0, crc = 0, firstByte = 0;
            bool BarcodeDsrEnded = false, BarcodeFound = false, DsrFound = false;
            PosLen = "";
            for (int i = 0; i < 127; i++)
            {
                if (!BarcodeDsrEnded)
                {
                    if ((Convert.ToInt32(HexList[i], 16)) == 8 || (Convert.ToInt32(HexList[i], 16)) == 9)
                    {
                        if (!BarcodeFound && !DsrFound)
                        {
                            BarcodeFound = true;
                            crcPos = i + (Convert.ToInt32(HexList[i], 16)) - 1;
                            crc = Convert.ToInt32(HexList[crcPos], 16);
                            if (type == "Barcode")
                                return (i.ToString("X2") + (Convert.ToInt32(HexList[i], 16) - 1).ToString("X2"));
                            else if (type == "Zap-Barcode")
                                return i.ToString("X2") + GetEndDataPos(i);
                        }
                    }
                    else if ((Convert.ToInt32(HexList[i], 16)) >= 3 && (Convert.ToInt32(HexList[i], 16)) <= 7 && crcPos != i)
                    {
                        if (Convert.ToInt32(HexList[i], 16) != crc)
                        {
                            DsrFound = true;
                            crcPos = i + (Convert.ToInt32(HexList[i], 16)) - 1;
                            crc = Convert.ToInt32(HexList[crcPos], 16);
                            PosLen = i.ToString("X2") + (Convert.ToInt32(HexList[i], 16) - 1).ToString("X2");
                            if (type == "Zap-DSR")
                                return i.ToString("X2") + GetEndDataPos(i);
                        }
                    }
                }

                if ((Convert.ToInt32(HexList[i], 16)) >= 10 && (Convert.ToInt32(HexList[i], 16)) <= 47 && i > crcPos)
                {
                    if (Convert.ToInt32(HexList[i], 16) != crc && HexList[i] != "20")
                    {
                        BarcodeDsrEnded = true;
                        if (type == "DSR")
                            break;
                        else if (type == "Catalog")
                            return (i.ToString("X2") + "00");
                        else if (type == "Zap-Catalog")
                            return i.ToString("X2") + GetEndDataPos(i);
                    }
                }
                if (HexList[i] == "FF")
                {
                    firstByte = i - 1;
                    break;
                }
            }
            return (PosLen.Length == 4) ? PosLen : crcPos.ToString("X2") + firstByte.ToString("X2");
        }

        //------------------------------------------------------------------------------------------------------------
        // Function name: public string GetEndDataPos(int pos)
        // Description: Return the ending address position of the data bytes
        //------------------------------------------------------------------------------------------------------------
        public string GetEndDataPos(int pos)
        {
            for (int i = pos; i < 127; i++)
            {
                if (HexList[i + 1] == "FF")
                {
                    return i.ToString("X2");
                }
            }
            return "00";
        }

        //------------------------------------------------------------------------------------------------------------
        // Function name: public void GetRecordList(string Data, ref string[] StrArray)
        // Description: Parse the data string and store the record data in an array
        //              Return the array by reference
        //------------------------------------------------------------------------------------------------------------
        public void GetRecordList(string Data, ref string[] StrArray)
        {
            HexList = new string[128];
            ParseStrToData(Data);
            int dataLen = GetDataNumber();
            StrArray = new string[dataLen];
            bool BarcodeDsrEnded = false, BarcodeFound = false, DsrFound = false;
            int crcPos = 0, crc = 0;
            int Index = 0;
            int barcodePos = -1;
            for (int i = 0; i < 127; i++)
            {
                if (!BarcodeDsrEnded)
                {
                    if (((Convert.ToInt32(HexList[i], 16)) == 8 || (Convert.ToInt32(HexList[i], 16)) == 9 ) &&
                        (HexList[i] != "00"))
                    {
                        if (!BarcodeFound && !DsrFound)
                        {
                            int barcodeLen = Convert.ToInt32(HexList[i], 16);
                            barcodePos = i + barcodeLen - 1;
                            string barcodeData = i.ToString("X2");
                            barcodeData = barcodeData + " " + HexList[i] + " ";
                            for (int j = i + 1; j < (i + barcodeLen - 1); j++)
                            {
                                if (HexList[j] == "20")
                                    barcodeData += "#";
                                else
                                    barcodeData += Convert.ToChar(Convert.ToInt32(HexList[j], 16)).ToString();
                            }
                            barcodeData += " " + HexList[i + barcodeLen - 1];
                            StrArray[Index] = barcodeData;
                            Index++;
                            BarcodeFound = true;
                            crcPos = i + (Convert.ToInt32(HexList[i], 16)) - 1;
                            crc = Convert.ToInt32(HexList[crcPos], 16);
                        }
                    }

                    else if ((Convert.ToInt32(HexList[i], 16)) >= 3 && (Convert.ToInt32(HexList[i], 16)) <= 7 && crcPos != i)
                    {
                        if (Convert.ToInt32(HexList[i], 16) != crc)
                        {
                            int len = Convert.ToInt32(HexList[i], 16);
                            string dsrData = i.ToString("X2");
                            dsrData = dsrData + " " + HexList[i] + " ";
                            for (int j = i + 1; j < (i + len - 1); j++)
                            {
                                dsrData += Convert.ToChar(Convert.ToInt32(HexList[j], 16)).ToString();
                            }
                            dsrData += " " + HexList[i + len - 1];
                            StrArray[Index] = dsrData;
                            Index++;
                            DsrFound = true;
                            crcPos = i + (Convert.ToInt32(HexList[i], 16)) - 1;
                            crc = Convert.ToInt32(HexList[crcPos], 16);
                        }
                    }
                }

                if ((Convert.ToInt32(HexList[i], 16)) >= 10 && (Convert.ToInt32(HexList[i], 16)) <= 47 && i > crcPos)
                {
                    // MessageBox.Show(Convert.ToInt32(HexList[i], 16).ToString());
                    if (Convert.ToInt32(HexList[i], 16) != crc && HexList[i] != " ")
                    {
                        int len = Convert.ToInt32(HexList[i], 16);
                        string catalog = i.ToString("X2") + " " + HexList[i] + " ";
                        StrArray[Index] = GetCatalog(catalog, len, i);
                        Index++;
                        BarcodeDsrEnded = true;
                        break;
                    }
                }
            }
            return;
        }

        //------------------------------------------------------------------------------------------------------------
        // Function name: private int GetDataNumber()
        // Description: compute and return the number of data in the EPROM of an ID Tag
        // Data - Barcode, DSR, and/or Catalog number
        //------------------------------------------------------------------------------------------------------------
        private int GetDataNumber()
        {
            bool BarcodeDsrEnded = false, BarcodeFound = false, DsrFound = false;
            int dataLen = 0, crcPos = 0, crc = 0;
            for (int i = 0; i < 127; i++)
            {
                if (!BarcodeDsrEnded)
                {
                    if ((Convert.ToInt32(HexList[i], 16)) == 8 || (Convert.ToInt32(HexList[i], 16)) == 9)
                    {
                        if (!BarcodeFound && !DsrFound)
                        {
                            dataLen++;
                            BarcodeFound = true;
                            crcPos = i + (Convert.ToInt32(HexList[i], 16)) - 1;
                            crc = Convert.ToInt32(HexList[crcPos], 16);
                        }
                    }
                    else if ((Convert.ToInt32(HexList[i], 16)) >= 3 && (Convert.ToInt32(HexList[i], 16)) <= 7 && crcPos != i)
                    {
                        if (Convert.ToInt32(HexList[i], 16) != crc)
                        {
                            dataLen++;
                            DsrFound = true;
                            crcPos = i + (Convert.ToInt32(HexList[i], 16)) - 1;
                            crc = Convert.ToInt32(HexList[crcPos], 16);
                        }
                        // crc = (Convert.ToInt32(HexList[i + (Convert.ToInt32(HexList[i], 16)) - 1], 16));
                    }
                }

                if ((Convert.ToInt32(HexList[i], 16)) >= 10 && (Convert.ToInt32(HexList[i], 16)) <= 47 && i > crcPos)
                {
                    // MessageBox.Show(Convert.ToInt32(HexList[i], 16).ToString());
                    if (Convert.ToInt32(HexList[i], 16) != crc && HexList[i] != " ")
                    {
                        dataLen++;
                        BarcodeDsrEnded = true;
                        break;
                    }
                }
            }
            return dataLen;
        }

        //------------------------------------------------------------------------------------------------------------
        // Function name: private string GetCatalog(string catalog, int len, int i)
        // Description: parse and return catalog number
        //------------------------------------------------------------------------------------------------------------
        private string GetCatalog(string catalog, int len, int i)
        {
            int crcPos = 0;
            string rawStr = "";
            for (int k = i + 1; k < HexList.Length - 1; k++)
            {
                crcPos++;
                if (k < (i + len - 1))
                {
                    catalog += Convert.ToChar(Convert.ToInt32(HexList[k], 16)).ToString();
                }
                else if (k == (i + len - 1))
                {
                    catalog += " " + HexList[k];
                }
                else
                {
                    if ((Convert.ToInt32(HexList[k], 16)) >= 1 && (Convert.ToInt32(HexList[k], 16)) <= 10)
                    {
                        len = Convert.ToInt32(HexList[k], 16);
                        catalog += " " + k.ToString("X2");
                        catalog += " " + HexList[k];
                        for (int m = k + 1; m < (k + len - 1); m++)
                        {
                            rawStr += Convert.ToChar(Convert.ToInt32(HexList[m], 16)).ToString();
                        }
                        catalog += " " + rawStr + " " + HexList[k + len - 1];
                        break;
                    }
                }
            }
            return catalog;
        }

        //------------------------------------------------------------------------------------------------------------
        // Function name: private static string SpaceDelimitor(string input)
        // Description: Remove any space in the input string if there is any
        //------------------------------------------------------------------------------------------------------------
        public string SpaceDelimitor(string input)
        {
            string output = "";
            for (int i = 0; i < input.Length; i++)
                if (char.IsLetterOrDigit(input[i]))
                    output += input[i];
            return output;
        }

        //------------------------------------------------------------------------------------------------------------
        // Function name: private static string RemoveExtraWords(string input)
        // Description: Remove extra unwanted words from the string
        //------------------------------------------------------------------------------------------------------------
        public string RemoveExtraWords(string input)
        {
            string[] buffer = Regex.Split(input, "finished");
            string output = "";
            foreach (string item in buffer)
                if (item != "finished")
                    output = item;
            return output;
        }

        //------------------------------------------------------------------------------------------------------------
        // Function name: private static bool SerialStrToBytes(string data)
        // Description: Separate data of individual ID Tag from the Serial String
        //------------------------------------------------------------------------------------------------------------
        public void SerialStrToBytes(string data, ref string[] DataBuffer)
        {
            string StrBuffer = "";
            if (data.Contains("end"))
            {
                string[] buffer = Regex.Split(data, "end");
                StrBuffer += buffer[0];
                for (int i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i].Contains("tag1"))
                    {
                        DataBuffer[0] = StrBuffer;
                    }
                    else if (buffer[i].Contains("tag2"))
                    {
                        DataBuffer[1] = buffer[i];
                    }
                    else if (buffer[i].Contains("tag3"))
                    {
                        DataBuffer[2] = buffer[i];
                    }
                    else if (buffer[i].Contains("tag4"))
                    {
                        DataBuffer[3] = buffer[i];
                    }
                    else if (buffer[i].Contains("tag5"))
                    {
                        DataBuffer[4] = buffer[i];
                    }
                    else if (buffer[i].Contains("tag6"))
                    {
                        DataBuffer[5] = buffer[i];
                    }
                }
            }
            else
            {
                StrBuffer += data;
            }
            if (data.Contains("finished"))
            {
                //MessageBox.Show("Reading done!");
                return;
            }
            return;
        }

        //------------------------------------------------------------------------------------------------------------
        // Function name: public string CharToHex(string str, ref int [] data)
        // Description: Convert ASCII to Hex data
        //------------------------------------------------------------------------------------------------------------
        public string CharToHex(string str, ref int [] data)
        {
            string output = "";
            data = new int[str.Length + 1];
            data[0] = str.Length + 2;
            output += (str.Length + 2).ToString("X2");
            for (int i = 0; i < str.Length; i++)
            {
                int dec = Convert.ToInt32(str[i]);
                output += dec.ToString("X2");
                data[i + 1] = dec;
            }
            return output;
        }

        //------------------------------------------------------------------------------------------------------------
        // Function name: public string GetDataToWrite(string previous, string current)
        // Description: Receive user input data
        //------------------------------------------------------------------------------------------------------------
        public string GetDataToWrite(string previous, string current, int dataLen = 1, int len = 0)
        {
            // len - > Length of the most recently replaced data in the current Catalog number
            crc = new ComputeCRC8();
            int [] array = {1,2,3};
            if (previous == current)
            {
                return "";
            }
            
            else
            {
                if (dataLen == 0 && len == 0 )
                {
                    bool replaced = true;
                    string catalog = (CharToHex((ChangedCatalogNumber(previous, current, ref replaced, len)), ref array) + crc.crc8(array));
                    if (replaced)
                        return catalog + "j"; // The piece of data will be added at the end - saves memory :)
                    else
                        return catalog;
                }
                else
                {
                    // if len > 0 - length of the previously replaced data - , data replacement in the catalog number
                    // has previously occured. Replacement can no longer be done.
                    return (CharToHex(current, ref array) + crc.crc8(array));       
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------
        // Function name: private static string ChangedCatalogNumber(string previous, string current)
        // Description:
        // Return the changed part of Catalog number
        // Example: previous catalog no: IS200BPD0H1BBC
        //          current catalog no:  IS200BPD0H1AA2
        // In the current catalog no. BBC is replaced by AA2. The function 
        // matches the input catalog number against the previous data, and returns the 
        // changed part of the data - in this case, AA2.
        //------------------------------------------------------------------------------------------------------------
        private static string ChangedCatalogNumber(string previous, string current, ref bool replaced, int PrevLen)
        {
            string changed = "";
            if (previous.Length == current.Length)
            {
                int matched = 0;
                for (int i = 0; i < previous.Length; i++)
                {
                    if (previous[i] == current[i])
                        matched++;
                    else
                        break;
                }
                for (int j = matched; j < current.Length; j++)
                    changed += current[j];
                if (changed.Length == current.Length)
                {
                    replaced = false;
                }
                else
                {
                    if(changed.Length >= PrevLen)
                        replaced = true;
                    else
                        replaced = false;
                }
                return changed;
            }
            else
            {
                replaced = false;
                return current;
            }
        }

        //
        // Return true if at least one data field has changed
        //
        public bool DataFieldChanged(string[] DataToWrite)
        {
            foreach (string element in DataToWrite)
            {
                if (element.Length > 2)
                {
                    return true;
                }
            }
            return false;
        }

        //-----------------------------------------------------------------------------------------------------------
        // Function name: public bool VerifyUserInputDsr(string current, string previous, ref string dsr, int tag)
        // Description: Check if the user input DSR value already exists, return true or false
        //-----------------------------------------------------------------------------------------------------------
        public bool VerifyUserInputDsr(string current, string previous)
        {
            if (previous.Length > 0 && current.Length > 0)
            {
                string[] prev_dsr = previous.Split(' ');
                for (int i = 0; i < prev_dsr.Length - 1; i++ )
                {
                    if (current == prev_dsr[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        //------------------------------------------------------------------------------------------------------------
        // Function name: public bool EraseData(string previous, string current, string tag, ref string sum_tag)
        // Description: Return true if data field is to be erased, return false otherwise
        //------------------------------------------------------------------------------------------------------------
        public bool EraseData(string previous, string current, string tag, ref string sum_tag)
        {
            if (previous.Length > 1 && current.Length == 0)
            {
                if (sum_tag.Length > 1)
                    sum_tag += ("0" + tag);
                else
                    sum_tag = tag;
                return true;
            }
            else
            {
                return false;
            }
        }

        //
        // Return data field names where the data are to be erased from
        //
        public string DataFieldNames(string data)
        {
            string[] str = data.Split('0');
            if (str.Length == 1)
            {
                return data;
            }
            else if (str.Length == 2)
            {
                return (str[0] + " and " + str[1]);
            }
            else if (str.Length == 3)
            {
                return (str[0] + ", " + str[1] + ", and " + str[2]);
            }else
            {
                return data;
            }
        }

        //
        // Return a tag for an ID Tag
        //
        public string GetTag(int i)
        {
            switch (i)
            {
                case 0:
                    return "a"; // Tag 1
                case 1:
                    return "b"; // Tag 2
                case 2:
                    return "c"; // Tag 3
                case 3:
                    return "d"; // Tag 4
                case 4:
                    return "e"; // Tag 5
                case 5:
                    return "f"; // Tag 6
                default:
                    return ""; // No tag
            }
        }
    }
}
