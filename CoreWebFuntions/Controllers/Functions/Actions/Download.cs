using CoreWebFuntions.Data.Configs;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreWebFuntions.Controllers.Functions.Actions
{
    public class Download
    {
        public class Request : IRequest<Response>
        {
            public string Key { get; set; }
        }

        public class Response
        {
            public byte[] Bytes { get; set; }
            public string ContentType { get; set; }
            public string FileName { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly DownloadConfig downloadConfig;

            public Handler(IOptions<DownloadConfig> downloadConfig)
            {
                this.downloadConfig = downloadConfig.Value;
            }

            public async Task<Response> Handle(Request request, CancellationToken token)
            {
                string tempFolderPath = downloadConfig.TempFolder;
                string target = downloadConfig.Files.FirstOrDefault(x => x.Key.ToLower() == request.Key.ToLower())?.Path;

                if ((string.IsNullOrWhiteSpace(target) == false)
                    && (File.Exists(target) || Directory.Exists(target)))
                {
                    Response response = new Response();
                    FileAttributes attr = File.GetAttributes(target);
                    if (attr.HasFlag(FileAttributes.Directory))
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(target);
                        string temp1 = Path.Combine(tempFolderPath, Guid.NewGuid().ToString("N"));
                        string temp2 = Path.Combine(temp1, directoryInfo.Name);
                        string download = Path.Combine(tempFolderPath, $"{Guid.NewGuid().ToString("N")}.zip");
                        Directory.CreateDirectory(temp1);
                        Directory.CreateDirectory(temp2);
                        foreach (var dir in directoryInfo.GetDirectories("*", SearchOption.AllDirectories))
                        {
                            System.Diagnostics.Debug.WriteLine($"copy directory from {dir.FullName} to {Path.Combine(temp2, dir.FullName.Substring(directoryInfo.FullName.Length + 1))}");
                            Directory.CreateDirectory(Path.Combine(temp2, dir.FullName.Substring(directoryInfo.FullName.Length + 1)));
                        }
                        foreach (var file in directoryInfo.GetFiles("*", SearchOption.AllDirectories))
                        {
                            System.Diagnostics.Debug.WriteLine($"copy file from {file.FullName} to {Path.Combine(temp2, file.FullName.Substring(directoryInfo.FullName.Length + 1))}");
                            File.Copy(file.FullName, Path.Combine(temp2, file.FullName.Substring(directoryInfo.FullName.Length + 1)));
                        }
                        ZipFile.CreateFromDirectory(temp1, download);
                        using (MemoryStream memoryStream = new MemoryStream())
                        using (FileStream fileStream = new FileStream(download, FileMode.Open, FileAccess.Read))
                        {
                            await fileStream.CopyToAsync(memoryStream);
                            response.Bytes = memoryStream.ToArray();
                        }
                        response.ContentType = MimeMapping.MimeUtility.GetMimeMapping("zip");
                        response.FileName = $"{directoryInfo.Name}.zip";
                        Directory.Delete(temp1, true);
                        File.Delete(download);
                    }
                    else
                    {
                        FileInfo fileInfo = new FileInfo(target);
                        using (MemoryStream memoryStream = new MemoryStream())
                        using (FileStream fileStream = new FileStream(target, FileMode.Open, FileAccess.Read))
                        {
                            await fileStream.CopyToAsync(memoryStream);
                            response.Bytes = memoryStream.ToArray();
                        }
                        response.ContentType = MimeMapping.MimeUtility.GetMimeMapping(fileInfo.Extension);
                        response.FileName = fileInfo.Name;
                    }
                    return response;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
