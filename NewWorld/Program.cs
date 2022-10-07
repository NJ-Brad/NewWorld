using DslParser;
using System.Text.Json;
using System.Text.Json.Nodes;
using FlowDiagram;
using CommandLine;
using GanttDiagram;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Spectre.Console.Cli;
using Spectre.Console;
using System.ComponentModel;    // some of the cli attributes are in this namespace
using System.Diagnostics.CodeAnalysis;  // notnull comes from here

namespace NewWorld
{
    /*
     * Sample --FileType --list
     * Generate fileName --Filetype --ShowOutput
    */

    // The "official" version may get completed some day  -  https://dotnetcoretutorials.com/2021/01/16/creating-modern-and-helpful-command-line-utilities-with-system-commandline/
    // For now use this  -  https://github.com/commandlineparser/commandline

    [Verb("add", HelpText = "Add file contents to the index.")]
    class AddOptions
    {
        //normal options here
    }

    [Verb("sample", HelpText = "Show a sample of a file type.")]
    class SampleOptions
    {
        [Option('f', "filetype", Required = false, HelpText = "Used to specify the type of file to show a sample of.")]
        public string? Filetype { get; set; }

        [Option('o', "outputfile", Required = false, HelpText = "Specifies the name of the output file.")]
        public string? OutputFile { get; set; }

        [Option('l', "list", Required = false, HelpText = "Displays a list of samples that are available.")]
        public bool List { get; set; }
    }


    //     * Generate fileName --Filetype --ShowOutput
    [Verb("generate", HelpText = "Generates an output file.")]
    class GenerateOptions
    {
        //        [Value(0, MetaName = "InputFile", Required = true, HelpText = "Specifies the name of the input file.")]

        [Option('i', "inputfile", Required = true, HelpText = "Specifies the name of the input file.")]
        public string? InputFile { get; set; }

        [Option('o', "outputfile", Required = false, HelpText = "Specifies the name of the output file.")]
        public string? OutputFile { get; set; }

        [Option('t', "title", Required = false, HelpText = "Title to show on the output.")]
        public string? Title { get; set; }

        [Option('f', "filetype", Required = false, HelpText = "Used to specify the type of file to show a sample of.")]
        public string? Filetype { get; set; }

        [Option('s', "showoutput", Required = false, HelpText = "Indicates whether the output file should be shown (using the default browser).")]
        public bool ShowOutput { get; set; }
    }


    [Verb("commit", HelpText = "Record changes to the repository.")]
    class CommitOptions
    {
        //commit options here
    }
    [Verb("clone", HelpText = "Clone a repository into a new directory.")]
    class CloneOptions
    {
        //clone options here
    }

    internal class Program
    {
        static int Main(string[] args)
        {
            //     // this could be removed if desired
            //     // https://stackoverflow.com/questions/68633872/how-to-get-or-put-string-to-clipboard-in-c-sharp-netcore-put-string-to-clipboar
            //     TextCopy.ClipboardService.SetText(mermaidText);

            //// add any new verbs to the line below
            //return CommandLine.Parser.Default.ParseArguments<AddOptions, CommitOptions, CloneOptions, SampleOptions, GenerateOptions>(args)
            //   .MapResult(
            //     (AddOptions opts) => RunAddAndReturnExitCode(opts),
            //     (CommitOptions opts) => RunCommitAndReturnExitCode(opts),
            //     (CloneOptions opts) => RunCloneAndReturnExitCode(opts),
            //     (SampleOptions opts) => ExecuteSample(opts),
            //     (GenerateOptions opts) => ExecuteGenerate(opts),
            //     errs => 1);


            var app = new CommandApp<FileSizeCommand>();
            return app.Run(args);
        }

internal sealed class FileSizeCommand : Command<FileSizeCommand.Settings>
        {
            public sealed class Settings : CommandSettings
            {
                [Description("Path to search. Defaults to current directory.")]
                [CommandArgument(0, "[searchPath]")]
                public string? SearchPath { get; init; }

                [CommandOption("-p|--pattern")]
                public string? SearchPattern { get; init; }

