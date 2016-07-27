/**
 * Stellar Generator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarGenerator
{
    public class Program
    {
        /// <summary>
        /// The entrypoint for our application.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            // Say hello
            Console.WriteLine("-------------------------------------------------------------");
            Console.WriteLine("Stellar Generator - Creates procedural systems for Kopernicus");
            Console.WriteLine("Copyright (c) 2016 Thomas P.");
            Console.WriteLine("Licensed under the Terms of the MIT License");
            Console.WriteLine("Version: 1.0");
            Console.WriteLine("-------------------------------------------------------------");

            // Ask for Input
            Int32 seed = Prompt("Please enter the Seed you want to use: ").GetHashCode();
            String folder = Prompt("Please choose a folder name for your system: ");

            // Generate the System
            Generator.Generate(seed, folder);
        }

        /// <summary>
        /// Asks the user something and returns an answer.
        /// </summary>
        public static String Prompt(String prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
    }
}
