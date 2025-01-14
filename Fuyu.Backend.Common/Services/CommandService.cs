using System;
using Fuyu.Backend.Common.Delegates;
using Fuyu.Common.IO;

namespace Fuyu.Backend.Common;

public class CommandService
{
    public static CommandService Instance => instance.Value;
    private static readonly Lazy<CommandService> instance = new(() => new CommandService());

    public bool IsRunning { get; private set; }

    public CommandCallback OnCommand;
    public CommandCallback OnHelp;
    public CommandCallback OnSessions;

    /// <summary>
    /// The construction of this class is handled in the <see cref="instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    private CommandService()
    {
        IsRunning = true;

        OnCommand += ExitCommand;
        OnHelp += ExitHelp;
    }

    public void RunCommand(string[] args)
    {
        if (args == null || args.Length == 0)
        {
            // no commands to run
            return;
        }

        if (args.Length == 1)
        {
            switch (args[0])
            {
                case "help":
                    {
                        OnHelp(args);
                        return;
                    }
                case "sessions":
                    {
                        OnSessions(args);
                        return;
                    }
            }
        }

        OnCommand(args);
    }

    private void ExitCommand(string[] args)
    {
        if (args.Length != 1 && args[0] != "exit")
        {
            // not ours to run
            return;
        }

        IsRunning = false;
    }

    private void ExitHelp(string[] args)
    {
        Terminal.WriteLine("exit: close the application");
    }
}