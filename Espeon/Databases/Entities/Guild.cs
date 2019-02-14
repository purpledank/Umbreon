﻿using System.Collections.Generic;

namespace Espeon.Databases.Entities
{
    public class Guild : DatabaseEntity
    {
        public override ulong Id { get; set; }

        public ulong WelcomeChannelId { get; set; }
        public string WelcomeMessage { get; set; }

        public ulong DefaultRoleId { get; set; }

        public List<string> Prefixes { get; set; }
        public ICollection<ulong> RestrictedChannels { get; set; }
        public ICollection<ulong> RestrictedUsers { get; set; }

        public ICollection<ulong> Admins { get; set; }
        public ICollection<ulong> Moderators { get; set; }

        public ICollection<ulong> SelfAssigningRoles { get; set; }
        public List<CustomCommand> Commands { get; set; }
    }

    public class CustomCommand
    {
        public Guild Guild { get; set; }
        public ulong GuildId { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}