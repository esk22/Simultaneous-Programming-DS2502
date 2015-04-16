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
    class DataOut
    {
        private static string[] HexList;
        DataIn BytesIn;
        crc _crc;
        //------------------------------------------------------------------------------------------------------------
        // Function name: public string GetDataPosLen(string Data, string type)
        // Description: Return the position of starting byte and length of a data type - Barcode,
        //              DSR, or Catlog number.
        //------------------------------------------------------------------------------------------------------------
        public string GetDataPosLen(string Data, string type)
        {
            string PosLen;
            HexList = new string[128];
            BytesIn = new DataIn();
            BytesIn.ParseStrToData(Data, ref HexList);
            if (HexList[0] == "FF")
                return "0000";
            int _crcPos = 0, _crc = 0, firstByte = 0;
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
                            _crcPos = i + (Convert.ToInt32(HexList[i], 16)) - 1;
                            _crc = Convert.ToInt32(HexList[_crcPos], 16);
                            if (type == "Barcode")
                                return (i.ToString("X2") + (Convert.ToInt32(HexList[i], 16) - 1).ToString("X2"));
                            else if (type == "Zap-Barcode")
                                return i.ToString("X2") + GetEndDataPos(i);
                        }
                    }
                    else if ((Convert.ToInt32(HexList[i], 16)) >= 3 && (Convert.ToInt32(HexList[i], 16)) <= 7 && _crcPos != i)
                    {
                        if (Convert.ToInt32(HexList[i], 16) != _crc)
                        {
                            DsrFound = true;
                            _crcPos = i + (Convert.ToInt32(HexList[i], 16)) - 1;
                            _crc = Convert.ToInt32(HexList[_crcPos], 16);
                            PosLen = i.ToString("X2") + (Convert.ToInt32(HexList[i], 16) - 1).ToString("X2");
                            if (type == "Zap-DSR")
                                return i.ToString("X2") + GetEndDataPos(i);
                        }
                    }
                }

                if ((Convert.ToInt32(HexList[i], 16)) >= 10 && (Convert.ToInt32(HexList[i], 16)) <= 47 && i > _crcPos)
                {
                    if (Convert.ToInt32(HexList[i], 16) != _crc && HexList[i] != "20")
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
            return (PosLen.Length == 4) ? PosLen : _crcPos.ToString("X2") + firstByte.ToString("X2");
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
        // Function name: public string CharToHex(string str, ref int [] data)
        // Description: Convert ASCII to Hex data, add length of the data at the beginning
        //------------------------------------------------------------------------------------------------------------
        public string CharToHex(string str, ref int[] data)
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
            _crc = new crc();
            int[] array = { 1, 2, 3 };
            if (previous == current)
            {
                return "";
            }

            else
            {
                if (dataLen == 0)
                {
                    bool replaced = true;
                    string catalog = (CharToHex((ChangedCatalogNumber(previous, current, ref replaced, len)), ref array) + _crc.crc8(array));
                    if (replaced)
                        return catalog + "j"; // The piece of data will be added at the end - saves memory :)
                    else
                        return catalog;
                }
                else
                {
                    return (CharToHex(current, ref array) + _crc.crc8(array));
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
                    if (changed.Length >= PrevLen)
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

        // Return true if at least one data field has changed
        public bool DataFieldChanged(string[] DataToWrite)
        {
            foreach (string element in DataToWrite)
            {
                if (element.Length > 1)
                {
                    return true;
                }
            }
            return false;
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

        // Return data field names where the data are to be erased from
        public string EraseDataFields(string data)
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
            }
            else
            {
                return data;
            }
        }

        // Return a tag for an ID Tag
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
