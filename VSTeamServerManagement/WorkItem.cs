using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace VSTeamServerManagement.WorkItem
{
    public class WorkItemResult
    {
        private static string _currentIteration = "";

        public static int Create(ProjectType project, WorkItemType workItem, string title, Area area, string assignedTo = "", string description = "", int relationWorkItemId = 0)
        {
            var url = Config.TeamServerUrlPrefix + $"{project.ToRealName()}/_apis/wit";
            using (var wc = new WebClient())
            {
                var body = new List<object>
                {
                    new Operation()
                    {
                        path = "/fields/System.Title",
                        value = title
                    },
                    new Operation()
                    {
                        path = "/fields/System.AssignedTo",
                        value = assignedTo
                    },
                    new Operation()
                    {
                        path = "/fields/System.IterationPath",
                        value = GetCurrentIteration(project)
                    },
                    new Operation()
                    {
                        path = "/fields/System.Description",
                        value = description
                    },
                    new Operation()
                    {
                        path = "/fields/System.AreaPath",
                        value = "!Backlog\\" + area
                    }
                };

                if (workItem == WorkItemType.PBI)
                {
                    body.Add(new Operation()
                    {
                        path = "/fields/System.State",
                        value = "Committed"
                    });
                }

                if (relationWorkItemId > 0)
                {
                    body.Add(new Operation()
                    {
                        path = "/relations/-",
                        value = new Operation.Value()
                        {
                            rel = "System.LinkTypes.Hierarchy-Reverse",
                            url = url + $"/workitems/{relationWorkItemId}",
                        }
                    });
                }

                wc.Encoding = Encoding.UTF8;
                wc.Credentials = new NetworkCredential(Config.UserName, Config.Password);
                wc.Headers.Add(HttpRequestHeader.ContentType, "application/json-patch+json");
                return JsonConvert.DeserializeObject<WIQLResult.WorkItem>(wc.UploadString(url + $"/workitems/${workItem.ToRealName()}?api-version=2.0", JsonConvert.SerializeObject(body))).Id;
            }
        }

        public static string GetCurrentIteration(ProjectType project)
        {
            if (string.IsNullOrWhiteSpace(_currentIteration))
            {
                var url = Config.TeamServerUrlPrefix + $"{project.ToRealName()}/_apis/work";
                using (var wc = new WebClient())
                {
                    wc.Encoding = Encoding.UTF8;
                    wc.Credentials = new NetworkCredential(Config.UserName, Config.Password);
                    wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    var iteration = JsonConvert.DeserializeObject<IterationResponse>(wc.DownloadString(url + "/TeamSettings/Iterations?$timeframe=current&api-version=2.0")).Value.FirstOrDefault();
                    if (iteration != null)
                    {
                        _currentIteration = iteration.Path;
                    }
                }
            }

            return _currentIteration;
        }

        public static WIQLResult Query(ProjectType project, string queryString)
        {
            var url = Config.TeamServerUrlPrefix + $"{project.ToRealName()}/_apis/wit/wiql?api-version=2.0";
            using (var wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                wc.Credentials = new NetworkCredential(Config.UserName, Config.Password);
                return JsonConvert.DeserializeObject<WIQLResult>(wc.UploadString(url, "POST", JsonConvert.SerializeObject(new Query() { query = queryString })));
            }
        }
    }

    public class Operation
    {
        public string op { get; set; } = "add";
        public string path { get; set; }
        public object value { get; set; }

        public class Value
        {
            public string rel { get; set; }
            public string url { get; set; }
            public object attributes { get; set; }
        }
    }

    public class Query
    {
        public string query { get; set; }
    }

    public class WIQLResult
    {
        public string AsOf { get; set; }
        public List<Column> Columns { get; set; }
        public string QueryResultType { get; set; }
        public string QueryType { get; set; }
        public List<WorkItem> WorkItems { get; set; }

        public class WorkItem
        {
            public int Id { get; set; }
            public string Url { get; set; }
        }

        public class Column
        {
            public string Name { get; set; }
            public string ReferenceName { get; set; }
            public string Url { get; set; }
        }
    }

    public class IterationResponse
    {
        public int Count { get; set; }

        [JsonProperty("Value")]
        public List<Iteration> Value { get; set; }

        public class Iteration
        {
            public Attributes Attributes { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }
            public string Url { get; set; }
        }

        public class Attributes
        {
            public string FinishDate { get; set; }
            public string StartDate { get; set; }
        }
    }
}