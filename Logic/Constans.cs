using System.Text.RegularExpressions;

namespace ImageTest.Logic
{
    public static class Constans
    {
        public static readonly string DirectoryName = "images";

        public static readonly int AltListIIndex = 0;
        public static readonly int NameListIIndex = 1;
        public static readonly int UrlListIIndex = 2;

        public static readonly int MaxRunTimeInSeconds = 60;

        #region Regexs
        public static Regex GetImageRegex() => new Regex(@"\<img.+?src=\""(?<imgsrc>.+?)\"".+?\>", RegexOptions.ExplicitCapture);
        public static Regex GetFileRegex() => new Regex(@"[^\s\/]\.(jpg|png|gif|bmp)\z", RegexOptions.Compiled);
        public static Regex GetAltRegex() => new Regex(@"\<img.+?alt=\""(?<imgalt>.+?)\"".+?\>", RegexOptions.ExplicitCapture);
        #endregion

        #region Exception Name
        public static readonly string GetImagesError = "Ошибка получения картинки в контроллере";

        public static readonly string HostError = "Ошибка хоста";
        public static readonly string ParsingError = "Ошибка парсинга данных";
        public static readonly string CreateDirError = "Ошибка создания директории";
        public static readonly string DownloadImageError = "Ошибка загрузки картинки или ее сохранения";

        public static readonly string Successfull = "Программа завершена успешно";
        public static readonly string TimeError = "Программа была завершена за 1 минуту, есть нескаченные файлы";
        #endregion
    }
}
