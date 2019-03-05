﻿using Espeon.Databases.CommandStore;
using Espeon.Databases.GuildStore;
using Espeon.Databases.UserStore;
using Espeon.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Espeon.Extensions
{
    public static partial class Extensions
    {
        public static IServiceCollection AddServices(this IServiceCollection collection, IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                collection.AddSingleton(type);
            }

            return collection;
        }

        public static IServiceProvider Inject(this IServiceProvider services, IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                var service = services.GetService(type);

                Inject(services, service);
            }

            return services;
        }

        public static void Inject(this IServiceProvider services, object obj)
        {
            var members = obj.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.GetCustomAttributes(typeof(InjectAttribute), true).Length > 0)
                .ToArray();

            foreach (var member in members)
            {
                Type type;
                object value;

                switch (member)
                {
                    case FieldInfo fieldInfo:
                        type = fieldInfo.FieldType;

                        value = services.GetService(type);

                        if (value is null)
                            continue;

                        fieldInfo.SetValue(obj, value);
                        break;

                    case PropertyInfo propertyInfo:
                        type = propertyInfo.PropertyType;

                        value = services.GetService(type);

                        if (value is null)
                            continue;

                        propertyInfo.SetValue(obj, value);
                        break;
                }
            }
        }

        public static async Task RunInitialisersAsync(this IServiceProvider services, UserStore userStore, GuildStore guildStore, CommandStore commandStore, IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                var service = services.GetService(type);

                if (!(service is BaseService validService))
                    throw new InvalidServiceException($"{type}");

                await validService.InitialiseAsync(userStore, guildStore, commandStore, services);
            }
        }
    }
}