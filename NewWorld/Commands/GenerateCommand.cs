using C4Diagram;
using DslParser;
using FlowDiagram;
using GanttDiagram;
using MarkdownConverter;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
//using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NewWorld.Commands
{
    internal class GenerateCommand : Command<GenerateCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [Description("The name of the input file.")]
            [CommandArgument(0, "[InputFile]")]
            public string? InputFile { get; set; }

            [Description("The name of the output file.")]
            [CommandOption("-o|--outputfile")]
            public string? OutputFile { get; init; }    // init; vs set; ?

            [Description("Title to show on the output.")]
            [CommandOption("-t|--title")]
            public string? Title { get; init; }    // init; vs set; ?


            [Description("Specify the type of file. (Defaults to using the input file's extension.")]
            [CommandOption("-f|--filetype")]
            public string? FileType { get; init; }    // init; vs set; ?

            //[CommandOption("-p|--pattern")]
            //public string? SearchPattern { get; init; }

            //[CommandOption("--hidden")]
            //[DefaultValue(true)]
            //public bool IncludeHidden { get; init; }

            [Description("Indicates whether the output file should be shown (using the default browser).")]
            [CommandOption("-s|--showoutput")]
            [DefaultValue(false)]
            public bool ShowOutput { get; init; }

            // ValidationResult(String, IEnumerable<String>)
            // this uses Spectre.Console.ValidationResult
            public override ValidationResult Validate()
            {
                ValidationResult vr = ValidationResult.Success();
                //vr.Message
                //vr.Successful

                //ValidationResult vr = ValidationResult.Error("Message");

                if (string.IsNullOrEmpty(InputFile))
                {
                    vr = ValidationResult.Error("Input file is required");

                }

                return vr;
            }
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
                string filetype = (string.IsNullOrEmpty(settings.FileType) ? Path.GetExtension(settings.InputFile).Substring(1) : settings.FileType).ToUpper();

                BlockParser parser = new();
                Block block = new();
                string mermaidText = "";

                string title = (string.IsNullOrEmpty(settings.Title) ? Path.GetFileNameWithoutExtension(settings.InputFile) : settings.Title);
                string outputFileName = (string.IsNullOrEmpty(settings.OutputFile) ? Path.ChangeExtension(FixPath(settings.InputFile), "html") : FixPath(settings.OutputFile));

                using (StreamReader sr = new(FixPath(settings.InputFile)))
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
                            if (settings.ShowOutput)
                            {
                                OpenBrowser($"file:///{outputFileName.Replace(" ", "%20")}");
                            }
                            break;
                        case "FLOW":
                            block = parser.ParseText(sr);
                            FlowchartWorkspace fcws = BlockToFlowchartConverter.Convert(block);
                            mermaidText = FlowchartPublisher.Publish(fcws, "MERMAID");
                            MermaidPageGenerator.Generate(mermaidText, title, outputFileName);
                            if (settings.ShowOutput)
                            {
                                OpenBrowser($"file:///{outputFileName.Replace(" ", "%20")}");
                            }
                            break;
                    case "C4":
                        block = parser.ParseText(sr);
                        C4Workspace c4ws = BlockToC4Converter.Convert(block);
                        C4Publisher pub = new();
                        mermaidText = pub.Publish(c4ws, "Context", "MERMAID");
                        string contextFName = Path.ChangeExtension(Path.Combine(Path.GetDirectoryName(outputFileName), Path.GetFileNameWithoutExtension(outputFileName)+"-Context"), "html");
                        MermaidPageGenerator.Generate(mermaidText, title+ " Context Diagram", contextFName);
                        if (settings.ShowOutput)
                        {
                            OpenBrowser($"file:///{contextFName.Replace(" ", "%20")}");
                        }

                        mermaidText = pub.Publish(c4ws, "Container", "MERMAID");
                        string containerFName = Path.ChangeExtension(Path.Combine(Path.GetDirectoryName(outputFileName), Path.GetFileNameWithoutExtension(outputFileName) + "-Container"), "html");
                        MermaidPageGenerator.Generate(mermaidText, title + " Container Diagram", containerFName);
                        if (settings.ShowOutput)
                        {
                            OpenBrowser($"file:///{containerFName.Replace(" ", "%20")}");
                        }

                        mermaidText = pub.Publish(c4ws, "Component", "MERMAID");
                        string componentFName = Path.ChangeExtension(Path.Combine(Path.GetDirectoryName(outputFileName), Path.GetFileNameWithoutExtension(outputFileName) + "-Component"), "html");
                        MermaidPageGenerator.Generate(mermaidText, title + " Component Diagram", componentFName);
                        if (settings.ShowOutput)
                        {
                            OpenBrowser($"file:///{componentFName.Replace(" ", "%20")}");
                        }
                        break;
                    case "MD":
                        string mdResult = MdPublisher.Publish(sr.ReadToEnd());
                        File.WriteAllText(outputFileName, mdResult);
                        if (settings.ShowOutput)
                        {
                            OpenBrowser($"file:///{outputFileName.Replace(" ", "%20")}");
                        }
                        break;
                }
            }
            return 0;
        }
        private static string FixPath(string originalPath)
        {
            string uDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            //string sampleName = "~/github/newworld/sample";
            //if(OperatingSystem.IsWindows())
            string expandedDir = originalPath.Substring(0, 1) == "~" ? originalPath.Replace("~", uDir) : originalPath;
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
