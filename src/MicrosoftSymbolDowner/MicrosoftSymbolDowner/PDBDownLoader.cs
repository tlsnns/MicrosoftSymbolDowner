using MihaZupan;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MicrosoftSymbolDowner
{
    class PDBDownLoader
    {
        HttpClient SysbolsClient;
        HttpClient RedirectClient;

        string PDBFolder;
        public PDBDownLoader(string strSymbolServer, string strSocks5Server, int socks5Port, string strFolder)
        {
            PDBFolder = strFolder;

            SysbolsClient = new HttpClient(new SocketsHttpHandler { AllowAutoRedirect = false });
            SysbolsClient.Timeout = new TimeSpan(00, 00, 30);
            SysbolsClient.BaseAddress = new Uri(strSymbolServer);
            SysbolsClient.DefaultRequestVersion = new Version(2, 0);

            if (string.IsNullOrWhiteSpace(strSocks5Server))
            {
                RedirectClient = new HttpClient();
            }
            else
            {
                var proxy = new HttpToSocks5Proxy(strSocks5Server, socks5Port);
                var httpClientHandler = new HttpClientHandler { Proxy = proxy };
                RedirectClient = new HttpClient(httpClientHandler, true);
            }
            RedirectClient.Timeout = new TimeSpan(00, 00, 30);
        }
        public bool TryDownLoadPDB(ManifestEntity manifestEntity, out string strErrorMessage)
        {
            string strDir = Path.Combine(PDBFolder, manifestEntity.PDBName, manifestEntity.PDBGuid + manifestEntity.Age);
            string strPathSrc = Path.Combine(strDir, manifestEntity.PDBName + ".tmp");
            string strPathDes = Path.Combine(strDir, manifestEntity.PDBName);
            if (File.Exists(strPathDes))
            {
                strErrorMessage = null;
                return true;
            }
            string strUrl = $@"/download/symbols/{manifestEntity.PDBName}/{manifestEntity.PDBGuid + manifestEntity.Age}/{manifestEntity.PDBName}";
            var t1 = SysbolsClient.GetAsync(strUrl);
            t1.Wait();
            var r1 = t1.Result;
            if (r1.StatusCode != HttpStatusCode.Redirect)
            {
                strErrorMessage = $"符号服务器响应失败，状态码：{r1.StatusCode}";
                return false;
            }
            if (r1.Headers.Location == null)
            {
                strErrorMessage = $"符号服务器响应头没有Location域";
                return false;
            }
            int retryNumber = 5;
            HttpResponseMessage r2;
            while (true)
            {
                try
                {
                    var t2 = RedirectClient.GetAsync(r1.Headers.Location);
                    t2.Wait();
                    r2 = t2.Result;
                    break;
                }
                catch
                {
                    retryNumber--;
                    if (retryNumber == 0)
                    {
                        throw;
                    }
                }
            }
            if (r2.StatusCode != HttpStatusCode.OK)
            {
                strErrorMessage = $"下载服务器响应失败，状态码：{r2.StatusCode}";
                return false;
            }
            if (r2.Content.Headers.ContentType.MediaType != @"application/octet-stream")
            {
                strErrorMessage = $"下载服务器响应响应头ContentType为：{r2.Content.Headers.ContentType.MediaType ?? "null"}";
                return false;
            }
            Directory.CreateDirectory(strDir);
            if (File.Exists(strPathDes))
            {
                strErrorMessage = null;
                return true;
            }
            using (FileStream fileStream1 = new FileStream(strPathSrc, FileMode.Create, FileAccess.Write))
            {
                r2.Content.CopyToAsync(fileStream1).Wait();
                fileStream1.Flush();
            }
            File.Move(strPathSrc, strPathDes);
            strErrorMessage = null;
            return true;
        }
    }
}
