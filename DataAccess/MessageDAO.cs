using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class MessageDAO
    {
        private readonly WebChatContext _context;
        public MessageDAO(WebChatContext context)
        {
            _context = context;
        }

        public IQueryable<Message> GetListMessageByCondition(Expression<Func<Message, bool>> condition)
        {
            try
            {
                return _context.Messages.Include(message => message.Sender)
                                        .Include(message => message.Receiver)
                                        .Include(message => message.Group)
                                        .Include(message => message.MessageStatuses)
                                        .Where(condition).AsQueryable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AddRange(List<Message> message)
        {
            try
            {
                _context.Messages.AddRange(message);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

