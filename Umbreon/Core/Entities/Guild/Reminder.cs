﻿using System;
using Umbreon.Interfaces;

namespace Umbreon.Core.Entities.Guild
{
    public class Reminder : IRemoveable
    {
        public string TheReminder { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong UserId { get; set; }

        public int Identifier { get; set; }
        public DateTime When { get; set; }
        public IRemoveableService Service { get; set; }
    }
}
