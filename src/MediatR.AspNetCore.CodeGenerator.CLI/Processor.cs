using MediatR.AspNetCore.CodeGenerator.CLI.Analyzers;
using MediatR.AspNetCore.CodeGenerator.CLI.Analyzers.Objects;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MediatR.AspNetCore.CodeGenerator.CLI
{
    class Processor
    {
        private readonly MSBuildWorkspace workspace;
        private readonly List<Request> allRequest = new();
        private readonly Renderer renderer = new();

        public Processor()
        {
            this.workspace = MSBuildWorkspace.Create();
        }

        public async Task LoadSolution(string solutionPath)
        {
            var solution = await workspace.OpenSolutionAsync(solutionPath);

            foreach (var project in solution.Projects)
            {
                var compilation = await TryGetCompilationAsync(project);
                if (compilation != null)
                {
                    var requests = compilation.GetRequests();
                    Console.WriteLine($"Project: \"{project.Name}\" ({requests.Count})");
                    allRequest.AddRange(requests);
                }
            }

            Console.WriteLine($"Total: {allRequest.Count}");
        }

        public async Task<bool> Render(string outputDir, string ns)
        {
            var grouped = allRequest
                    .GroupBy(x => x.GroupName)
                    .ToDictionary(x => x.Key, x => x.ToArray());

            foreach (var group in grouped)
            {
                var groupName = group.Key.Replace("\"", "").Split("/");
                var className = groupName.Last() + "Controller";

                var finalNS = ns;
                if (groupName.Length > 1)
                    finalNS += "." + string.Join('.', groupName[0..^1]);

                var generated = this.renderer.RenderController(finalNS, className, group.Value.ToList());
                var outPath = Path.Join(outputDir, $"{string.Join('_', groupName)}Controller.cs");

                await File.WriteAllTextAsync(outPath, generated);
            }

            return true;
        }

        private static async Task<Compilation> TryGetCompilationAsync(Project project)
        {
            try
            {
                var compilation = await project.GetCompilationAsync();
                if (compilation != null)
                    return compilation;
            }
            catch (Exception)
            {
                // TODO: Log exception
            }

            return null;
        }
    }
}
