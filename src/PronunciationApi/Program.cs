using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PronunciationApi;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddSingleton(_ => SpeechOptions.FromEnv());
        services.AddHttpClient<SpeechClient>();
    })
    .Build();

host.Run();
