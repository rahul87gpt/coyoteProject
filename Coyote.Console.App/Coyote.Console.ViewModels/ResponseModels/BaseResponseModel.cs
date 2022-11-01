﻿namespace Coyote.Console.ViewModels.ResponseModels
{
    public class BaseResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public object Data { get; set; }
    }
}
