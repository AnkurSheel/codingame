using System;
using System.IO;


 // 07/05/2020 08:12


namespace JoinThePac
{
    public static class Constants
    {
        public static readonly Random Random = new Random(123);
    }
}

namespace JoinThePac
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
#define LOCAL

#define DEBUGON

#define FORINPUT



namespace JoinThePac.Services
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
            return input;
#endif

#endif
        }
    }
}