// This software is part of the Autofac IoC container
// Copyright © 2017 Autofac Contributors
// http://autofac.org
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using Autofac.Builder;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Autofac.Integration.ServiceFabric
{
    /// <summary>
    /// Extension methods for registering services (stateful and stateless) with Autofac and Service Fabric.
    /// </summary>
    public static class AutofacServiceExtensions
    {
        /// <summary>
        /// Registers a stateful service with the container.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        /// <param name="serviceTypeName">ServiceTypeName as provided in service manifest.</param>
        /// <typeparam name="TService">The type of the stateful service to register.</typeparam>
        /// <returns>A registration builder allowing further configuration of the component.</returns>
        /// <exception cref="ArgumentException">Thrown when <typeparamref name="TService"/> is not a valid service type.</exception>
        /// <remarks>The service will be wrapped in a dynamic proxy and must be public and not sealed.</remarks>
        public static IRegistrationBuilder<TService, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            RegisterStatefulService<TService>(
                this ContainerBuilder builder,
                string serviceTypeName)
            where TService : StatefulServiceBase
        {
            var registration = RegisterServiceWithContainer<TService, ServiceInterceptor>(builder, serviceTypeName);

            RegisterStatefulServiceInternal<TService>(builder, serviceTypeName);

            return registration;
        }

        /// <summary>
        /// Registers a stateful service with the container.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        /// <param name="serviceTypeName">ServiceTypeName as provided in service manifest.</param>
        /// <typeparam name="TService">The type of the stateful service to register.</typeparam>
        /// <typeparam name="TInterceptor">The type of the interceptor used with service</typeparam>
        /// <returns>A registration builder allowing further configuration of the component.</returns>
        /// <exception cref="ArgumentException">Thrown when <typeparamref name="TService"/> is not a valid service type.</exception>
        /// <remarks>The service will be wrapped in a dynamic proxy and must be public and not sealed.</remarks>
        public static IRegistrationBuilder<TService, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            RegisterStatefulService<TService, TInterceptor>(
                this ContainerBuilder builder,
                string serviceTypeName)
            where TService : StatefulServiceBase
            where TInterceptor : ServiceInterceptor
        {
            var registration = RegisterServiceWithContainer<TService, TInterceptor>(builder, serviceTypeName);

            RegisterStatefulServiceInternal<TService>(builder, serviceTypeName);

            return registration;
        }

        /// <summary>
        /// Registers a stateless service with the container.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        /// <param name="serviceTypeName">ServiceTypeName as provided in service manifest.</param>
        /// <typeparam name="TService">The type of the stateless service to register.</typeparam>
        /// <returns>A registration builder allowing further configuration of the component.</returns>
        /// <exception cref="ArgumentException">Thrown when <typeparamref name="TService"/> is not a valid service type.</exception>
        /// <remarks>The service will be wrapped in a dynamic proxy and must be public and not sealed.</remarks>
        public static IRegistrationBuilder<TService, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            RegisterStatelessService<TService>(
                this ContainerBuilder builder,
                string serviceTypeName)
            where TService : StatelessService
        {
            var registration = RegisterServiceWithContainer<TService, ServiceInterceptor>(builder, serviceTypeName);

            RegisterStatelessServiceInternal<TService>(builder, serviceTypeName);

            return registration;
        }

        /// <summary>
        /// Registers a stateless service with the container and custom interceptor.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        /// <param name="serviceTypeName">ServiceTypeName as provided in service manifest.</param>
        /// <typeparam name="TService">The type of the stateless service to register.</typeparam>
        /// <typeparam name="TInterceptor">The type of the interceptor used with service</typeparam>
        /// <returns>A registration builder allowing further configuration of the component.</returns>
        /// <exception cref="ArgumentException">Thrown when <typeparamref name="TService"/> is not a valid service type.</exception>
        /// <remarks>The service will be wrapped in a dynamic proxy and must be public and not sealed.</remarks>
        public static IRegistrationBuilder<TService, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            RegisterStatelessService<TService, TInterceptor>(
                this ContainerBuilder builder,
                string serviceTypeName)
            where TService : StatelessService
            where TInterceptor : ServiceInterceptor
        {
            var registration = RegisterServiceWithContainer<TService, TInterceptor>(builder, serviceTypeName);

            RegisterStatelessServiceInternal<TService>(builder, serviceTypeName);

            return registration;
        }

        private static void RegisterStatefulServiceInternal<TService>(
            this ContainerBuilder builder,
            string serviceTypeName)
            where TService : StatefulServiceBase
        {
            builder.RegisterBuildCallback(c =>
                c.Resolve<IStatefulServiceFactoryRegistration>()
                    .RegisterStatefulServiceFactory<TService>(c, serviceTypeName));
        }

        private static void RegisterStatelessServiceInternal<TService>(
                this ContainerBuilder builder,
                string serviceTypeName)
            where TService : StatelessService
        {
            builder.RegisterBuildCallback(c =>
                c.Resolve<IStatelessServiceFactoryRegistration>()
                    .RegisterStatelessServiceFactory<TService>(c, serviceTypeName));
        }

        private static IRegistrationBuilder<TService, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            RegisterServiceWithContainer<TService, TInterceptor>(
                ContainerBuilder builder,
                string serviceTypeName)
            where TService : class
            where TInterceptor : ServiceInterceptor
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (string.IsNullOrEmpty(serviceTypeName))
                throw new ArgumentException("The service type name must be provided", nameof(serviceTypeName));

            var serviceType = typeof(TService);

            if (!serviceType.CanBeProxied())
                throw new ArgumentException(serviceType.GetInvalidProxyTypeErrorMessage());

            var registration = builder.RegisterServiceWithInterception<TService, TInterceptor>();

            registration.EnsureRegistrationIsInstancePerLifetimeScope();

            return registration;
        }
    }
}
