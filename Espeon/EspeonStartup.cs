﻿using Discord;
using Discord.WebSocket;
using Espeon.Attributes;
using Espeon.Database;
using Espeon.Services;
using Microsoft.Extensions.DependencyInjection;
using Pusharp;
using Pusharp.Entities;
using Qmmands;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Espeon
{
    public class EspeonStartup
    {
        private readonly IServiceProvider _services;

        [Inject] private readonly DiscordSocketClient _client;
        [Inject] private readonly CommandService _commands;
        [Inject] private readonly PushBulletClient _push;

        private readonly Config _config;

        private Device _phone;
        private Device Phone => _phone ?? (_phone = _push.Devices.First());

        private readonly TaskCompletionSource<Task> _completionSource;

        public EspeonStartup(IServiceProvider services, Config config)
        {
            _services = services;
            _config = config;
            _completionSource = new TaskCompletionSource<Task>();
        }

        public async Task StartBotAsync(DatabaseContext context)
        {
            EventHooks(context);
            
            var modules = _commands.AddModules(Assembly.GetEntryAssembly());

            var response = _services.GetService<ResponseService>();
            await response.OnCommandsRegisteredAsync(modules);

            await _push.ConnectAsync();

            await _client.LoginAsync(TokenType.Bot, _config.DiscordToken);
            await _client.StartAsync();

            //'Ready event' that only fires at startup
            await _completionSource.Task;

            await _services.GetService<ReminderService>().LoadRemindersAsync(context);
        }

        //TODO clean this up
        private void EventHooks(DatabaseContext context)
        {
            _client.Ready += () =>
            {
                _completionSource.SetResult(Task.CompletedTask);
                return Task.CompletedTask;
            };
            
            var logger = _services.GetService<LogService>();
            _client.Log += log =>
            {
                var (source, severity, lMessage, exception) = LogFactory.FromDiscord(log);
                return logger.LogAsync(source, severity, lMessage, exception);
            };

            _push.Log += log =>
            {
                var (source, severity, lMessage) = LogFactory.FromPusharp(log);
                return logger.LogAsync(source, severity, lMessage);
            };

#if !DEBUG //Don't want to waste my pushes
            _client.Connected += async () => 
            {
                await Phone.SendNoteAsync(x =>
                {
                    x.Title = "Espeon Connected";
                    x.Body = "<3";
                });

            };

            _client.Disconnected += async _ =>
            {
                await Phone.SendNoteAsync(x =>
                {
                    x.Title = "Espeon Disconnected";
                    x.Body = "</3";
                });
            };
#endif
        }
    }
}
