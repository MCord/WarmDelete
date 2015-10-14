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

        [Option("no-kill", DefaultValue = false, HelpText = "Do not kill the target process.")]
        public bool DenyKill { get; set; }

        [Option("no-message", DefaultValue = false, HelpText = "Do not send a close message (usefull when the process has no visible windows).")]
        public bool DenyMessage { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
                current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}