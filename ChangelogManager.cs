using System;

namespace PriorityChatV2
{
    public class ChangelogManager
    {
        public static FormChangelog changelog = new FormChangelog();
        public static void setup()
        {
            changelog.addEntry("2.3.0", "Added changelog", ChangelogType.ADDED);
            changelog.addEntry("2.3.0", "Added colors to messages", ChangelogType.ADDED);
            changelog.addEntry("2.3.0", "Structural Rework", ChangelogType.CHANGED);
            changelog.addEntry("2.3.0", "Fixed flash", ChangelogType.CHANGED);
            changelog.addEntry("2.3.1", "Flash now includes Windows internal flash", ChangelogType.CHANGED);
            changelog.addEntry("2.3.2", "(temporary) removed colored messages", ChangelogType.REMOVED);
            changelog.addEntry("2.3.3", "Messages can now longer render invisible ", ChangelogType.CHANGED);
            changelog.addEntry("2.3.4", "Added auto-scroll to chat", ChangelogType.ADDED);
            changelog.addEntry("2.3.5", "Slight changelog changes", ChangelogType.CHANGED);
            changelog.addEntry("2.3.6", "Quit button works again", ChangelogType.CHANGED);
            changelog.addEntry("2.3.6", "Fixed usernames not showing up in chat", ChangelogType.CHANGED);
            changelog.addEntry("2.3.7", "DLL dependency by shipping it inside the exe file", ChangelogType.ADDED);
            changelog.addEntry("2.4.0", "Settings window will closed automatically after clicking on \"apply\"", ChangelogType.ADDED);
            changelog.addEntry("2.4.0", "Added message timestamps", ChangelogType.ADDED);
            changelog.addEntry("2.4.0", "Added adaptive UI scaling", ChangelogType.ADDED);
            changelog.addEntry("2.5.0", "Added \"Send on Enter\" feature to the settings", ChangelogType.ADDED);
            changelog.addEntry("2.5.0", "{WIP} change changelog POG", ChangelogType.CHANGED);


            changelog.addEntry("2.5.X", "Fix colored messages", ChangelogType.PLANNED);
            changelog.addEntry("2.5.X", "{WIP} Added Userlist/Userstatus", ChangelogType.PLANNED);
            changelog.addEntry("2.5.X", "Added Self-updating", ChangelogType.PLANNED);
            changelog.addEntry("2.5.X", "Added support for Smant emotes", ChangelogType.PLANNED);
        }
    }
}