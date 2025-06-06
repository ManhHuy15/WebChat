using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class ResponseDTO
    {
        public HttpStatusCode status { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }
}
