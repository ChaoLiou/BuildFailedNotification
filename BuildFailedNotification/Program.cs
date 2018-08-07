using VSTeamServerManagement.Build;
using VSTeamServerManagement.Definition;
using VSTeamServerManagement.WorkItem;
using VSTeamServerManagement.Repository;
using VSTeamServerManagement;
using System.Linq;
using System.Configuration;
using System;

namespace BuildFailedNotification
{
    internal class Program
    {
        private static string _taskTitleFormat = ConfigurationManager.AppSettings["pbiTitleFormat"];

        private static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var command = args[0].ToLower();
                switch (command)
                {
                    case "run":
                        if (args.Length == 2)
                        {
                            var type = args[1].ToLower();
                            run(type);
                        }
                        else if (args.Length == 4)
                        {
                            var argument = args[2].ToLower();
                            if (argument == "-buildid")
                            {
                                var type = args[1].ToLower();
                                var buildId = 0;
                                if (int.TryParse(args[3], out buildId))
                                {
                                    runByBuildId(type, buildId);
                                }
                                else
                                {
                                    usage();
                                }
                            }
                        }
                        else
                        {
                            usage();
                        }
                        break;

                    default:
                        usage();
                        break;
                }
            }
            else
            {
                usage();
            }
        }

        private static void usage()
        {
            Console.WriteLine($"{nameof(BuildFailedNotification)} run {{project}} [-buildid {{buildid}}]");
        }

        private static void run(string type)
        {
            var project = type.ToProject();
            // list all definitions
            var definitions = DefinitionResult.List(project);
            Console.WriteLine($"{definitions.Count} definitions");
            // list all latest failed builds each definition
            foreach (var build in BuildResult.List(project, definitions.Select(d => d.Id).ToList(), true)
                .Where(b => b.Result == "failed"))
            {
                runByBuild(project, build);
            }
        }

        private static void runByBuildId(string type, int buildId)
        {
            var project = type.ToProject();
            var build = BuildResult.Get(project, buildId);
            runByBuild(project, build);
        }

        private static void runByBuild(ProjectType project, Build build)
        {
            Console.WriteLine($"Build Id:{build.Id}");

            try
            {
                var buildFailedPBIId = 0;
                var wiqlResult_PBI = WorkItemResult.Query(ProjectType.b, $@"
SELECT [System.Id], [System.Title], [System.State]
FROM WorkItems
WHERE [System.WorkItemType] = 'Product Backlog Item' AND [System.Title] = '{Config.PBITitle}' AND [System.IterationPath] = '{WorkItemResult.GetCurrentIteration(ProjectType.b)}'");

                // create a PBI, if it is not exist
                if (wiqlResult_PBI.WorkItems.Any())
                {
                    buildFailedPBIId = wiqlResult_PBI.WorkItems.Select(wi => wi.Id).Single();
                }
                else
                {
                    buildFailedPBIId = WorkItemResult.Create(ProjectType.b, WorkItemType.PBI, Config.PBITitle, Area.CICD);
                    Console.WriteLine($"pbi created:[id:{buildFailedPBIId}][title:{Config.PBITitle}]");
                }

                // create a task, if there is no tag in the build
                if (!BuildResult.GetTags(project, build.Id).Contains(Config.TagName))
                {
                    var taskTitle = _taskTitleFormat
                        .Replace("$Date", DateTime.Now.ToString(@"yyyy/MM/dd"))
                        .Replace("$Repository", RepositoryResult.Get(project, build.Repository.Id).Name);
                    var description = (@"<a href=""$url"" aria-label=""CTRL+Click or CTRL+Enter to follow link $url"">$url</a>")
                        .Replace("$url", build.Links.Web.Href);
                    var taskId = WorkItemResult.Create(ProjectType.b, WorkItemType.Task, taskTitle, Area.CICD, build.RequestedFor.UniqueName, description, buildFailedPBIId);
                    BuildResult.AddTag(project, build.Id, Config.TagName);
                    Console.WriteLine($"task created:[id:{taskId}][title:{taskTitle}]");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}