using System;
using System.Diagnostics;

namespace Launcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var psi = new ProcessStartInfo("dotnet.exe");
            psi.WorkingDirectory = "./../Book/bin/Debug/netcoreapp2.0/";
            psi.Arguments = "Book.dll";
            var ps = Process.Start(psi);

            Console.ReadKey();

            ps.Kill();
        }
    }
}
