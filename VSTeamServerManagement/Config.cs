using System;
using System.Collections.Generic;
using System.Configuration;

namespace VSTeamServerManagement
{
    public class Config
    {
        public static string UserName = ConfigurationManager.AppSettings["userName"];
        public static string Password = ConfigurationManager.AppSettings["password"];
        public static string TeamServerUrlPrefix = ConfigurationManager.AppSettings["teamServerUrlPrefix"];
        public static string PBITitle = "Build Failed";
        public static string TagName = "task-created";
    }

    public class Response
    {
        public int Count { get; set; }
        public List<string> Value { get; set; }
    }

    public enum WorkItemType
    {
        PBI,
        Task
    }

    public enum ProjectType
    {
        a,
        b
    }

    public enum Area
    {
        CICD
    }

    public static class Extension
    {
        public static string ToRealName(this ProjectType project)
        {
            switch (project)
            {
                case ProjectType.a:
                    return "a";

                case ProjectType.b:
                    return "b";

                default:
                    return project.ToString();
            }
        }

        public static ProjectType ToProject(this string type)
        {
            switch (type.ToLower())
            {
                case "a":
                    return ProjectType.a;

                case "b":
                    return ProjectType.b;

                default:
                    throw new Exception("type is not matching");
            }
        }

        public static string ToRealName(this WorkItemType workItem)
        {
            switch (workItem)
            {
                case WorkItemType.PBI:
                    return "Product%20Backlog%20Item";

                case WorkItemType.Task:
                    return "Task";

                default:
                    return workItem.ToString();
            }
        }
    }
}