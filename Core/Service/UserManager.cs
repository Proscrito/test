using System;
using System.Collections.Generic;
using System.Linq;
using yamvc.Models;

namespace yamvc.Core.Service
{
    public class UserManager
    {
        private static readonly Lazy<UserManager> lazy = new Lazy<UserManager>(() => new UserManager());

        public static UserManager Instance => lazy.Value;

        private List<ExtendedUserModel> LoggedInUsers { get; }
        private readonly object _lock = new object();

        private UserManager()
        {
            LoggedInUsers = new List<ExtendedUserModel>();
        }

        public IEnumerable<ExtendedUserModel> GetLoggedInUsers()
        {
            return LoggedInUsers;
        }

        public void AddUser(ExtendedUserModel user)
        {
            lock (_lock)
            {
                LoggedInUsers.Add(user);
            }
        }

        public void RemoveUser(string login)
        {
            lock (_lock)
            {
                var userToRemove = Find(login);

                if (userToRemove != null)
                {
                    LoggedInUsers.Remove(userToRemove);
                }
            }
        }

        public void RemoveUser(ExtendedUserModel user)
        {
            RemoveUser(user.Login);
        }

        public void AssignSocket(string login, string socketId)
        {
            lock (_lock)
            {
                var user = Find(login);

                if (user != null)
                    user.SocketId = socketId;
            }
        }

        public ExtendedUserModel Find(string login)
        {
            return LoggedInUsers.FirstOrDefault(x => x.Login.Equals(login, StringComparison.InvariantCultureIgnoreCase));
        }

        public void DisconnectUser(string login)
        {
            RemoveUser(login);
        }
    }
}
