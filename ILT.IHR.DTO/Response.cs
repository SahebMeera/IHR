using System;
using System.Collections.Generic;
using System.Text;

namespace ILT.IHR.DTO
{
    public class Response<T>
    {
        public MessageType MessageType { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public enum MessageType
    {
        None,
        Success,
        Error,
        Warning
    }
}
