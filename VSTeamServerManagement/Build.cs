using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace VSTeamServerManagement.Build
{
    public class BuildResult
    {
        public static List<Build> List(ProjectType project, List<int> definitionIds, bool latest = false)
        {
            var url = Config.TeamServerUrlPrefix + $"{project.ToRealName()}/_apis/build";
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Credentials = new NetworkCredential(Config.UserName, Config.Password);
                return JsonConvert.DeserializeObject<BuildResult>(wc.DownloadString(url + "/builds?definitions=" + string.Join(",", definitionIds) + (latest ? "&maxBuildsPerDefinition=1" : ""))).Builds;
            }
        }

        public static Build Get(ProjectType project, int buildId)
        {
            var url = Config.TeamServerUrlPrefix + $"{project.ToRealName()}/_apis/build";
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Credentials = new NetworkCredential(Config.UserName, Config.Password);
                return JsonConvert.DeserializeObject<Build>(wc.DownloadString(url + $"/builds/{buildId}"));
            }
        }

        public static void AddTag(ProjectType project, int buildId, string tagName)
        {
            var url = Config.TeamServerUrlPrefix + $"{project.ToRealName()}/_apis/build";
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Credentials = new NetworkCredential(Config.UserName, Config.Password);
                wc.UploadString(url + $"/builds/{buildId}/tags/{tagName}?api-version=2.0", "PUT", "");
            }
        }

        public static List<string> GetTags(ProjectType project, int buildId)
        {
            var url = Config.TeamServerUrlPrefix + $"{project.ToRealName()}/_apis/build";
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Credentials = new NetworkCredential(Config.UserName, Config.Password);
                return JsonConvert.DeserializeObject<Response>(wc.DownloadString(url + $"/builds/{buildId}/tags")).Value;
            }
        }

        public int Count { get; set; }

        [JsonProperty("Value")]
        public List<Build> Builds { get; set; }
    }

    public class Build
    {
        public string BuildNumber { get; set; }
        public Definition Definition { get; set; }
        public string FinishTime { get; set; }
        public int Id { get; set; }
        public bool KeepForever { get; set; }
        public Member LastChangedBy { get; set; }
        public string LastChangedDate { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }

        public Logs Logs { get; set; }
        public Plan OrchestrationPlan { get; set; }
        public List<Plan> Plans { get; set; }
        public string Priority { get; set; }
        public Project Project { get; set; }
        public Queue Queue { get; set; }
        public string QueueTime { get; set; }
        public string Reason { get; set; }
        public Repository Repository { get; set; }
        public Member RequestedBy { get; set; }
        public Member RequestedFor { get; set; }
        public string Result { get; set; }
        public bool RetainedByRelease { get; set; }
        public string SourceBranch { get; set; }
        public string SourceVersion { get; set; }
        public string StartTime { get; set; }
        public string Status { get; set; }
        public string Uri { get; set; }
        public string Url { get; set; }
    }

    public class Repository
    {
        public bool CheckoutSubmodules { get; set; }
        public object Clean { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
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

    public class Plan
    {
        public string PlanId { get; set; }
    }

    public class Logs
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
    }

    public class Links
    {
        public Link Self { get; set; }
        public Link Timeline { get; set; }
        public Link Web { get; set; }
    }

    public class Link
    {
        public string Href { get; set; }
    }

    public class Member
    {
        public string DisplayName { get; set; }
        public string Id { get; set; }
        public string ImageUrl { get; set; }
        public string UniqueName { get; set; }
        public string Url { get; set; }
    }

    public class Definition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public Project Project { get; set; }
        public int Revision { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
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
}