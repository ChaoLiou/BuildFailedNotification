using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace VSTeamServerManagement.Definition
{
    public class DefinitionResult
    {
        public static List<Definition> List(ProjectType project)
        {
            var url = Config.TeamServerUrlPrefix + $"{project.ToRealName()}/_apis/build";
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Credentials = new NetworkCredential(Config.UserName, Config.Password);
                return JsonConvert.DeserializeObject<DefinitionResult>(wc.DownloadString(url + "/definitions")).Definitions;
            }
        }

        public int Count { get; set; }

        [JsonProperty("Value")]
        public List<Definition> Definitions { get; set; }
    }

    public class Definition
    {
        public AuthoredBy AuthoredBy { get; set; }
        public string CreatedDate { get; set; }
        public int Id { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }

        public string Name { get; set; }
        public string Path { get; set; }
        public Project Project { get; set; }
        public string Quality { get; set; }
        public Queue Queue { get; set; }
        public int Revision { get; set; }
        public string Type { get; set; }
        public string Uri { get; set; }
        public string Url { get; set; }
    }

    public class Queue
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Pool Pool { get; set; }
    }

    public class Pool
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Project
    {
        public string Description { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public int Revision { get; set; }
        public string State { get; set; }
        public string Url { get; set; }
        public string Visibility { get; set; }
    }

    public class Links
    {
        public Link Editor { get; set; }
        public Link Self { get; set; }
        public Link Web { get; set; }
    }

    public class Link
    {
        public string Href { get; set; }
    }

    public class AuthoredBy
    {
        public string DisplayName { get; set; }
        public string Id { get; set; }
        public string ImageUrl { get; set; }
        public string UniqueName { get; set; }
        public string Url { get; set; }
    }
}