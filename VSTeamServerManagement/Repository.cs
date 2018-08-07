using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace VSTeamServerManagement.Repository
{
    public class RepositoryResult
    {
        public static Repository Get(ProjectType project, string repositoryId)
        {
            var url = Config.TeamServerUrlPrefix + $"{project.ToRealName()}/_apis/git";
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Credentials = new NetworkCredential(Config.UserName, Config.Password);
                wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                return JsonConvert.DeserializeObject<Repository>(wc.DownloadString(url + $"/repositories/{repositoryId}"));
            }
        }
    }

    public class Repository
    {
        public string DefaultBranch { get; set; }
        public string Id { get; set; }
        public Links Links { get; set; }
        public string Name { get; set; }
        public Project Project { get; set; }
        public string RemoteUrl { get; set; }
        public string Url { get; set; }
    }

    public class Project
    {
        public string Description { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public long Revision { get; set; }
        public string State { get; set; }
        public string Url { get; set; }
        public string Visibility { get; set; }
    }

    public class Links
    {
        public Commits Commits { get; set; }
        public Commits Items { get; set; }
        public Commits Project { get; set; }
        public Commits PullRequests { get; set; }
        public Commits Pushes { get; set; }
        public Commits Refs { get; set; }
        public Commits Self { get; set; }
        public Commits Web { get; set; }
    }

    public class Commits
    {
        public string Href { get; set; }
    }
}