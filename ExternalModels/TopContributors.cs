using System.Runtime.Serialization;

namespace APIASYNC.ExternalModels
{
    [DataContract]
    public class TopContributor
    {
        [DataMember]
        private string user { get; }

        [DataMember]
        private int total_commits { get; }

        public TopContributor(string user, int total_commits)
        {
            this.user = user;
            this.total_commits = total_commits;
        }

        public TopContributor()
        {
        }
    }
}