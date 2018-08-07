# BuildFailedNotification
- it is a console application for detecting every failed Build during CI on the TFS (on-premises), and it will create a PBI and a Task for the developer who is in charge of the repository.

## How to use
1. fill the configs (`BuildFailedNotification\App.config`)
   - userName, password: the administrator account for sure
   - teamServerUrlPrefix: it's your TFS link, like `http://teamserver:8080/tfs/DefaultCollection/`
2. change the your Project Team information (`VSTeamServerManagement\Config.cs`)
   - ProjectType Enum
   - ExtensionMethods: .ToRealName(), .ToProject()
     - you should check the project team name on TFS and copy & paste here, 
        or check it on link, like `http://teamserver:8080/tfs/DefaultCollection/{your project team name}/`
3. the arguments information of the console application: `run {project} [-buildid {buildid}]`
   - you can execute it just by the project team name, or execute it with specific build id
4. so, you can set up a scheduler to execute it daily or whenever,
     or you can create a Task to execute it next to the Build Task you think it is important, and set up the new Task to skip if previous Task succeed, and to execute if previous failed on the Build Definition
