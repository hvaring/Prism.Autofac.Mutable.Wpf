using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using CommonServiceLocator;
using Prism.Regions;

namespace Prism.Autofac.Mutable.Wpf.Regions
{
    public class AutofacRegionNavigationContentLoader : RegionNavigationContentLoader
    {
        private readonly ILifetimeScope _lifetimeScope;

        public AutofacRegionNavigationContentLoader(IServiceLocator serviceLocator, ILifetimeScope lifetimeScope) : base(serviceLocator)
        {
            _lifetimeScope = lifetimeScope;
        }

        /// <summary>
        /// Returns the set of candidates that may satisfiy this navigation request.
        /// </summary>
        /// <param name="region">The region containing items that may satisfy the navigation request.</param>
        /// <param name="candidateNavigationContract">The candidate navigation target.</param>
        /// <returns>An enumerable of candidate objects from the <see cref="IRegion"/></returns>
        protected override IEnumerable<object> GetCandidatesFromRegion(IRegion region, string candidateNavigationContract)
        {
            if (candidateNavigationContract == null || candidateNavigationContract.Equals(string.Empty))
                throw new ArgumentNullException(nameof(candidateNavigationContract));

            IEnumerable<object> contractCandidates = base.GetCandidatesFromRegion(region, candidateNavigationContract);

            if (!contractCandidates.Any())
            {
                //First try friendly name registration.
                var matchingRegistration = _lifetimeScope.ComponentRegistry.Registrations.FirstOrDefault(r => r.Services.OfType<KeyedService>().Any(s => s.ServiceKey.Equals(candidateNavigationContract)));

                //If not found, try type registration
                if (matchingRegistration == null)
                    matchingRegistration = _lifetimeScope.ComponentRegistry.Registrations.FirstOrDefault(r => candidateNavigationContract.Equals(r.Activator.LimitType.Name, StringComparison.Ordinal));

                if (matchingRegistration == null)
                    return new object[0];

                string typeCandidateName = matchingRegistration.Activator.LimitType.FullName;

                contractCandidates = base.GetCandidatesFromRegion(region, typeCandidateName);
            }

            return contractCandidates;
        }
    }
}