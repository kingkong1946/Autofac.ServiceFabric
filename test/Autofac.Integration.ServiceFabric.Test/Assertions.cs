﻿using Autofac.Core;
using Xunit;

namespace Autofac.Integration.ServiceFabric.Test
{
    internal static class Assertions
    {
        internal static void AssertRegistered<TService>(this IComponentContext context)
        {
            Assert.True(context.IsRegistered<TService>());
        }

        internal static void AssertSharing<TComponent>(this IComponentContext context, InstanceSharing sharing)
        {
            var cr = context.RegistrationFor<TComponent>();
            Assert.Equal(sharing, cr.Sharing);
        }

        internal static void AssertLifetime<TComponent, TLifetime>(this IComponentContext context)
        {
            var cr = context.RegistrationFor<TComponent>();
            Assert.IsType<TLifetime>(cr.Lifetime);
        }

        internal static void AssertOwnership<TComponent>(this IComponentContext context, InstanceOwnership ownership)
        {
            var cr = context.RegistrationFor<TComponent>();
            Assert.Equal(ownership, cr.Ownership);
        }

        internal static IComponentRegistration RegistrationFor<TComponent>(this IComponentContext context)
        {
            IComponentRegistration r;
            Assert.True(context.ComponentRegistry.TryGetRegistration(new TypedService(typeof(TComponent)), out r));
            return r;
        }
    }
}
