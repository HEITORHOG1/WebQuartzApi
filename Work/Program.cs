using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Quartz;
using Work;
using Work.Extensions;

internal class Program
{
    private static IConfiguration configuration;

    private static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((hostContext, services) =>
            {
                configuration = hostContext.Configuration;
                services.AddQuartzConfiguration(configuration);
            })
            .Build();

        // Inicia o host para começar a executar o serviço
        await host.StartAsync();

        var scheduler = await SchedulerService.InitializeScheduler(configuration);
        // Obtém o scheduler do container de serviços

        // Dispara o job específico 'two' do grupo 'fps'
        await TriggerJob("one", "fps");

        // Mantém o console aberto até que um comando de cancelamento seja recebido
        Console.WriteLine("Pressione [Enter] para sair...");
        Console.ReadLine();

        // Para o host antes de sair
        await host.StopAsync();
    }

    private static async Task TriggerJob(string jobName, string groupName)
    {
        var scheduler = await SchedulerService.InitializeScheduler(configuration);

        var jobKey = new JobKey(jobName, groupName);
        if (await scheduler.CheckExists(jobKey))
        {
            await scheduler.TriggerJob(jobKey);
            Console.WriteLine($"Job '{jobName}' in group '{groupName}' was triggered successfully.");
        }
        else
        {
            Console.WriteLine($"Job '{jobName}' in group '{groupName}' does not exist.");
        }
    }
}

public static class CompilationHelper
{
    public static bool HasCompilationErrors(string projectPath)
    {
        var syntaxTrees = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories)
            .Select(filePath => CSharpSyntaxTree.ParseText(File.ReadAllText(filePath)));

        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic)
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location));

        var compilation = CSharpCompilation.Create("temp", syntaxTrees, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using (var ms = new MemoryStream())
        {
            EmitResult result = compilation.Emit(ms);

            if (!result.Success)
            {
                IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error);

                foreach (Diagnostic diagnostic in failures)
                {
                    Console.WriteLine(diagnostic.GetMessage());
                }

                return true;
            }
        }

        return false;
    }
}