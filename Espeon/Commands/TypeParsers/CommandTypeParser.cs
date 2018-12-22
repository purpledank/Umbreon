﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

namespace Espeon.Commands.TypeParsers
{
    public class CommandTypeParser : TypeParser<Command>
    {
        public override Task<TypeParserResult<Command>> ParseAsync(string value, ICommandContext context, IServiceProvider provider)
        {
            var commands = provider.GetService<CommandService>();
            var command = commands.GetAllCommands().SingleOrDefault(x =>
                string.Equals(x.Name, value, StringComparison.InvariantCultureIgnoreCase));

            return Task.FromResult(command is null
                ? new TypeParserResult<Command>("Multiple or no matching commands")
                : new TypeParserResult<Command>(command));
        }
    }
}