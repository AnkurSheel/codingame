//#define LOCAL

#define DEBUGON

//#define FORINPUT


using System;
using System.IO;

namespace Common.Services
{
    public static class Io
    {
        private static StreamReader _file;

        public static void Initialize()
        {
#if (LOCAL)
            _file = new StreamReader(@".\in.txt");
#endif
        }

        public static void Debug(string output)
        {
#if DEBUGON
            Console.Error.WriteLine(output);
#endif
        }

        public static void WriteLine(string output)
        {
            Console.WriteLine(output);
        }

        private static string ReadLine()
        {
#if LOCAL
            return _file.ReadLine();
#else
            var input = Console.ReadLine();
#if FORINPUT
            Debug("IN");
            Debug(input);
            Debug("/IN");
#else
            Debug(input);
#endif
            return input;
#endif
        }
    }
}