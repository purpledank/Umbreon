﻿using System;
using System.Threading.Tasks;

namespace Espeon.Services
{
    public interface ILogService
    {
        void Log(Source source, Severity severity, string message, Exception ex = null);
        Task BotLogAsync(string message);
    }
}
