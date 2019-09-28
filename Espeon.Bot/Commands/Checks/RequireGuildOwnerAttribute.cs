﻿using Espeon.Commands;
using Espeon.Services;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using System;
using System.Threading.Tasks;

namespace Espeon.Bot.Commands
{
    public class RequireGuildOwnerAttribute : RequireOwnerAttribute
    {
        public override async ValueTask<CheckResult> CheckAsync(EspeonContext context, IServiceProvider provider)
        {
            var result = await base.CheckAsync(context, provider);

            if(result.IsSuccessful)
                return CheckResult.Successful;

            if (context.User.Id == context.Guild.OwnerId)
                return CheckResult.Successful;

            var user = context.Invoker;
            var response = provider.GetService<IResponseService>();

            return CheckResult.Unsuccessful(response.GetResponse(this, user.ResponsePack, 0));
        }
    }
}
