using System.IO;

namespace Halite2.hlt
{
    public class DebugLog
    {
        private TextWriter _file;
        private static DebugLog _instance;

        private DebugLog(TextWriter f)
        {
            _file = f;
        }

        public static void Initialize(TextWriter f)
        {
            _instance = new DebugLog(f);
        }

        public static void AddLog(string message)
        {
            try
            {
                _instance._file.WriteLine(message);
                _instance._file.Flush();
            }
            catch (IOException) { }
        }
    }
}
