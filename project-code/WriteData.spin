

CON
  _clkmode = xtal1 + pll16x
  _xinfreq = 5_000_000
  MAX_DEVICES   = 1
  ' PIN 10 - 15 will be used for
  ' W/R operation for ID Tags (DS2502)
  PIN10         = 10
  PIN11         = 11
  PIN12         = 12
  PIN13         = 13
  PIN14         = 14
  PIN15         = 15
  
  PIN22         = 22
  PIN23         = 23
  PIN24         = 24
  PIN25         = 25
  PIN26         = 26
  PIN27         = 27
  
OBJ
  tag1      : "SpinOneWire-debug-mode"
  tag2      : "SpinOneWire-debug-mode"
  tag3      : "SpinOneWire-debug-mode"
  tag4      : "SpinOneWire-debug-mode"
  tag5      : "SpinOneWire-debug-mode"
  tag6      : "SpinOneWire-debug-mode"
  system    : "Propeller Board of Education"
  PORT      : "Parallax Serial Terminal Plus"
  
VAR
  long addrs1[2 * MAX_DEVICES]
  long BytesRead[6]
  long BytesWritten[6]
  long BytesReady[6]
  ' Define addresses - one for each COG
  long addr2, addr3, addr4, addr5, addr6
  byte counter1, counter2, counter3, counter4, counter5, counter6
  byte TagNumber
  byte dataStart[6]
  byte CRC1[4]
  byte CRC2[4]
  byte CRC3[4]
  byte CRC4[4]
  byte CRC5[4]
  byte CRC6[4]
  byte PGM
  
PUB go | a
    SetPGMLineHigh
    ' Intialization of buffers
    system.Clock(80_000_000)
    'TagsInit
    tag1.start(10)
    PGM := 22
    ReadDevice1

                   
' Initialize the flags           
PRI InitReadFlags : a
    repeat a from 0 to 6
        BytesRead[a] := 0
        BytesReady[a] := 0

' Reading operation - Tag 1 
PRI ReadDevice1 | i, numDevices, addr1, x
    numDevices := tag1.search(tag1#REQUIRE_CRC, MAX_DEVICES, @addrs1)
    repeat i from 0 to MAX_DEVICES
        if i => numDevices
            ' No device found
            BytesRead[0] := 1
            BytesReady[0] := 1
        else
            addr1 := @addrs1 + (i << 3)
            if BYTE[addr1] == tag1#FAMILY_DS2502
                SendChipSerialNo(addr1)
                CRC1[0] := tag1#READ_MEMORY
                Write(127,127, $0D)
                repeat x from 127 to 127
                    CRC1[1] := x
                    CRC2[2] := 0
                    tag1.SendStr(string(" Pos: "))
                    tag1.SendDec(x)
                    tag1.SendStr(string(" Data: "))
                    tag1.SendHex(tag1.ReadAddressContent(x), 2)
                    tag1.SetNewLine
                tag1.SendStr(String("tag1end"))
                BytesRead[0] := 1
                BytesReady[0] := 1
                   
PRI Write(s, e, d) : a
    repeat a from s to e
        tag1.ByteToMemory(a, d, PGM, 1)
    return
    
'' Turn on LEDs P16 when data/command
'' is received through the Serial Port  
'' This function may not be required later.   
PRI DataReceiveIndicator
    dira[16] := 1
    outa[16] := 1
    waitcnt(clkfreq + cnt)
    outa[16] := 0      

'' Send the Serial Number of an ID Tag  to the 
'' GUI through the Serial Port. 
'' This ID Tag Serial Number is read from the ROM
PRI SendChipSerialNo(address)
    tag1.SendHex(LONG[address + 4], 8)
    tag1.SendHex(LONG[address], 8)
    tag1.SendStr(string(" "))

'' Set the programming lines high
'' The driver circuit is active low circuit. The circuit
'' provides a 12V pulse to an ID Tag when the PGM line is low.
PRI SetPGMLineHigh
    dira[PIN22]~~
    outa[PIN22] := 1
    dira[PIN23]~~
    outa[PIN23] := 1
    dira[PIN24]~~
    outa[PIN24] := 1
    dira[PIN25]~~
    outa[PIN25] := 1
    dira[PIN26]~~
    outa[PIN26] := 1
    dira[PIN27]~~
    outa[PIN27] := 1

'' Initial PIN configuration for R/W operations              
PRI TagsInit
    tag1.start(PIN10)
    tag2.start(PIN11)
    tag3.start(PIN12)
    tag4.start(PIN13)
    tag5.start(PIN14)
    tag6.start(PIN15)
    