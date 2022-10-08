using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;
using System.Diagnostics.CodeAnalysis;

namespace NewWorld.Commands
{
    internal class SampleCommand : Command<SampleCommand.Settings>
    {

        //static int ExecuteSample(SampleOptions opts)
        //{
        //    if (opts.List)
        //    {
        //        System.Console.WriteLine("flow");
        //        System.Console.WriteLine("Work");
        //        System.Console.WriteLine("c4");
        //    }
        //    return 0;
        //}


        public sealed class Settings : CommandSettings
        {

            [Description("The name of the output file.")]
            [CommandOption("-o|--outputfile")]
            public string? OutputFile { get; init; }    // init; vs set; ?

            [Description("Specify the type of file.")]
            [CommandOption("-f|--filetype")]
            public string? FileType { get; init; }    // init; vs set; ?

            public override ValidationResult Validate()
            {
                ValidationResult vr = ValidationResult.Success();
                //vr.Message
                //vr.Successful

                //ValidationResult vr = ValidationResult.Error("Message");

                if (string.IsNullOrEmpty(FileType))
                {
                    vr = ValidationResult.Error(@"Select a file type (flow, work, c4)");
                }
                else
                {
                    switch (FileType.ToUpper())
                    {
                        case "WORK":
                        case "FLOW":
                        case "C4":
                            break;
                        default:
                            vr = ValidationResult.Error(@"Invalid file type.  Valid choices are flow, work, c4");
                            break;
                    }
                }
                    return vr;
            }
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            string sampleString = string.Empty;
            switch (settings.FileType.ToUpper())
            {
                case "WORK":
                    sampleString = TestData.GetWorkItems();
                    break;
                case "FLOW":
                    //sampleString = TestData.GetWhamFlow();
                    sampleString = TestData.GetNoDecisionWhamFlow();
                    break;
                case "C4":
                    sampleString = TestData.GetC4Sample();
                    break;
            }

            if (string.IsNullOrEmpty(settings.OutputFile))
            {
                Console.WriteLine(sampleString);
            }
            else
            {
                File.WriteAllText(FixPath(settings.OutputFile), sampleString);
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
    }
}