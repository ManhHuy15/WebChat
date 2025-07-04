using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class BaseDAO
    {
        public readonly WebChatContext _context;
        public BaseDAO(WebChatContext context)
        {
            _context = context;
        }
    }
}
