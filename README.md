# willem-winio32
Willem programmer use winio32 for add support some offical unsupported chip

## Current Support Chips(V1.50):

>AM29/MX29(Tested Chip: AM29LV200BB、MX29LV320、S29GL032、S29GL064、MX29LV128、S29GL128、MX29GL256、S29GL256、MX29GL512、S29GL512、S29GL01G)

>M59PW016

>M59PW032(untested)

>M59PW064

>M59PW1282

>MX29F1615

>MX29L3211(support,but write slow)

>MX29F1610(support,but not recommend，use M59PW016 instead)

>MX26L6420(support,but not recommend，use M59PW064 instead，make sure check diffient bytes again)

>MX26L12811(support,but strongly not recommend,use M59PW1282 instead, only 10 erase/programm cycle, much bytes are factory bad!)

>S25XX SPI NOR FLASH(Support 24bit Address& 32bit Address chip, older chip that use 16bit address,please use willem-programmer soft instead. Only tested on EN25T80 and MX25L51245. NOTE:If chip use 3.3V VCC require LDO for chip VCC, If chip use 1.8V VCC require level shifter)

>ATF16V8B

## Work in progress Chip:

>ATF22V10B


## Plan Chip:

>S70GL02

## May Support Chip

>GAL16V8B/GAL20V8B/GAL22V10B (VPP voltage may require modify from willem programmer)

>F0095H0

## Never Support Chip:

>SPI NAND FLASH (Bad Block Manage, And Chip Size too large too slow. Use CH341A programmer instead)

>NAND FLASH(Bad Block Manage, may direct connect to LPT will better for speed. Use CBM2199E instead)

>eMMC(Use eMMC to SDcard reader instead)

## Test OS

>Windows XP SP3 32bit

>Windows 7 SP1 x64(Require Test mode)

>May support OS: Windows 8/Windows 10

## Development Environment
>WIndows XP SP3 32bit + Visual Studio 2010

>Windows 7 SP1 x64(Test mode) + Visual Studio 2010 SP1

## Use Library
C# wrapper for WinChipHead CH341(A) ( https://github.com/iillii/ch341-Sharp )

A fork of WinIo which developed by Yariv Kaplan from ( https://github.com/starofrainnight/winio )

## License

GNU General Public License v3.0