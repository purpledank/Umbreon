﻿using Discord;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Umbreon.Attributes;
using Umbreon.Core;
using Umbreon.Interfaces;

namespace Umbreon.Services
{
    [Service]
    public class TimerService
    {
        private readonly LogService _log;
        private readonly IServiceProvider _services;

        private Timer _timer;
        private ConcurrentQueue<IRemoveable> _queue = new ConcurrentQueue<IRemoveable>();
        
        public TimerService(LogService log, IServiceProvider services)
        {
            _log = log;
            _services = services;
        }

        public void InitialiseTimer()
        {
            _timer = new Timer(async _ =>
            {
                if (_queue.TryDequeue(out var removeable))
                {
                    var service = _services.GetService(removeable.Service.GetType());
                    if(service is IRemoveableService removeableService)
                        await removeableService.Remove(removeable);
                }

                _log.NewLogEvent(LogSeverity.Verbose, LogSource.Timer, "Memory cleaned");
            }, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        public void Enqueue(IRemoveable removeable)
        {
            _queue.Enqueue(removeable);
            _queue = new ConcurrentQueue<IRemoveable>(_queue.OrderBy(x => x.When));
            if (_queue.TryPeek(out var obj))
            {
                _timer.Change(obj.When, TimeSpan.Zero);
            }
        }
    }
}