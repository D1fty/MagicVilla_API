namespace MagicVilla_VillaAPI.Logging
{
    public class Logging : iLogging
    {
        public void Log(string message, eLogLevel type)
        {
            switch(type)
            {
                case eLogLevel.Error:
                    Console.WriteLine($"ERROR - { message } ");

                    break;

                default: 
                    Console.Write(message);
                    break;
            }
        }
    }
}
