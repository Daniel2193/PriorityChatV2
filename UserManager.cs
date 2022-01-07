using System.Collections.Generic;
using System.Drawing;
namespace PriorityChatV2
{
    public class UserManager
    {
        public static List<User> userList = new List<User>();
        public static void UpdateUser(string username, Status status, Color color)
        {
            if(userList.Exists(x => x.Username == username))
            {
                userList.Find(x => x.Username == username).Status = status;
                userList.Find(x => x.Username == username).Color = color;
            }
            else
            {
                userList.Add(new User() { Username = username, Status = status, Color = color });
            }
        }
        public static void UpdateStatus(string username, Status status)
        {
            if (userList.Exists(x => x.Username == username))
            {
                userList.Find(x => x.Username == username).Status = status;
            }
            else
            {
                userList.Add(new User() { Username = username, Status = status , Color = Color.Blue});
            }
        }
        public static void UpdateColor(string username, Color color)
        {
            if (userList.Exists(x => x.Username == username))
            {
                userList.Find(x => x.Username == username).Color = color;
            }
            else
            {
                userList.Add(new User() { Username = username, Status = Status.ONLINE, Color = color });
            }
        }
        public static void setup(){
            userList.Clear();
            userList.Add(new User() { Username = ConfigManager.getConfig().username, Status = Status.ONLINE, Color = ConfigManager.getConfig().colorUsername });
        }
    }
}
