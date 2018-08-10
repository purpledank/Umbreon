﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Umbreon.Attributes;
using Umbreon.Extensions;
using Umbreon.Modules.Contexts;
using Umbreon.Modules.ModuleBases;
using Umbreon.Preconditions;
using Umbreon.TypeReaders;

namespace Umbreon.Modules
{
    [Summary("Music commands")]
    [RequireMusic]
    public class Music : MusicModuleBase<UmbreonContext>
    {
        //TODO add show queue

        [Command("join")]
        [Summary("Gets the bot to join your voice channel")]
        [Usage("music join")]
        [Name("Join Channel")]
        public async Task JoinChannel()
        {
            if (Context.User.VoiceChannel is null)
            {
                await SendMessageAsync("You need to be in a voice channel to invoke this command");
                return;
            }

            await Music.JoinAsync(Context);
        }

        [Command("play")]
        [Summary("Plays the passed song")]
        [Usage("music play https://www.youtube.com/watch?v=y6120QOlsfU")]
        [Name("Play Song")]
        [RequireSameChannel]
        public async Task PlayMusic(
            [Name("Song")]
            [Summary("The song you want to play")]
            [Remainder] string toSearch)
        {
            var track = await Music.GetTrackAsync(toSearch);
            if (track is null)
            {
                await SendMessageAsync("No track found");
                return;
            }

            var res = await Music.PlayTrackAsync(Context, track);
            var embed = new EmbedBuilder
            {
                Title = res ? $"{track.Title} added to queue" : $"Now playing {track.Title}",
                Color = Color.Red
            };
            await SendMessageAsync(string.Empty, embed: embed.Build());
        }

        [Command("leave")]
        [Summary("Leave the current voice channel")]
        [Usage("music leave")]
        [Name("Leave Channel")]
        [RequireSameChannel]
        public async Task LeaveChannel()
        {
            if (Context.Guild.CurrentUser.VoiceChannel is null)
            {
                await SendMessageAsync("Bot is not in a voice channel");
                return;
            }
            await Music.LeaveAsync(Context);
        }

        [Command("volume")]
        [Summary("Set the volume")]
        [Usage("music volume 69")]
        [Name("Set Volume")]
        [RequireSameChannel]
        public async Task SetVolume(
            [Name("Volume")]
            [Summary("The volume you want to set")]
            [OverrideTypeReader(typeof(VolumeTypeReader))] uint volume)
        {
            await Music.SetVolumeAsync(Context, volume);
            await SendMessageAsync($"Volume has been set to: {Math.Floor(volume / 1.5)}");
        }

        [Command("pause")]
        [Summary("Pauses the current song")]
        [Usage("music pause")]
        [Name("Pause Song")]
        [RequireSameChannel]
        public async Task Pause()
        {
            await Music.PauseAsync(Context);
        }

        [Command("resume")]
        [Summary("Resumes a paused song")]
        [Usage("music resume")]
        [Name("Resume Song")]
        [RequireSameChannel]
        public async Task Resume()
        {
            await Music.ResumeAsync(Context);
        }

        [Command("skip")]
        [Summary("Skips the current song")]
        [Usage("music skip")]
        [Name("Skip Song")]
        [RequireSameChannel]
        public async Task Skip()
        {
            await Music.SkipSongAsync(Context);
        }

        [Command("approve")]
        [Summary("Approve someone to use music commands")]
        [Usage("music approve Umbreon")]
        [Name("Approve User")]
        [RequireOwner]
        public async Task Approve(
            [Name("User")]
            [Summary("The user you want to approve")]
            [Remainder] SocketGuildUser user)
        {
            CurrentGuild.MusicUsers.Add(user.Id);
            await SendMessageAsync($"{user.GetDisplayName()} has been approved");
        }

        [Command("unapprove")]
        [Summary("Unapprove a user for music commands")]
        [Usage("music unapprove Umbreon")]
        [Name("Unapprove User")]
        [RequireOwner]
        public async Task Unapprove(
            [Name("User")]
            [Summary("The user you want to unapprove")]
            [Remainder] SocketGuildUser user)
        {
            CurrentGuild.MusicUsers.Remove(user.Id);
            await SendMessageAsync($"{user.GetDisplayName()} has been unapproved");
        }
    }
}