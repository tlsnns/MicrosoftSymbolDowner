using MihaZupan;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using tlsnn.NetCore.ConsoleUnit;

namespace MicrosoftSymbolDowner
{
    class PDBDownLoader
    {
        HttpClient SysbolsClient;
        string Socks5Server;
        int Socks5Port;
        public PDBDownLoader(string strSymbolServer, string strSocks5Server, int socks5Port)
        {
            SysbolsClient = new HttpClient(new SocketsHttpHandler { AllowAutoRedirect = false });
            SysbolsClient.Timeout = new TimeSpan(00, 00, 30);
            SysbolsClient.BaseAddress = new Uri(strSymbolServer);
            Socks5Server = strSocks5Server;
            Socks5Port = socks5Port;

        }
        public void DownLoad(ConcurrentQueue<PDBEntity> taskLists)
        {
            ConcurrentQueue<PDBEntity> needLoadTask = new ConcurrentQueue<PDBEntity>();
            do
            {
                taskLists = QueryPDBLocation(taskLists, needLoadTask);
            }
            while (taskLists.Count != 0);
            do
            {
                needLoadTask = DownLoadPDB(needLoadTask);
            }
            while (needLoadTask.Count != 0);
        }
        public ConcurrentQueue<PDBEntity> QueryPDBLocation(ConcurrentQueue<PDBEntity> queryTasks, ConcurrentQueue<PDBEntity> needLoadTask)
        {
            ConcurrentQueue<PDBEntity> needRetryTask = new ConcurrentQueue<PDBEntity>();
            ConsoleTopProgressBar progressBarSingleton = new ConsoleTopProgressBar(queryTasks.Count, 50, "查询PDB位置");
            while (queryTasks.TryDequeue(out PDBEntity item))
            {
                if (File.Exists(item.PDBPath))
                {
                    progressBarSingleton.Dispaly();
                }
                else
                {
                    QueryPDBLocationAsync(item.URL).ContinueWith(withTask =>
                    {
                        if (withTask.IsCompletedSuccessfully)
                        {
                            if (withTask.Result != null)
                            {
                                item.Location = withTask.Result;
                                needLoadTask.Enqueue(item);
                            }
                            progressBarSingleton.Dispaly();
                        }
                        else if (withTask.IsCanceled)
                        {
                            needRetryTask.Enqueue(item);
                            progressBarSingleton.Dispaly();
                        }
                        else if (withTask.IsFaulted)
                        {
                            needRetryTask.Enqueue(item);
                            progressBarSingleton.Dispaly(item.RSDSEntity.PDBName + "\t" + withTask.Exception.Message);
                        }
                    });
                }
            }
            progressBarSingleton.Over();
            return needRetryTask;
        }

        public async Task<Uri> QueryPDBLocationAsync(string strUrl)
        {
            var r = await SysbolsClient.GetAsync(strUrl);
            if (r.StatusCode == HttpStatusCode.Redirect)
            {
                if (r.Headers.Location == null)
                {
                    throw new HttpRequestException("符号服务器响应头返回重定向，但是没有Location域");
                }
                return r.Headers.Location;
            }
            else if (r.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            else
            {
                throw new HttpRequestException($"符号服务器响应失败，状态码：{r.StatusCode}");
            }
        }

        public ConcurrentQueue<PDBEntity> DownLoadPDB(ConcurrentQueue<PDBEntity> loadTasks)
        {
            ConcurrentQueue<PDBEntity> needRetryTask = new ConcurrentQueue<PDBEntity>();
            ConsoleTopProgressBar progressBarSingleton = new ConsoleTopProgressBar(loadTasks.Count, 50, "下载PDB");
            while (loadTasks.TryDequeue(out PDBEntity item))
            {
                DownLoadPDBAsync(item).ContinueWith(withTask =>
                {
                    if (withTask.IsCompletedSuccessfully)
                    {
                        progressBarSingleton.Dispaly();
                    }
                    else if (withTask.IsCanceled)
                    {
                        needRetryTask.Enqueue(item);
                        progressBarSingleton.Dispaly();
                    }
                    else if (withTask.IsFaulted)
                    {
                        needRetryTask.Enqueue(item);
                        progressBarSingleton.Dispaly(item.RSDSEntity.PDBName + "\t" + withTask.Exception.Message);
                    }
                });
            }
            progressBarSingleton.Over();
            return needRetryTask;
        }

        public async Task DownLoadPDBAsync(PDBEntity pdbEntity)
        {

            HttpClient DownLoadClient;
            if (string.IsNullOrWhiteSpace(Socks5Server))
            {
                DownLoadClient = new HttpClient();
            }
            else
            {
                var httpClientHandler = new HttpClientHandler { Proxy = new HttpToSocks5Proxy(Socks5Server, Socks5Port) };
                DownLoadClient = new HttpClient(httpClientHandler, true);
            }
            DownLoadClient.Timeout = new TimeSpan(00, 10, 00);

            HttpResponseMessage r2 = await DownLoadClient.GetAsync(pdbEntity.Location);

            if (r2.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpRequestException($"下载服务器响应失败，状态码：{r2.StatusCode}");
            }
            if (r2.Content.Headers.ContentType.MediaType != @"application/octet-stream")
            {
                throw new HttpRequestException($"下载服务器响应响应头ContentType为：{r2.Content.Headers.ContentType.MediaType ?? "null"}");
            }
            Directory.CreateDirectory(pdbEntity.PDBDir);
            FileStream fileStream1 = new FileStream(pdbEntity.TmpPDBPath, FileMode.Create, FileAccess.Write);
            await r2.Content.CopyToAsync(fileStream1).ContinueWith(withTask =>
            {
                if (withTask.IsCompletedSuccessfully)
                {
                    fileStream1.Close();
                    File.Move(pdbEntity.TmpPDBPath, pdbEntity.PDBPath);
                }
            });
        }
    }
}
