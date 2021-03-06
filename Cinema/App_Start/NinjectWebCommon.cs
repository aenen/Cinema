﻿[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Cinema.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(Cinema.App_Start.NinjectWebCommon), "Stop")]
namespace Cinema.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Ninject.Web.Common.WebHost;
    using Cinema.Data.Database;
    using System.Data.Entity;
    using Cinema.Data.Repository;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<DbContext>().To<CinemaContext>().InRequestScope();
            kernel.Bind<IRepository<Movie>>().To<MovieRepository>().InRequestScope();
            kernel.Bind<IRepository<CinemaEntity>>().To<CinemaRepository>().InRequestScope();
            kernel.Bind<IRepository<Session>>().To<SessionRepository>().InRequestScope();
            kernel.Bind<IRepository<Order>>().To<OrderRepository>().InRequestScope();
            kernel.Bind<IRepository<Ticket>>().To<TicketRepository>().InRequestScope();
        }
    }
}