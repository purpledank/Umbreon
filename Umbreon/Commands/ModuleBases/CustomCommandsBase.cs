﻿using System.Collections.Generic;
using Discord.Commands;
using Umbreon.Core.Entities.Guild;
using Umbreon.Services;

namespace Umbreon.Commands.ModuleBases
{
    public class CustomCommandsBase<T> : UmbreonBase<T> where T : class, ICommandContext
    {
        public CustomCommandsService Commands { get; set; }
        public IEnumerable<CustomCommand> CurrentCmds;
        public string[] ReservedWords = { "Create", "Modify", "Delete", "Cancel", "List", "c" };

        protected override void BeforeExecute(CommandInfo command)
        {
            CurrentCmds = Commands.GetCmds(Context);
        }
    }
}