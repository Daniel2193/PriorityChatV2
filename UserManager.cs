using System.Collections.Generic;

namespace PriorityChatV2
{
    public class UserManager
    {
        public static Dictionary<string, Status> users = new Dictionary<string, Status>();
        public static void UpdateUser(string username, Status status)
        {
            if(users.ContainsKey(username))
            {
                users[username] = status;
            }
            else
            {
                users.Add(username, status);
            }
        }
    }
}
