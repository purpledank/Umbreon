﻿using Discord;
using Discord.WebSocket;
using Espeon.Databases.CommandStore;
using Espeon.Databases.GuildStore;
using Espeon.Databases.UserStore;
using Espeon.Services;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Espeon.Commands
{
    public class InteractiveService : BaseService
    {
        [Inject] private readonly DiscordSocketClient _client;
        [Inject] private readonly TimerService _timer;

        private readonly ConcurrentDictionary<ulong, CallbackData> _reactionCallbacks;

        private static TimeSpan DefaultTimeout => TimeSpan.FromMinutes(2);

        public InteractiveService()
        {
            _reactionCallbacks = new ConcurrentDictionary<ulong, CallbackData>();
        }

        public override Task InitialiseAsync(UserStore userStore, GuildStore guildStore, CommandStore commandStore, IServiceProvider services)
        {
            _client.ReactionAdded += HandleReactionAsync;
            return Task.CompletedTask;
        }

        public async Task<SocketUserMessage> NextMessageAsync(EspeonContext context,
            ICriterion<SocketUserMessage> criterion, TimeSpan? timeout = null)
        {
            timeout ??= DefaultTimeout;

            var taskCompletionSource = new TaskCompletionSource<SocketUserMessage>();

            async Task MessageReceivedAsync(SocketUserMessage message)
            {
                var result = await criterion.JudgeAsync(context, message);

                if(result)
                    taskCompletionSource.SetResult(message);
            }

            Task HandleMessageAsync(SocketMessage msg) => msg is SocketUserMessage message
                ? MessageReceivedAsync(message)
                : Task.CompletedTask;

            context.Client.MessageReceived += HandleMessageAsync;                

            var resultTask = taskCompletionSource.Task;
            var delay = Task.Delay(timeout.Value);

            var taskResult = await Task.WhenAny(resultTask, delay);

            context.Client.MessageReceived -= HandleMessageAsync;

            return taskResult == resultTask ? await resultTask : null;
        }

        public async Task SendPaginatedMessageAsync(PaginatorBase paginator, TimeSpan? timeout = null)
        {
            timeout ??= DefaultTimeout;

            await paginator.InitialiseAsync();

            await InternalAddCallbackAsync(paginator, timeout.Value);
        }

        public async Task<bool> TryAddCallbackAsync(IReactionCallback callback, TimeSpan? timeout = null)
        {
            if (callback is PaginatorBase)
                return false;

            var message = callback.Message;

            //null check for games support... Yeah I should rethink this
            if (!(message is null) && _reactionCallbacks.ContainsKey(message.Id))
                return false;

            timeout ??= DefaultTimeout;

            await callback.InitialiseAsync();

            return await InternalAddCallbackAsync(callback, timeout.Value);
        }

        private async Task<bool> InternalAddCallbackAsync(IReactionCallback callback, TimeSpan timeout)
        {
            var message = callback.Message;

            foreach (var emote in callback.Reactions)
                await message.AddReactionAsync(emote);

            var callbackData = new CallbackData(callback, timeout)
            {
                //WhenToRemove = DateTimeOffset.UtcNow.Add(timeout).ToUnixTimeMilliseconds()
            };

            var key = await _timer.EnqueueAsync(callbackData, DateTimeOffset.UtcNow.Add(timeout).ToUnixTimeMilliseconds(), RemoveAsync);

            callbackData.TaskKey = key;

            return _reactionCallbacks.TryAdd(message.Id, callbackData);
        }

        public async Task<bool> TryRemoveCallbackAsync(IReactionCallback callback)
        {
            if (!_reactionCallbacks.TryGetValue(callback.Message.Id, out var callbackData))
                return false;
            
            await _timer.RemoveAsync(callbackData.TaskKey);

            return _reactionCallbacks.TryRemove(callback.Message.Id, out _);
        }
             
        private async Task HandleReactionAsync(Cacheable<IUserMessage, ulong> cachedMessage,
            ISocketMessageChannel channel, SocketReaction reaction)
        {
            var message = await cachedMessage.GetOrDownloadAsync();

            if (message is null)
                return;

            if (!_reactionCallbacks.TryGetValue(message.Id, out var callbackData))
                return;

            var callback = callbackData.Callback;
            var criterion = callback.Criterion;
            var result = await criterion.JudgeAsync(callback.Context, reaction);

            if (!result)
                return;

            result = await callback.HandleCallbackAsync(reaction);

            if (!result)
            {
                await _timer.RemoveAsync(callbackData.TaskKey);
                
                var newKey = await _timer.EnqueueAsync(callbackData, DateTimeOffset.UtcNow.Add(callbackData.Timeout).ToUnixTimeMilliseconds(), RemoveAsync);
                callbackData.TaskKey = newKey;
            }
            else
            {
                await _timer.RemoveAsync(callbackData.TaskKey);
                await RemoveAsync(callbackData.TaskKey, callbackData);
            }
        }

        private async Task RemoveAsync(string key, object removable)
        {
            var callbackData = (CallbackData) removable;

            var callback = callbackData.Callback;
            await callback.HandleTimeoutAsync();

            _reactionCallbacks.TryRemove(callback.Message.Id, out _);
        }

        private class CallbackData
        {
            public IReactionCallback Callback { get; }
            public TimeSpan Timeout { get; }
            
            //public long WhenToRemove { get; set; }

            public string TaskKey { get; set; }

            public CallbackData(IReactionCallback callback, TimeSpan timeout)
            {
                Callback = callback;
                Timeout = timeout;
            }
        }
    }
}