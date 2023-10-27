using CodeGeneration;
using Microsoft.Build.Evaluation;
using RazorEngine.Templating;

try
{
    string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Schema/model.json");
    string json = File.ReadAllText(jsonFilePath);
    JsonReferenceResolver resolver = new JsonReferenceResolver();
    var requestModel = resolver.Deserialize<ParentModel>(json);

    foreach (var item in requestModel.Enums)
    {

        string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template/EnumTemplate.cshtml");
        string templateContent = File.ReadAllText(templatePath);
        //Compile and execute the template
        string result = GenerateCodeFromTemplate(templateContent, item);

        // Save the generated C# code
        string outputPath = GetOutputFilePath(item);
        File.WriteAllText(outputPath, result);

        // Add generated code folder and file to the project
        AddGeneratedCodeToProject(item);
    }
    foreach (var item in requestModel.Entities)
    {
        string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template/Template.cshtml");
        string templateContent = File.ReadAllText(templatePath);
        //Compile and execute the template
        string result = GenerateCodeFromTemplate(templateContent, item);

        // Save the generated C# code
        string outputPath = GetOutputFilePath(item);
        File.WriteAllText(outputPath, result);

        // Add generated code folder and file to the project
        AddGeneratedCodeToProject(item);
    }

    Console.WriteLine("Code generation completed.");
}
catch (Exception exception)
{
    Console.WriteLine("Code generation was not completed.", exception.InnerException.ToString());
}
string GenerateCodeFromTemplate(string templateContent, GenerationModel requestModel)
{
    var templateService = RazorEngine.Engine.Razor;
    return templateService.RunCompile(templateContent, "templateKey", null, requestModel);
}

string GetOutputFilePath(GenerationModel requestModel)
{
    string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\net6.0", string.Empty), requestModel.Folder);
    Directory.CreateDirectory(folderPath);
    return Path.Combine(folderPath, $"{requestModel.FileName}.cs");
}

void AddGeneratedCodeToProject(GenerationModel requestModel)
{
    var projectFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\net6.0", string.Empty), "CodeGeneration.csproj");
    var project = LoadOrCreateProject(projectFilePath);

    string folderPath = requestModel.Folder;
    string fileLocation = $"{requestModel.Folder}\\" + requestModel.FileName + ".cs";

    // Check if folder is not already in the project
    if (project.Items.FirstOrDefault(i => i.EvaluatedInclude == folderPath) == null)
        project.AddItem("Folder", folderPath);

    // Check if the file is not already in the project
    if (project.Items.FirstOrDefault(i => i.EvaluatedInclude == fileLocation) == null)
        project.AddItem("Compile", fileLocation);

    project.Save();
}

Project LoadOrCreateProject(string projectFilePath)
{
    var project = Microsoft.Build.Evaluation
             .ProjectCollection.GlobalProjectCollection
             .LoadedProjects.FirstOrDefault(pr => pr.FullPath == projectFilePath);

    if (project == null)
        project = new Project(projectFilePath);

    project.ReevaluateIfNecessary();
    return project;
}
