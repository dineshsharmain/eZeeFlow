namespace eZeeFlow.Web
{
    using System.Web.Security;
    using Microsoft.Practices.Unity;
    
    /// <summary>
    /// Unity Container Factory
    /// </summary>
    /// <remarks></remarks>
    public class UnityContainerFactory
    {
        /// <summary>
        /// Creates the configured container.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope", Justification = "Container has the scope of the application.")
        ]
        public IUnityContainer CreateConfiguredContainer()
        {
            var container = new UnityContainer();
            LoadConfigurationOverrides(container);
            return container;
        }

        /// <summary>
        /// Loads the configuration overrides.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <remarks></remarks>
        private static void LoadConfigurationOverrides(IUnityContainer container)
        {
            container
          .RegisterInstance<MembershipProvider>(Membership.Provider);
        }
    }
}