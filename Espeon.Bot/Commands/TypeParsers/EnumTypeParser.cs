﻿using Casino.Qmmands;
using Espeon.Commands;
using Espeon.Services;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using System;
using System.Threading.Tasks;

namespace Espeon.Bot.Commands
{
    public abstract class EnumTypeParser<T> : EspeonTypeParser<T> where T : Enum
    {
        private PrimitiveTypeParser<T> _parser;
        private string[] _names;

        public override ValueTask<TypeParserResult<T>> ParseAsync(Parameter parameter, string value, EspeonContext context, IServiceProvider provider)
        {
            var commands = provider.GetService<CommandService>();

            _parser ??= commands.GetPrimiteTypeParser<T>();

            var result = _parser.TryParse(parameter, value, out var res);

            if (result)
                return TypeParserResult<T>.Successful(res);

            _names ??= typeof(T).GetEnumNames();

            var toSend = string.Join(", ", _names);
            var response = provider.GetService<IResponseService>();
            var user = context.Invoker;

            return TypeParserResult<T>.Unsuccessful(
                response.GetResponse(this, user.ResponsePack, 0, toSend));
        }
    }
}
