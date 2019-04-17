﻿using Espeon.Databases;
using Espeon.Services;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Espeon.Commands
{
    public class CustomCommandTypeParser : TypeParser<CustomCommand>
    {
        public override async ValueTask<TypeParserResult<CustomCommand>> ParseAsync(Parameter param, string value, CommandContext originalContext,
            IServiceProvider provider)
        {
            var context = (EspeonContext)originalContext;

            var service = provider.GetService<CustomCommandsService>();
            var commands = await service.GetCommandsAsync(context);

            var found = commands.FirstOrDefault(x =>
                string.Equals(x.Name, value, StringComparison.InvariantCultureIgnoreCase));

            return found is null
                ? TypeParserResult<CustomCommand>.Unsuccessful("Failed to find command")
                : TypeParserResult<CustomCommand>.Successful(found);
        }
    }
}
