using System;
using System.Collections.Generic;

namespace ImageTest.RequestModels
{
    public class ImageRequest
    {
        public Uri Url { get; set; }
        public int ThreadCount { get; set; }
        public int ImageCount { get; set; }
    }
}