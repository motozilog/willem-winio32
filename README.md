# willem-winio32
Willem programmer use winio32 for add support some offical unsupported chip

Finally version : V1.60

End of development.

## Current Support Chips(V1.60):

>AM29/MX29(Tested Chip: AM29LV200BB、MX29LV320、S29GL032、S29GL064、MX29LV128、S29GL128、MX29GL256、S29GL256、MX29GL512、S29GL512、S29GL01G、MX68GL1G)

32bit Adapter: https://oshwhub.com/motozilog/32bit-willem

TSOP48 Adapter: https://oshwhub.com/motozilog/tsop56-dip48

TSOP56 Adapter: https://oshwhub.com/motozilog/tsop56-dip48_copy

>S70GL02(Tested Chip: S70GL02GS11FHI010)

S70GL02 Adapter: https://oshwhub.com/motozilog/32bit-willem_copy_copy_copy_copy

>M59PW016

>M59PW032(untested)

>M59PW064

>M59PW1282

>MX29F1615

>MX29L3211(support,but write slow)

>MX29F1610(support,but not recommend，use M59PW016 instead)

>S25XX SPI NOR FLASH(Support 24bit Address& 32bit Address chip, older chip that use 16bit address,please use willem-programmer soft instead. Only tested on EN25T80 and MX25L51245. NOTE:If chip use 3.3V VCC require LDO for chip VCC, If chip use 1.8V VCC require level shifter)

>ATF16V8B


## May Support Chip

>ATF20V8B/ATF22V10B


## Never Support Chip:

>SPI NAND FLASH (Bad Block Manage, And Chip Size too large too slow. Use CH341A programmer instead)

>NAND FLASH(Bad Block Manage, may direct connect to LPT will better for speed. Use CBM2199E instead)

>eMMC(Use eMMC to SDcard reader instead)

>F0095H0(use VTX instead https://github.com/xvortex/VTXCart/ )

## Use in Development build Chip:

>MX26L6420(write unsuccess in once, require write many times)

>MX26L12811(write fail in unknown condition, and can't write many times)

>SST 29EE512(unknown together OE low to high to low will update address)



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