# MicrosoftSymbolDowner

一个下载微软pe符号的工具，支持socks5代理

命令行

--h   显示帮助信息

--cm    创建清单文件

--dp    下载pdb文件

-im     输入清单文件

-if     输入pe文件

-id     输入pe目录

-idr    输入pe目录，及其子目录

-om     输出清单文件

-od     输出pdb文件夹

-ss     符号服务器 默认为：http://msdl.microsoft.com

-sps    socks5代理地址

-spp    socks5代理端口

示例

一

1.  --cm -id C:\Windows\System32 -om .\manifest.txt 生成清单文件

2.  --dp -im .\manifest.txt -od .\ 输入清单下载符号

二

--dp -if C:\Windows\System32\cmd.exe -od .\ 下载指定文件的符号

三

-dp -id C:\Windows\System32 -od .\ 下载指定文件夹的符号

四

-dp -idr C:\Windows\System32 -od .\ 下载指定文件夹及其子文件夹的符号