using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ImageTest.ResponseModels;
using ImageTest.RequestModels;
using ImageTest.Logger;
using ImageTest.Logic;

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
        [Route("GetImages")]
        public List<ResponseModel> GetImages(ImageRequestModel request)
        {
            var start = DateTime.Now;
            LoggerWriter loggerWriter = new LoggerWriter();

            try
            {
                ThreadPool.SetMaxThreads(request.ThrCount, request.ThrCount);
                string baseUrl = request.Url.GetLeftPart(UriPartial.Authority);

                ResponseModel response = new ResponseModel();
                response.Host = baseUrl;
                response.Images = new List<ImageResponseModel>();

                BaseLogic logic = new BaseLogic(_logger);
                string imageDir = logic.GetImageDir(request.Url.Host);
                var attrList = logic.GetAttrList(request.Url);

                string[] altList = attrList[Constans.AltListIIndex];
                string[] nameList = attrList[Constans.NameListIIndex];
                string[] urlList = attrList[Constans.UrlListIIndex];

                int count = request.ImgCount >= urlList.Length
                    ? urlList.Length
                    : request.ImgCount;


                for (int i = 0; i < count; i++)
                {
                    string savePath = Path.Combine(imageDir, nameList[i]);
                    string currentAlt = (altList.Length <= i)
                        ? ""
                        : altList[i];

                    ImageResponseModel imgResponse;

                    if (urlList[i][0] == '/' && urlList[i][1] == '/')
                    {
                        imgResponse = logic.DownloadImage(urlList[i].Substring(2, urlList[i].Length - 2), savePath, currentAlt, baseUrl + urlList[i]);
                        response.Images.Add(imgResponse);
                        continue;
                    }

                    if (urlList[i].Contains("https://") || urlList[i].Contains("http://") || urlList[i].Contains("www.") || urlList[i].Contains("\\"))
                    {
                        imgResponse = logic.DownloadImage(urlList[i], savePath, currentAlt, baseUrl + "/" + urlList[i]);
                        response.Images.Add(imgResponse);
                        continue;
                    }

                    imgResponse = logic.DownloadImage(baseUrl + "/" + urlList[i], savePath, currentAlt, baseUrl + "/" + urlList[i]);
                    response.Images.Add(imgResponse);

                    if ((start.AddSeconds(Constans.MaxRunTimeInSeconds) > DateTime.Now) && (i + 1 < count))
                    {
                        response.CompletedProgram = Constans.TimeError;
                        break;
                    }
                }
                Console.WriteLine("Загрузка завершена!");
                return new List<ResponseModel>() { response };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                loggerWriter.WriteLog(ex.Message.ToString(), ex.StackTrace.ToString());
                Console.WriteLine(Constans.GetImagesError);
                throw new Exception(Constans.GetImagesError);
            }
        }
    }
}