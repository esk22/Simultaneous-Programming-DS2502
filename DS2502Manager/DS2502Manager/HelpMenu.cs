//
// Author: Arun Rai
// Date: 04/17/2015
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS2502Manager
{
    class HelpMenu
    {
        // Get command
        public void HelpCommand(string [] command)
        {
            if (command[1] == "Erase" || command[1] == "erase" || command[1] == "ERASE")
            {
                HelpErase();
            }
            else if (command[1] == "write" || command[1] == "Write" || command[1] == "WRITE")
            {
                HelpWrite();
            }
            else if (command[1] == "Read" || command[1] == "read" || command[1] == "READ")
            {
                HelpRead();
            }
            else
            {
                Console.WriteLine("Unrecognized command.");
            }
        }

        public void HelpErase()
        {
            string HelpErase = "\r\nErase command help : ";
            HelpErase += " erase-command chip-number data-type(s)-separted-by-backslash \r\n";
            HelpErase += "Notations <=> Barcode --> b, D.S.R. --> d, and Catalog number --> c \r\n";
            HelpErase += "Example 1: $ Program.exe erase 1 b  \r\n";
            HelpErase += "Example 2: $ Program.exe erase 1 b-backslash-d-backslash-c \r\n";
            HelpErase += "\r\nMultiple erase commands are separated by comma(s). \r\n";
            HelpErase += "Example 3: $ Program.exe erase 1 b-backslash-d-backslash-c, 3 b-backslash-c \r\n";
            HelpErase += "\r\nWhen asked to continue, press y and hit enter to continue, \r\n OR press n and hit enter to terminate. \r\n";
            Console.WriteLine(HelpErase);
        }

        public void HelpWrite()
        {
            string HelpWrite = "\r\nWrite command help : chip-number barcode dsr(s)-separated-by-| catalog-number \r\n";
            HelpWrite += "\r\n1) Enter the write command - $ Program.exe write \r\n";
            HelpWrite += "2) Enter the number of ID Chips to program. \r\n";
            HelpWrite += "3) Enter write command(s) - one command at a time. \r\n";
            HelpWrite += "   Example command: 2 TYUJHNK 12C0|12O|89KL IS200OIPLKBNM \r\n";
            HelpWrite += "   The data types must be entered in the sequence - barcode, D.S.R., and catalog. \r\n";
            HelpWrite += "\r\n   If there are only one or two data type(s) to program, enter in the appropriate sequence. \r\n";
            HelpWrite += "   Example command: 5 1234|90 IS200OIKJBBA - no barcode will be programmed in the ID Chip 5 \r\n";
            HelpWrite += "\r\n   If no barcode is provided, '|' should be added at the end of a single D.S.R. \r\n";
            HelpWrite += "   Example command: 2 1C00| IS200OPLKMJNBB \r\n";
            HelpWrite += "\r\n   Input data length restrictions: \r\n";
            HelpWrite += "   a. Barcode should be 6-7 characters long. A barcode with 5 or less characters will be padded \r\n";
            HelpWrite += "      with leading space(s). \r\n";
            HelpWrite += "   b. D.S.R. must be 5 or less characters long. \r\n";
            HelpWrite += "   c. Catalog number must be 8 or more characters long.\r\n";
            HelpWrite += "\r\nWhen asked to continue, press y and hit enter to continue, \r\n OR press n and hit enter to terminate. \r\n";

            Console.WriteLine(HelpWrite);
        }

        public void HelpRead()
        {
            string HelpRead = "\r\nRead command help : read-command chip-number \r\n";
            HelpRead += "\r\nExample : $ Program.exe read 2 \r\n";
            HelpRead += "\r\n   OR     $ Program.exe read all \r\n";
            Console.WriteLine(HelpRead);
        }
    }
}
