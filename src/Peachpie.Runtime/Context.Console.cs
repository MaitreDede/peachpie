﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Pchp.Core.Utilities;

namespace Pchp.Core
{
    partial class Context
    {
        /// <summary>
        /// Implementation of a console application runtime context.
        /// </summary>
		sealed class ConsoleContext : Context
        {
            public ConsoleContext(params string[] args)
            {
                RootPath = Directory.GetCurrentDirectory();
                WorkingDirectory = RootPath;

                //
                InitOutput(Console.OpenStandardOutput(), Console.Out);

                // Globals
                InitSuperglobals();
                IntializeArgvArgc(args);
            }
        }

        /// <summary>Initializes global $argv and $argc variables.</summary>
        protected void IntializeArgvArgc(params string[] args)
        {
            Debug.Assert(args != null);

            // command line argc, argv:
            var argv = new PhpArray(args.Length);

            // adds all arguments to the array (the 0-th argument is not '-' as in PHP but the program file):
            for (int i = 0; i < args.Length; i++)
            {
                argv.Add(i, PhpValue.Create(args[i]));
            }

            this.Globals["argv"] = PhpValue.Create(argv);
            this.Globals["argc"] = PhpValue.Create(args.Length);
        }

        /// <summary>
        /// Creates context to be used within a console application.
        /// </summary>
        public static Context CreateConsole(params string[] args)
        {
            return new ConsoleContext(args);
        }
    }
}
