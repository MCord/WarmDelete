using CommandLine;

namespace WarmDelete
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            var options = new Options();
            if (!Parser.Default.ParseArguments(args, options))
            {
                return 1;
            }

            var wr = new WarmRemover
            {
                VerboseMode = options.Verbose
            };

            return wr.Remove(options.Target);
        }
    }
}