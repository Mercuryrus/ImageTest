using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageTest.ResponseModels
{
    public class ResponseModel
    {
        public string Host { get; set; }
        public List<ImageResponseModel> Images { get; set; }
    }
}
