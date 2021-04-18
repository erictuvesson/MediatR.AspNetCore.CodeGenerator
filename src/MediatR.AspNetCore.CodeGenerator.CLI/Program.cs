using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

namespace MediatR.AspNetCore.CodeGenerator.CLI
{

    class Program
    {
        static int Main(string[] args)
        {
            MSBuildLocator.RegisterDefaults();

            var rootCommand = new RootCommand("MediatR.AspNetCore.CodeGenerator.CLI")
            {
                new Option<string>(
                    new string[] { "-s", "--solution" },
                    ""
                ),
                new Option<string>(
                    new string[] { "-o", "--output" },
                    getDefaultValue: () => Directory.GetCurrentDirectory(),
                    "The output directory."
                ),
                new Option<string>(
                    new string[] { "--out-namespace" },
                    getDefaultValue: () => "GeneratedNamespace",
                    "The output controller namespace."
                )
            };

            rootCommand.Handler = CommandHandler.Create<string, string, string>(async (solution, output, outNamespace) =>
            {
                if (!File.Exists(solution))
                {
                    Console.WriteLine($"Unable to find solution: \"{solution}\".");
                    return 1;
                }

                if (!Directory.Exists(output))
                {
                    Console.WriteLine($"Unable to find output directory: \"{output}\".");
                    return 1;
                }

                using var workspace = MSBuildWorkspace.Create();

                var processor = new Processor();

                Console.WriteLine("Loading solution...");
                await processor.LoadSolution(solution);
                Console.WriteLine("Loading complete.");

                Console.WriteLine("Rendering...");
                await processor.Render(output, outNamespace);
                Console.WriteLine("Rendering complete.");

                return 0;
            });

            return rootCommand.InvokeAsync(args).Result;
        }
    }
}
