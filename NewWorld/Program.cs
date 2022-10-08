using Spectre.Console.Cli;
using NewWorld.Commands;

namespace NewWorld
{

    internal class Program
    {
        static int Main(string[] args)
        {
            // Multiple commands using Spectre
            var app = new CommandApp();
            app.Configure(config =>
            {
                config.SetApplicationName("NewWorld");
                config.ValidateExamples();
                //config.AddExample(new[] { "generate", "--no-build" });

                // Generate

                config.AddCommand<GenerateCommand>("generate");

                // Sample
                config.AddCommand<SampleCommand>("sample");

                // Run
                //    config.AddCommand<RunCommand>("run");

                //    // Add
                //    config.AddBranch<AddSettings>("add", add =>
                //    {
                //        add.SetDescription("Add a package or reference to a .NET project");
                //        add.AddCommand<AddPackageCommand>("package");
                //        add.AddCommand<AddReferenceCommand>("reference");
                //    });

                //    // Serve
                //    config.AddCommand<ServeCommand>("serve")
                //    .WithExample(new[] { "serve", "-o", "firefox" })
                //    .WithExample(new[] { "serve", "--port", "80", "-o", "firefox" });
            });

            return app.Run(FixArgs(args));
        }

        private static string[] FixArgs(string[] asEntered)
        {
            List<string> newArgs = new List<string>(asEntered);

            if ((newArgs[0].ToUpperInvariant() == "--HELP") ||
                (newArgs[0].ToUpperInvariant() == "--H"))
            {
                newArgs.RemoveAt(0);
                newArgs.Add("--help");
            }

            return newArgs.ToArray();
        }
    }
}