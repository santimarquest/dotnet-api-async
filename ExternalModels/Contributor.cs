using System.Runtime.Serialization;

namespace APIASYNC.ExternalModels
{
    [DataContract]
    public class Contributor
    {
        [DataMember]
        public string login { get; set; }
    }
}