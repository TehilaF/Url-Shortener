using shortid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Data
{
    public class ActionsRepository
    {
        private string _connectionstring;

        public ActionsRepository(string connectionString)
        {
            _connectionstring = connectionString;
        }

        public void AddUrl(URL url)
        {
            using (var context = new UrlShortenerDataContext(_connectionstring))
            {
                context.URLs.InsertOnSubmit(url);
                context.SubmitChanges();
            }
        }

        public URL GetUrl(string userEmail, string url)
        {
            using (var context = new UrlShortenerDataContext(_connectionstring))
            {
                return context.URLs.FirstOrDefault(u => u.User.EmailAddress == userEmail && u.UrlOrig == url);
            }
        }

        public URL Get(string shortUrl)
        {
            using (var context = new UrlShortenerDataContext(_connectionstring))
            {
                return context.URLs.FirstOrDefault(u => u.UrlShort.ToLower() == shortUrl.ToLower());
            }
        }

        public void IncrementViews(int urlId)
        {
            using (var context = new UrlShortenerDataContext(_connectionstring))
            {
                context.ExecuteCommand("UPDATE URLs SET Views = Views + 1 WHERE ID = {0}", urlId);
            }
        }

        public IEnumerable<URL> GetHistory(string email)
        {
            using (var context = new UrlShortenerDataContext(_connectionstring))
            {
                return context.URLs.Where(u => u.User.EmailAddress == email).ToList();
            }
        }
    }
}
