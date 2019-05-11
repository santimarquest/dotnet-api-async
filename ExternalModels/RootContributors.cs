namespace APIASYNC.ExternalModels
{
    public class RootContributors
    {
        public int total_count { get; set; }

        public bool incomplete_results { get; set; }

        public Contributor[] items { get; set; }
    }
}