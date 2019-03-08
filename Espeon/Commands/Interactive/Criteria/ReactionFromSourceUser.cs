﻿using System.Threading.Tasks;
using Discord.WebSocket;

namespace Espeon.Commands
{
    public class ReactionFromSourceUser : ICriterion<SocketReaction>
    {
        private readonly ulong _userId;

        public ReactionFromSourceUser(ulong userId)
        {
            _userId = userId;
        }

        public Task<bool> JudgeAsync(EspeonContext context, SocketReaction reaction)
            => Task.FromResult(reaction.UserId == _userId);
    }
}