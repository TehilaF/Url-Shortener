using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UrlShortener.Data;

namespace UrlShortener.Models
{
    public class UserHistoryViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<URL> Urls { get; set; }
    }
}