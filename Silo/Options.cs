using CommandLine;

namespace Silo
{
    public class Options
    {
        [Option('e', "environment", Required = false, HelpText = "Executes in a specific environment.")]
        public EnvironmentType Environment { get; set; }
    }

    public enum EnvironmentType
    {
        SqlServerCluster,
        LocalhostClustering
    }
}
