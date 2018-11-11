﻿using System.Collections.Generic;
using Espeon.Core.Entities;
using LiteDB;

namespace Espeon.Entities
{
    public class ModuleInfo : DatabaseEntity
    {
        [BsonId]
        public override ulong Id { get; set; }

        public string Name { get; set; }

        public IList<string> Aliases { get; set; } = new List<string>();
        public IList<CommandInfo> Commands { get; set; } = new List<CommandInfo>();

        public override long WhenToRemove { get; set; }
    }

    public class CommandInfo
    {
        public string Name { get; set; }
        public IList<string> Aliases { get; set; } = new List<string>();
    }
}