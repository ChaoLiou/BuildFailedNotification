# BuildFailedNotification
- it is a console application for detecting every Build fails during CI on the TFS (on-premises), and it will create a PBI and a Task with build results for the developer who is in charge of the repository.
- so, if Alice pushes code to the version control system (like git or tfvc), and it begins to run thru the Build (I had enabled CI feature for this repository), but it failed during building. With the BuildFailedNotification Console Application, it will notice that a build has failed and create a PBI and a Task for Alice, then she will notice them on her Scrum Task Board, and check the build results in the Task Description.

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

## The Workflow in the Console Application
```
 ___________________________________     _________________________________________
|run with specific project team name|-->|gather all build definitions informations|
 '''''''''''''''''''''''''''''''''''     '''''''''''''''''|'''''''''''''''''''''''
 ___________________________________       _______________V__________________________________________
|run with specific project team name|     |gather latest builds informations in each build definition|
|and specific build id              |     |and only take which the Result is failed.                 |
 '''''''''''''''''''''''''''\'''''''      /''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                      _______V___________V_________________________
                     |check this build failed PBI is created before|
                      '''''''/''''''''''''''''\''''''''''''''''''''
                            / no               \ yes
            _______________V_________        ___V____________________________________ 
           |create a build failed PBI|----->|check this build has a Tag(task-created)|
            '''''''''''''''''''''''''        ''''''''''''''''|'''''''''''''''''''''''\
                                                             | no                     \ yes
                                                  ___________V_________________        V_______
                                                 |create a task inside this PBI|------>|The End|
                                                 |and add a tag to this Build  |        '''''''
                                                  '''''''''''''''''''''''''''''
```                   
