using System.Collections.Generic;
using System.IO;

namespace tlsnn.NetCore.IO
{
    public static class IOUtils
    {
        public static List<string> GetAllFilesFromFolder(string folder, bool searchSubfolders, params string[] searchPattern)
        {
            Queue<string> folders = new Queue<string>();
            List<string> files = new List<string>();
            folders.Enqueue(folder);
            while (folders.TryDequeue(out string currentFolder))
            {
                try
                {
                    if (searchPattern?.Length > 0)
                    {
                        for (int i = 0; i < searchPattern.Length; i++)
                        {
                            files.AddRange(Directory.GetFiles(currentFolder, searchPattern[i]));
                        }
                    }
                    else
                    {
                        files.AddRange(Directory.GetFiles(currentFolder));
                    }
                    if (searchSubfolders)
                    {
                        string[] foldersInCurrent = Directory.GetDirectories(currentFolder);
                        foreach (string _current in foldersInCurrent)
                        {
                            folders.Enqueue(_current);
                        }
                    }
                }
                catch { }
            }
            return files;
        }
    }
}
