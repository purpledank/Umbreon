﻿using Discord.WebSocket;
using Qmmands;
using System;
using System.Threading.Tasks;
using Discord;

namespace Espeon.Commands
{
    /*
     * Star
     * View
     * Stats
     * Random
     */

    [Name("Starboard")]
    [Group("Star")]
    public class Starboard : EspeonBase
    {
        public Random Random { get; set; }

        [Command("enable")]
        [Name("Enable Starboard")]
        [RequireElevation(ElevationLevel.Admin)]
        public async Task EnableStarboardAsync([Remainder] SocketTextChannel channel)
        {
            var guild = Context.CurrentGuild;
            guild.StarboardChannelId = channel.Id;
            Context.GuildStore.Update(guild);

            await Task.WhenAll(Context.GuildStore.SaveChangesAsync(), SendOkAsync(0));
        }

        [Command("disable")]
        [Name("Disable Starboard")]
        [RequireElevation(ElevationLevel.Admin)]
        public async Task DisableStarboardAsync()
        {
            var guild = Context.CurrentGuild;
            guild.StarboardChannelId = 0;
            Context.GuildStore.Update(guild);

            await Task.WhenAll(Context.GuildStore.SaveChangesAsync(), SendOkAsync(0));
        }

        [Command("limit")]
        [Name("Set Starboard Limit")]
        [RequireElevation(ElevationLevel.Admin)]
        public async Task SetStarboardLimitAsync([RequireRange(0)] int limit)
        {
            var guild = Context.CurrentGuild;
            guild.StarLimit = limit;
            Context.GuildStore.Update(guild);

            await Task.WhenAll(Context.GuildStore.SaveChangesAsync(), SendOkAsync(0));
        }

        [Command("random")]
        [Name("Random Star")]
        public async Task ViewRandomStarAsync()
        {
            var guild = await Context.GuildStore.GetOrCreateGuildAsync(Context.Guild, x => x.StarredMessages);

            if(guild.StarredMessages.Count == 0)
            {
                await SendNotOkAsync(0);
                return;
            }

            var randomStar = guild.StarredMessages[Random.Next(guild.StarredMessages.Count)];

            var user = await Context.Guild.GetGuildUserAsync(randomStar.AuthorId)
                ?? await Context.Client.GetUserAsync(randomStar.AuthorId);

            var jump = Utilities.BuildJumpUrl(Context.Guild.Id, randomStar.ChannelId, randomStar.Id);

            var starMessage = Utilities.BuildStarMessage(user, randomStar.Content, jump, randomStar.ImageUrl);

            var m = string.Concat(
                $"{Utilities.Star}" ,
                $"**{randomStar.ReactionUsers.Count}** - ",
                $"{(user as IGuildUser)?.GetDisplayName() ?? user.Username} in <#",
                $"{randomStar.ChannelId}>");

            await SendMessageAsync(m, starMessage);
        }
    }
}
