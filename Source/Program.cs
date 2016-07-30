/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Stellarator
{
    public class Program
    {
        /// <summary>
        /// The entrypoint for our application.
        /// </summary>
        /// <param name="args"></param>
        public static Int32 Main(string[] args)
        {
            // Say hello
            Console.WriteLine("-------------------------------------------------------------");
            Console.WriteLine("Stellar Generator - Creates procedural systems for Kopernicus");
            Console.WriteLine("Copyright (c) 2016 Thomas P.");
            Console.WriteLine("Licensed under the Terms of the MIT License");
            Console.WriteLine("Version: 1.0");
            Console.WriteLine("-------------------------------------------------------------");

            // Can I have some tea?
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            // Ask for Input
            String seed = Prompt("Please enter the Seed you want to use: ");
            String folder = Prompt("Please choose a folder name for your system: ");
            String systematic = Prompt("Use systematic planet names? (y/n) ", true);
            Console.WriteLine();

            // Check
            if (systematic != "y" && systematic != "n")
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
        /// Asks the user something and returns an answer.
        /// </summary>
        public static String Prompt(String prompt, Boolean key = false)
        {
            Console.Write(prompt);
            return key ? Console.ReadKey().KeyChar.ToString() : Console.ReadLine();
        }
    }
}
