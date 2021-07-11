using System;
using ImageTest.RequestModels;
using ImageTest.Logger;

namespace ImageTest
{
    public class ConsoleApp
    {
        public ImageRequestModel EnterSource()
        {
            LoggerWriter loggerWriter = new LoggerWriter();
            ImageRequestModel image = new ImageRequestModel();
            try
            {
                Console.WriteLine("Введите URL:");
                image.Url = new Uri(Console.ReadLine());
                Console.WriteLine("Введите кол-во потоков:");
                image.ThrCount = Convert.ToInt32(Console.ReadLine());
                if (image.ThrCount < Environment.ProcessorCount)
                    image.ThrCount += Environment.ProcessorCount - image.ThrCount;

                Console.WriteLine("Введите кол-во изображений:");
                image.ImgCount = Convert.ToInt32(Console.ReadLine());
                return image;
            }
            catch (Exception ex)
            {
                loggerWriter.WriteLog(ex.Message.ToString(), ex.StackTrace.ToString());
                Console.WriteLine("Неверный ввод");
                image.Url = null;
                image.ThrCount = 1;
                image.ImgCount = 0;
                return EnterSource();
            }
        }
    }
}