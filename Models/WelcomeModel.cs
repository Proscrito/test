using System.Collections.Generic;
using yamvc.Core.Service;

namespace yamvc.Models
{
    public class WelcomeModel
    {
        public string Login { get; set; }
        public bool IsAdmin { get; set; }
        public IEnumerable<ExtendedUserModel> LoggedInUsers => UserManager.Instance.GetLoggedInUsers();
    }
}
