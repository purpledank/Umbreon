﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbreon.Attributes;
using Umbreon.Commands.Preconditions;
using Umbreon.Core;
using Umbreon.Core.Entities.Guild;

namespace Umbreon.Services
{
    [Service]
    public class CustomCommandsService
    {
        private readonly DatabaseService _database;
        private readonly CommandService _commandService;
        private readonly MessageService _message;
        private readonly LogService _logs;
        private readonly ConcurrentDictionary<ulong, ModuleInfo> _modules = new ConcurrentDictionary<ulong, ModuleInfo>();

        public CustomCommandsService(DatabaseService database, CommandService commandsService, MessageService message, LogService logs)
        {
            _database = database;
            _commandService = commandsService;
            _message = message;
            _logs = logs;
        }

        public async Task LoadCmdsAsync(DiscordSocketClient client)
        {
            foreach (var guild in client.Guilds)
            {
                await NewCmdsAsync(guild);
            }

            _logs.NewLogEvent(LogSeverity.Info, LogSource.CustomCmds, "Custom commands have been loaded");
        }

        private async Task NewCmdsAsync(IGuild guild)
        {
            if (_modules.TryGetValue(guild.Id, out var found))
                await _commandService.RemoveModuleAsync(found);

            var cmds = _database.TempLoad(guild).CustomCommands;
            var created = await _commandService.CreateModuleAsync("", module =>
            {
                module.AddPrecondition(new RequireGuildAttribute(guild.Id));
                module.WithSummary("The custom commands for this server");
                module.WithName(guild.Name);

                foreach (var cmd in cmds)
                {
                    module.AddCommand(cmd.CommandName, CommandCallbackAsync, command =>
                    {
                        command.AddAttributes(new UsageAttribute($"{cmd.CommandName}"));
                        command.WithSummary("This is a custom command");
                        command.WithName(cmd.CommandName);
                    });
                }
            });

            if (!_modules.TryAdd(guild.Id, created))
                _modules[guild.Id] = created;
        }

        private async Task CommandCallbackAsync(ICommandContext context, object[] _, IServiceProvider __, CommandInfo info)
        {
            await _message.SendMessageAsync(context, GetCmds(context).FirstOrDefault(x =>
                string.Equals(x.CommandName, info.Name, StringComparison.CurrentCultureIgnoreCase))?.CommandValue);
        }

        public async Task CreateCmdAsync(ICommandContext context, string cmdName, string cmdValue)
        {
            var newCmd = new CustomCommand
            {
                CommandName = cmdName,
                CommandValue = cmdValue
            };
            var guild = _database.GetGuild(context);
            guild.CustomCommands.Add(newCmd);
            _database.UpdateGuild(guild);
            await NewCmdsAsync(context.Guild);
        }

        public void UpdateCommand(ICommandContext context, string cmdName, string newValue)
        {
            var guild = _database.GetGuild(context);
            guild.CustomCommands
                .Find(x => string.Equals(x.CommandName, cmdName, StringComparison.CurrentCultureIgnoreCase))
                .CommandValue = newValue;
            _database.UpdateGuild(guild);
        }

        public async Task RemoveCmdAsync(ICommandContext context, string cmdName)
        {
            var guild = _database.GetGuild(context);
            var targetCmd = guild.CustomCommands.Find(x =>
                string.Equals(x.CommandName, cmdName, StringComparison.CurrentCultureIgnoreCase));
            guild.CustomCommands.Remove(targetCmd);
            _database.UpdateGuild(guild);
            await NewCmdsAsync(context.Guild);
        }

        public bool IsReserved(string toCheck)
        {
            var cmds = _commandService.Commands.SelectMany(x => x.Aliases).ToList();
            var reserved = new List<string>();
            foreach (var cmd in cmds)
            {
                reserved.Add(RemoveGroupName(cmd));
            }

            return reserved.Contains(toCheck, StringComparer.CurrentCultureIgnoreCase);
        }

        private string RemoveGroupName(string inStr)
        {
            var groups = _commandService.Modules.Where(x => !(x.Group is null)).Select(y => y.Group);
            var outStr = inStr;
            foreach (var group in groups)
            {
                if (inStr.Contains(group))
                {
                    outStr = inStr.Replace($"{group} ", "");
                }
            }
            return outStr;
        }

        public static bool TryParse(IEnumerable<CustomCommand> cmds, string cmdName, out CustomCommand cmd)
        {
            cmd = cmds.FirstOrDefault(x => string.Equals(x.CommandName, cmdName, StringComparison.CurrentCultureIgnoreCase));
            return !(cmd is null);
        }

        public IEnumerable<CustomCommand> GetCmds(ICommandContext context)
            => GetCmds(context.Guild.Id);

        private IEnumerable<CustomCommand> GetCmds(ulong guildId)
            => _database.GetGuild(guildId).CustomCommands;
    }
}
