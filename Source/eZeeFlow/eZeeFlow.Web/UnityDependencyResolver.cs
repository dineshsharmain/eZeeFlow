namespace eZeeFlow.Web
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Unity Dependency Resolver
    /// </summary>
    /// <remarks></remarks>
    public class UnityDependencyResolver : IDependencyResolver
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IUnityContainer unity;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityDependencyResolver"/> class.
        /// </summary>
        /// <param name="unity">The unity.</param>
        /// <remarks></remarks>
        public UnityDependencyResolver(IUnityContainer unity)
        {
            this.unity = unity;
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public object GetService(Type serviceType)
        {
            try
            {
                return this.unity.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                // By definition of IDependencyResolver contract, this should return null if it cannot be found.
                return null;
            }
        }

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return this.unity.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                // By definition of IDependencyResolver contract, this should return null if it cannot be found.
                return null;
            }
        }
    }
}