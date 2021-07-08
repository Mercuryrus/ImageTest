using System;
using ImageTest.RequestModels;

namespace ImageTest
{
    public class ConsoleApp
    {
        
        public ImageRequest EnterSource()
        {
            ImageRequest image = new ImageRequest();
            try
            {
                Console.WriteLine("Введите URL:");
                image.Url = new Uri(Console.ReadLine());
                Console.WriteLine("Введите кол-во потоков:");
                image.ThreadCount = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Введите кол-во изображений:");
                image.ImageCount = Convert.ToInt32(Console.ReadLine());
            }
            catch(UriFormatException)
            {
                Console.WriteLine("Неверный формат ссылки или ссылка не введена");
            }
            catch (FormatException)
            {
                Console.WriteLine("Неверный ввод числа");
            }
            catch(OverflowException)
            {
                Console.WriteLine("Слишком большое число");
            }
            
            return image;
        }        
    }
}