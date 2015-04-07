//
// Author: Arun Rai - Virginia Tech
//
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
    class DataIn
    {
        private static string[] HexList;
        crc _crc;
        //------------------------------------------------------------------------------------------------------------
        // Function name:  private void ParseStrToData(string Data)
        // Description: Parse data string to appropriate data values
        //              - Each data value comes from one specific addresss
        //                in the ID Tag EPROM memory (Example: 09 is data value)
        //------------------------------------------------------------------------------------------------------------
        public void ParseStrToData(string Data, ref string[] HexList)
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
        // Function name: public void GetRecordList(string Data, ref string[] StrArray)
        // Description: Parse the data string and store the record data in an array
        //              Return the array by reference
        //------------------------------------------------------------------------------------------------------------
        public void GetRecordList(string Data, ref string[] StrArray)
        {
            HexList = new string[128];
            ParseStrToData(Data, ref HexList);
            int dataLen = GetDataNumber();
            StrArray = new string[dataLen];
            bool BarcodeDsrEnded = false, BarcodeFound = false, DsrFound = false;
            int _crcPos = 0, _crc = 0;
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
                            _crcPos = i + (Convert.ToInt32(HexList[i], 16)) - 1;
                            _crc = Convert.ToInt32(HexList[_crcPos], 16);
                        }
                    }

                    else if ((Convert.ToInt32(HexList[i], 16)) >= 3 && (Convert.ToInt32(HexList[i], 16)) <= 7 && _crcPos != i)
                    {
                        if (Convert.ToInt32(HexList[i], 16) != _crc)
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
                            _crcPos = i + (Convert.ToInt32(HexList[i], 16)) - 1;
                            _crc = Convert.ToInt32(HexList[_crcPos], 16);
                        }
                    }
                }

                if ((Convert.ToInt32(HexList[i], 16)) >= 10 && (Convert.ToInt32(HexList[i], 16)) <= 47 && i > _crcPos)
                {
                    // MessageBox.Show(Convert.ToInt32(HexList[i], 16).ToString());
                    if (Convert.ToInt32(HexList[i], 16) != _crc && HexList[i] != " ")
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
            int dataLen = 0, _crcPos = 0, _crc = 0;
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
                            _crcPos = i + (Convert.ToInt32(HexList[i], 16)) - 1;
                            _crc = Convert.ToInt32(HexList[_crcPos], 16);
                        }
                    }
                    else if ((Convert.ToInt32(HexList[i], 16)) >= 3 && (Convert.ToInt32(HexList[i], 16)) <= 7 && _crcPos != i)
                    {
                        if (Convert.ToInt32(HexList[i], 16) != _crc)
                        {
                            dataLen++;
                            DsrFound = true;
                            _crcPos = i + (Convert.ToInt32(HexList[i], 16)) - 1;
                            _crc = Convert.ToInt32(HexList[_crcPos], 16);
                        }
                    }
                }

                if ((Convert.ToInt32(HexList[i], 16)) >= 10 && (Convert.ToInt32(HexList[i], 16)) <= 47 && i > _crcPos)
                {
                    // MessageBox.Show(Convert.ToInt32(HexList[i], 16).ToString());
                    if (Convert.ToInt32(HexList[i], 16) != _crc && HexList[i] != " ")
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
            int _crcPos = 0;
            string rawStr = "";
            for (int k = i + 1; k < HexList.Length - 1; k++)
            {
                _crcPos++;
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
        // Function name: private static bool ParseSerialString(string data)
        // Description: Separate data of individual ID Tag from the Serial String
        //------------------------------------------------------------------------------------------------------------
        public void ParseSerialString(string data, ref string[] DataBuffer)
        {
            string StrBuffer = "";
            if (data.Contains("end"))
            {
                string[] buffer = Regex.Split(data, "end");
                StrBuffer += buffer[0];
                for (int i = 0; i < buffer.Length; i++)
                {
                    //Console.WriteLine(data);
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
    }
}