                [CommandOption("--hidden")]
                [DefaultValue(true)]
                public bool IncludeHidden { get; init; }
            }

            public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
            {
                var searchOptions = new EnumerationOptions
                {
                    AttributesToSkip = settings.IncludeHidden
                        ? FileAttributes.Hidden | FileAttributes.System
                        : FileAttributes.System
                };

                var searchPattern = settings.SearchPattern ?? "*.*";
                var searchPath = settings.SearchPath ?? Directory.GetCurrentDirectory();
                var files = new DirectoryInfo(searchPath)
                    .GetFiles(searchPattern, searchOptions);

                var totalFileSize = files
                    .Sum(fileInfo => fileInfo.Length);

                AnsiConsole.MarkupLine($"Total file size for [green]{searchPattern}[/] files in [green]{searchPath}[/]: [blue]{totalFileSize:N0}[/] bytes");

                return 0;
            }
        }

        static int RunAddAndReturnExitCode(AddOptions opts)
        {
            //handle options
            return 0;
        }
        static int RunCommitAndReturnExitCode(CommitOptions opts)
        {
            //handle options
            return 0;
        }
        static int RunCloneAndReturnExitCode(CloneOptions opts)
        {
            //handle options
            return 0;
        }
        static int ExecuteSample(SampleOptions opts)
        {
            if (opts.List)
            {
                System.Console.WriteLine("flow");
                System.Console.WriteLine("Work");
                System.Console.WriteLine("c4");
            }
            return 0;
        }
        static int ExecuteGenerate(GenerateOptions opts)
        {
            //            opts.InputFile
            //if (Path.GetFileNameWithoutExtension(fileName).ToUpper() == "SELF")
            //{
            //}
            //if (Path.GetFileNameWithoutExtension(fileName).ToUpper() == "LEADER")
            //{

            if (!string.IsNullOrEmpty(opts.InputFile))
            {
                string filetype = (string.IsNullOrEmpty(opts.Filetype) ? Path.GetExtension(opts.InputFile).Substring(1) : opts.Filetype).ToUpper();

                BlockParser parser = new();
                Block block = new ();
                string mermaidText = "";

                string title = (string.IsNullOrEmpty(opts.Title) ? Path.GetFileNameWithoutExtension(opts.InputFile) : opts.Title);
                string outputFileName = (string.IsNullOrEmpty(opts.OutputFile) ? Path.ChangeExtension(FixPath(opts.InputFile), "html") : FixPath(opts.OutputFile));

                using (StreamReader sr = new(FixPath(opts.InputFile)))
                {
                    //        summarizer.LoadOther(sr);
                    //String line = sr.ReadToEnd();
                    switch (filetype)
                    {
                        case "WORK":
                            block = parser.ParseText(sr);
                            WorkItemWorkspace wiws = BlockToWorkItemsConverter.Convert(block);
                            mermaidText = WorkItemPublisher.Publish(wiws);
                            MermaidPageGenerator.Generate(mermaidText, title, outputFileName);
                            if (opts.ShowOutput)
                            {
                                OpenBrowser($"file:///{outputFileName}");
                            }
                            break;
                        case "FLOW":
                            block = parser.ParseText(sr);
                            FlowchartWorkspace fcws = BlockToFlowchartConverter.Convert(block);
                            mermaidText = FlowchartPublisher.Publish(fcws, "MERMAID");
                            MermaidPageGenerator.Generate(mermaidText, title, outputFileName);
                            if (opts.ShowOutput)
                            {
                                OpenBrowser($"file:///{outputFileName}");
                            }
                            break;
                    }

                }
            }

            return 0;
        }

        private static string FixPath(string originalPath)
        {
            string uDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            //string sampleName = "~/github/newworld/sample";
            //if(OperatingSystem.IsWindows())
            string expandedDir = originalPath.Substring(0,1) == "~" ? originalPath.Replace("~", uDir) : originalPath;
            string pName = Path.GetFullPath(expandedDir);
            return pName;
        }

        // from https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/
        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }




    }
}