using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Data
{
    public class UserRepository
    {
        private string _connectionstring;

        public UserRepository(string connectionString)
        {
            _connectionstring = connectionString;
        }

        public void AddUser(User user, string password)
        {
            string salt = PasswordHelper.GenerateSalt();
            string passwordHash = PasswordHelper.HashPassword(password, salt);
            user.PasswordSalt = salt;
            user.PasswordHash = passwordHash;
            using (var context = new UrlShortenerDataContext(_connectionstring))
            {
                context.Users.InsertOnSubmit(user);
                context.SubmitChanges();
            }
        }

        public User LogIn(string email, string password)
        {
            User user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }
            bool isCorrectPassword = PasswordHelper.PasswordMatch(password, user.PasswordSalt, user.PasswordHash);
            if (!isCorrectPassword)
            {
                return null;
            }

            return user;
        }

        public User GetByEmail(string email)
        {
            using (var context = new UrlShortenerDataContext(_connectionstring))
            {
                return context.Users.FirstOrDefault(u => u.EmailAddress == email);
            }
        }
    }
}
