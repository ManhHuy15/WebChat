using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class ResponseDTO<T>
    {
        public HttpStatusCode status { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public T data { get; set; }
    }
}
