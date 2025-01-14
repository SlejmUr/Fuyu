using System.Threading.Tasks;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.Common;
using Fuyu.Backend.Core;
using Fuyu.Backend.Core.Servers;
using Fuyu.Backend.EFTMain;
using Fuyu.Common.IO;
using Fuyu.Common.Networking;
using Fuyu.Common.Serialization;
using Fuyu.DependencyInjection;
using Fuyu.Modding;

namespace Fuyu.Backend;

public class Program
{
    static async Task Main()
    {
        var container = new DependencyContainer();

        Terminal.SetLogFile("Fuyu/Logs/Backend.log");
        LoadDatabase(container);
        LoadServers(container);
        await LoadMods(container);

        Terminal.WriteLine("Done!");
        Terminal.WriteLine("You can now run commands.");
        Terminal.WriteLine("Users can now connect.");

        CommandService.Instance.OnSessions += _ =>
        {
            Terminal.WriteLine(Json.Stringify(EftOrm.Instance.GetSessions().Keys));
        };

        while (CommandService.Instance.IsRunning)
        {
            var text = Terminal.ReadLine();
            var args = text.Split(' ');
            CommandService.Instance.RunCommand(args);
        }

        await ModManager.Instance.UnloadAll();
    }

    static void LoadDatabase(DependencyContainer container)
    {
        Terminal.WriteLine("Loading database...");

        CoreLoader.Instance.Load();
        EftLoader.Instance.Load();
        TraderDatabase.Instance.Load();
        ItemFactoryService.Instance.Load();
    }

    static void LoadServers(DependencyContainer container)
    {
        Terminal.WriteLine("Loading backends...");

        var coreServer = new CoreServer();
        container.RegisterSingleton<HttpServer, CoreServer>(coreServer);

        coreServer.RegisterServices();
        coreServer.Start();

        var eftMainServer = new EftMainServer();
        container.RegisterSingleton<HttpServer, EftMainServer>(eftMainServer);

        eftMainServer.RegisterServices();
        eftMainServer.Start();
    }

    static Task LoadMods(DependencyContainer container)
    {
        Terminal.WriteLine("Loading mods...");

        ModManager.Instance.AddMods("./Fuyu/Mods/Backend");
        return ModManager.Instance.Load(container);
    }
}