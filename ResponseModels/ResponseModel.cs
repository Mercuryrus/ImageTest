using ImageTest.Logic;
using System.Collections.Generic;

namespace ImageTest.ResponseModels
{
    public class ResponseModel
    {
        public string CompletedProgram { get; set; }
        public string Host { get; set; }
        public List<ImageResponseModel> Images { get; set; }

        public ResponseModel()
        {
            CompletedProgram = Constans.Successfull;
        }
    }
}
