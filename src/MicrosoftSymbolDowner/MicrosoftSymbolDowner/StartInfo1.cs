using tlsnn.NetCore.ArgsNS;
using System.Collections.Generic;

namespace MicrosoftSymbolDowner
{
    class StartInfo1 : StartInfo
    {
        public List<string> InputPEFilePaths { get; set; } = new List<string>();
        public string InputManifestFile { get; set; }
        public string Socks5Server { get; set; }
        public int Socks5Port { get; set; }
        public string SymbolServer { get; set; } = @"http://msdl.microsoft.com";
        public string OutputManifestFile { get; set; }
        public string OutputPDBFolder { get; set; }

    }
}
