namespace TagTool.Data.Secrets
{
    /* 
    This file contains API Keys, and allows private keys to be hidden when open-
    sourcing the project.
    */
    public static class GetKey
    {
         
        // Google Maps Api Key
        public static string GoogleAPIKey()
        {
            return "";
        }

        // What Three Words API Key
        public static string What3WordsAPIKey(){return "";}

        // Mailgun API Key
        public static string MailgunAPIKey()
        {
            return "";
        }

        // Testmail API Key
        public static string TestmailAPIKey()
        {
            return "";
        }

        // Highmaps API Key
        public static string HighmapsAPIKey(){return "";}

        // Database Connection
        /*
        In appsettings.json (under TagTool.Web) modify remote DB to your 
        connection string for database of choice. 
        */
    }
}