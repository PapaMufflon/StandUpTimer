using System;
using System.IO;
using System.Reflection;

namespace ReplaceVersionString
{
    class Program
    {
        static void Main(string[] args)
        {
            var version = Assembly.LoadFrom(args[0]).GetName().Version.ToString();
            var content = File.ReadAllText(args[1]);

            content = content.Replace(args[2], version);

            File.WriteAllText(args[1], content);

            Console.Write(version);
        }
    }
}