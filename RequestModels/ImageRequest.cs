using System;
using System.Collections.Generic;

namespace ImageTest.RequestModels
{
    public class ImageRequest
    {
        public Uri Url { get; set; }
        public int ThrCount { get; set; }
        public int ImgCount { get; set; }
    }
}