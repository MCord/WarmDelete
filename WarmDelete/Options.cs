using CommandLine;
using CommandLine.Text;
using JetBrains.Annotations;

namespace WarmDelete
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    internal class Options
    {
        [Option('t', "target", Required = true, HelpText = "Target file or folder.")]
        public string Target { get; set; }

        [Option('v', "verbose", DefaultValue = false, HelpText = "Prints detailed messages to standard output.")]
        public bool Verbose { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
                current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}