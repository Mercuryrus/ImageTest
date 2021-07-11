using System;

namespace ImageTest.RequestModels
{
    public class ImageRequestModel
    {
        public Uri Url { get; set; }
        public int ThrCount { get; set; }
        public int ImgCount { get; set; }
    }
}