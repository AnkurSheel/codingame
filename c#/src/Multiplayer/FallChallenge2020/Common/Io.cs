using System;
using System.IO;

namespace FallChallenge2020.Common
{
    public static class Io
    {
        private static StreamReader _file;

        public static void Initialize()
        {
            if (Constants.IsLocalRun)
            {
                _file = new StreamReader(@".\in.txt");
            }
        }

        public static void Debug(string output)
        {
            if (Constants.IsDebugOn || Constants.IsForInput)
            {
                Console.Error.WriteLine(output);
            }
        }

        public static void WriteLine(string output)
        {
            Console.WriteLine(output);
        }

        public static string ReadLine()
        {
            if (Constants.IsLocalRun)
            {
                return _file.ReadLine();
            }
            else
            {
                var input = Console.ReadLine();

                if (Constants.IsForInput)
                {
                    Debug("IN");
                    Debug(input);
                    Debug("/IN");
                }
                else
                {
                    Debug(input);
                }

                return input;
            }
        }
    }
}
