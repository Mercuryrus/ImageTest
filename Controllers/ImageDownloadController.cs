using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using ImageTest.ResponseModels;
using ImageTest.RequestModels;
using ImageTest.Logger;
using System.Threading.Tasks;

namespace ImageTest.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class ImageDownloadController : ControllerBase
    {
        private readonly ILogger _logger;
        public ImageDownloadController(ILogger<ImageDownloadController> logger)
        {
            _logger = logger;
        }
        
        [HttpPost]
        [Route("DownloadFiles")]
        public List<ResponseModel> DownloadFiles()
        {
            LoggerWriter loggerWriter = new LoggerWriter();
            ConsoleApp consoleApp = new ConsoleApp();
            ImageRequest image = consoleApp.EnterSource();
            ResponseModel response = new ResponseModel();
            List<FileResponseModel>fileResponse = new List<FileResponseModel>();
            ThreadPool.SetMaxThreads(image.ThrCount, image.ThrCount);

            int workerThreds;
            int completionPortThreads;
            ThreadPool.GetAvailableThreads(out workerThreds, out completionPortThreads);
            Console.WriteLine($"available threads = {workerThreds} / {completionPortThreads}");
            string directory = "images";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            WebClient client = new WebClient();

            string data= "";
            try
            {
                using (Stream stream = client.OpenRead(image.Url))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        data = reader.ReadToEnd();
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                loggerWriter.WriteLog(ex.Message.ToString());
                Console.WriteLine("Ошибка хоста");
            }

            string baseUrl = image.Url.GetLeftPart(UriPartial.Authority);
            Regex imgRegex = new Regex(@"\<img.+?src=\""(?<imgsrc>.+?)\"".+?\>", RegexOptions.ExplicitCapture);
            MatchCollection matches = imgRegex.Matches(data);
            Regex fileRegex = new Regex(@"[^\s\/]\.(jpg|png|gif|bmp)\z", RegexOptions.Compiled);
                
            var imgs = imgRegex.Matches(data)
                                   .Select(m => m.Groups["imgsrc"].Value.Trim())
                                   .Select(url => new { url, name = url.Split(new[] { '/' }).Last() })
                                   .Where(a => fileRegex.IsMatch(a.name))
                                   .Distinct()
                                   .AsParallel()
                                   .WithDegreeOfParallelism(1)
                                   .ToList();

            Regex altRegex = new Regex(@"\<img.+?alt=\""(?<imgalt>.+?)\"".+?\>", RegexOptions.ExplicitCapture);
            MatchCollection matchAlt = altRegex.Matches(data);

            string[] altList = matchAlt.Select(x => x.Groups["imgalt"].Value.Trim()).ToArray();
            string[] nameList = imgs.Select(x => x.name).ToArray();
            string[] urlList = imgs.Select(x => x.url).ToArray();
            
            int count = 0;
            
            if (image.ImgCount >= imgs.Count)
            { count = imgs.Count; }
            else 
            { count = image.ImgCount; }

            string imageDir = directory + "/" + image.Url.Host;
            if (!Directory.Exists(imageDir))
            {
                Directory.CreateDirectory(imageDir);
            }
            response.Host = baseUrl;
            response.Images = new List<ImageResponseModel>();
            for (int i = 0; i < count; i++)
            {
                long size = 0;
                string savePath = Path.Combine(imageDir, nameList[i]);
                ImageResponseModel imgResponse = new ImageResponseModel();
                try
                {
                    if (urlList[i].Contains("https://") || urlList[i].Contains("http://") || urlList[i].Contains("www.") || urlList[i].Contains("\\"))
                    {

                        using (WebClient localClient = new WebClient())
                        {
                            localClient.DownloadFileTaskAsync(urlList[i], savePath);
                            int w, c;
                            ThreadPool.GetAvailableThreads(out w, out c);
                            Console.WriteLine($"{w} {c}");
                            FileInfo file = new FileInfo(savePath);
                            size = file.Length;
                            imgResponse.Size = file.Length.ToString();
                            imgResponse.Src = baseUrl + urlList[i];
                            imgResponse.Alt = altList[i];
                        }
                    }
                    else
                    {

                        using (WebClient localClient = new WebClient())
                        {
                            localClient.DownloadFileTaskAsync(baseUrl + urlList[i], savePath);
                            FileInfo file = new FileInfo(imageDir);
                            size = file.Length;
                            imgResponse.Size = file.Length.ToString();
                            imgResponse.Src = baseUrl + urlList[i];
                            imgResponse.Alt = altList[i];
                        }

                    }
                    response.Images.Add(imgResponse);
                    Console.WriteLine($"{nameList[i]} загружен\n Размер: {size} байт");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message.ToString());
                    loggerWriter.WriteLog(ex.Message.ToString());
                }
            }
            Console.WriteLine("Загрузка завершена!");
            return new List<ResponseModel>() { response };
        }
    }
}