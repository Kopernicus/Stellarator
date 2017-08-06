/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

using System;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Stellarator
{
    public static class Program
    {
        /// <summary>
        ///     The entrypoint for our application.
        /// </summary>
        /// <param name="args"></param>
        public static int Main(string[] args)
        {
            // Say hello
            Console.WriteLine("-------------------------------------------------------------");
            Console.WriteLine("Stellar Generator - Creates procedural systems for Kopernicus");
            Console.WriteLine("Copyright (c) 2016 Thomas P.");
            Console.WriteLine("Licensed under the Terms of the MIT License");
            Console.WriteLine("Version: 0.2");
            Console.WriteLine("-------------------------------------------------------------");

            // Can I have some tea?
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            //TODO: MartinX3 Change How we get the args. Only show question, if arg is not given.
            // Ask for Input
            var seed = Prompt("Please enter the seed you want to use: ", "--seed");
            var folder = Prompt("Please choose a folder name for your system: ", "--name");
            var systematic = Prompt("Use systematic planet names? (y/n) ", "--systematic", true);
            Console.WriteLine();

            //TODO: Is there a better way? y/n
            // Check
            if ((systematic != "y") && (systematic != "n"))
            {
                Console.WriteLine("Invalid Input! Enter either y or n.");
                return -1;
            }

            // Generate the System
            Generator.Generate(seed, folder, systematic == "y");

            // Log
            Console.WriteLine("Generation has finished. Program is exiting.");
            return 0;
        }

        /// <summary>
        ///     Asks the user something and returns an answer.
        /// </summary>
        private static string Prompt(string prompt, string cmdLine, bool key = false)
        {
            //TODO: MartinX3 Change How we get the args. Only show question, if arg is not given.
            var args = Environment.GetCommandLineArgs();
            if (args.Any(s => s.Trim().StartsWith(cmdLine)))
            {
                var arg = args.First(s => s.Trim().StartsWith(cmdLine));
                arg = arg.Trim().Remove(0, (cmdLine + ":").Length);
                Console.WriteLine(prompt + arg);
                return arg;
            }
            Console.Write(prompt);
            return key ? Console.ReadKey().KeyChar.ToString() : Console.ReadLine();
        }
    }
}