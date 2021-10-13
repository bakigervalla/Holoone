using Flurl.Http;
using Flurl.Http.Configuration;

namespace Holoone.Api.Helpers
{
    public class PerBaseUrlFlurlClientFactory : FlurlClientFactoryBase
	{
		/// <summary>
		/// Returns the entire URL, which is assumed to be some "base" URL for a service.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns>The cache key</returns>
		protected override string GetCacheKey(Flurl.Url url) => url.ToString();

		/// <summary>
		/// Returns a new new FlurlClient with BaseUrl set to the URL passed.
		/// </summary>
		/// <param name="url">The URL</param>
		/// <returns></returns>
		protected override IFlurlClient Create(Flurl.Url url) => new FlurlClient(url);

    }
}
