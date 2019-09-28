﻿using Qmmands;
using System;
using System.Threading.Tasks;

namespace Espeon.Commands
{
    public abstract class EspeonCheckBase : CheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(CommandContext context, IServiceProvider provider)
            => CheckAsync((EspeonContext) context, provider);

        public abstract ValueTask<CheckResult> CheckAsync(EspeonContext context, IServiceProvider provider);
    }
}
