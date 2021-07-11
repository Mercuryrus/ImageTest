using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using ImageTest.ResponseModels;
using ImageTest.Logger;

namespace ImageTest.Logic
{
    public class BaseLogic
    {
        private LoggerWriter _loggerWriter;
        private ILogger _logger;

        public BaseLogic(ILogger logger)
        {
            _loggerWriter = new LoggerWriter();
            _logger = logger;
        }

        public List<string[]> GetAttrList(Uri url)
        {
            string data = GetData(url);

            Regex imgRegex = Constans.GetImageRegex();
            Regex fileRegex = Constans.GetFileRegex();
            Regex altRegex = Constans.GetAltRegex();

            try
            {
                var imgs = imgRegex.Matches(data)
                                   .Select(m => m.Groups["imgsrc"].Value.Trim())
                                   .Select(url => new { url, name = url.Split(new[] { '/' }).Last() })
                                   .Where(a => fileRegex.IsMatch(a.name))
                                   .Distinct()
                                   .AsParallel()
                                   .WithDegreeOfParallelism(4)
                                   .ToList();

                string[] altList = altRegex.Matches(data)
                    .Select(x => x.Groups["imgalt"].Value.Trim())
                    .ToArray();
                string[] nameList = imgs.Select(x => x.name).ToArray();
                string[] urlList = imgs.Select(x => x.url).ToArray();

                return new List<string[]>() { altList, nameList, urlList };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                _loggerWriter.WriteLog(ex.Message.ToString(), ex.StackTrace.ToString());
                Console.WriteLine(Constans.ParsingError);
                throw new Exception(Constans.ParsingError);
            }
        }

        public string GetImageDir(string savePath)
        {
            try
            {
                if (!Directory.Exists(Constans.DirectoryName))
                    Directory.CreateDirectory(Constans.DirectoryName);

                string imageDir = Constans.DirectoryName + "/" + savePath;
                if (!Directory.Exists(imageDir))
                    Directory.CreateDirectory(imageDir);

                return imageDir;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                _loggerWriter.WriteLog(ex.Message.ToString(), ex.StackTrace.ToString());
                Console.WriteLine(Constans.CreateDirError);
                throw new Exception(Constans.CreateDirError);
            }
        }

        public ImageResponseModel DownloadImage(string urlDownload, string savePath, string alt, string src)
        {
            ImageResponseModel imgResponse = new ImageResponseModel();

            try
            {
                using (WebClient localClient = new WebClient())
                {
                    localClient.DownloadFile(urlDownload, savePath);
                    FileInfo file = new FileInfo(savePath);
                    long size = file.Length;

                    imgResponse.Alt = alt;
                    imgResponse.Size = size.ToString();
                    imgResponse.Src = src;
                }

                return imgResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                _loggerWriter.WriteLog(ex.Message.ToString(), ex.StackTrace.ToString());
                Console.WriteLine(Constans.DownloadImageError);
                throw new Exception(Constans.DownloadImageError);
            }
        }

        private string GetData(Uri url)
        {
            string data = "";
            WebClient client = new WebClient();
            try
            {
                using (Stream stream = client.OpenRead(url))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        data = reader.ReadToEnd();
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                _loggerWriter.WriteLog(ex.Message.ToString(), ex.StackTrace.ToString());
                Console.WriteLine(Constans.HostError);
                throw new Exception(Constans.HostError);
            }
        }
    }
}
