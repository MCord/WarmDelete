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

            SetRights(options);

            var wr = new WarmRemover
            {
                VerboseMode = options.Verbose
            };

            return wr.Remove(options.Target);
        }

        private static void SetRights(Options options)
        {
            Unlocker.Allow = Unlocker.Result.All;

            if (options.DenyKill)
            {
                Unlocker.Allow = Unlocker.Allow ^ Unlocker.Result.Kill;
            }
            if (options.DenyMessage)
            {
                Unlocker.Allow = Unlocker.Allow ^ Unlocker.Result.Message;
            }
        }
    }
}