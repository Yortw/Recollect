using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http.Filters;

namespace Recollect
{
	/// <summary>
	/// An attribute applied to to action methods or controllers in order to control the cache-control header values for responses. This informs clients and intermediate proxies of caching rules.
	/// </summary>
	[AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class ApiOutputClientCache : System.Web.Http.Filters.ActionFilterAttribute
	{

		#region Public Properties

		/// <summary>
		/// The number of seconds the client is allowed to cache the response for. Zero indicates no caching. Negative values are treated as zero.
		/// </summary>
		/// <remarks>
		/// <para>This is value is used for the <see cref="System.Net.Http.Headers.CacheControlHeaderValue.MaxAge"/> value if it is greater than zero.</para>
		/// </remarks>
		public int ClientCacheSeconds { get; set; } = 0;

		/// <summary>
		/// If true then 304 Not Modified responses should include cache headers similar to the original response, causing the cached data's life time to be extended by the <see cref="ClientCacheSeconds"/> value before the next recheck.
		/// </summary>
		public bool ClientCacheExtendedOnNotModified { get; set; } = false;

		/// <summary>
		/// If true the client is told it must re-validate with the server when cached responses are considered stale.
		/// </summary>
		/// <remarks>
		/// <para>This is value is used for the <see cref="System.Net.Http.Headers.CacheControlHeaderValue.MustRevalidate"/> value if it is greater than zero.</para>
		/// </remarks>
		public bool MustRevalidate { get; set; } = true;

		/// <summary>
		/// Tells clients not to cache or store responses.
		/// </summary>
		public bool NoStore { get; set; } = false;

		/// <summary>
		/// Tells clients they must always re-validate with the server before using any cached response, even if it is not yet stale.
		/// </summary>
		public bool NoCache { get; set; } = false;

		/// <summary>
		/// Controls if and when responses can be cached in shared/public caches (such as intermediate proxy/cache servers).
		/// </summary>
		/// <remarks>
		/// <para>See https://tools.ietf.org/html/rfc7234#section-3 for details on normal rules for when caching is allowed.</para>
		/// </remarks>
		public PublicCacheAllowed PublicCache { get; set; } = PublicCacheAllowed.Never;

		/// <summary>
		/// Specifies the time, in seconds, that a response can be stored in a public/shared cache.
		/// </summary>
		/// <seealso cref="PublicCache"/>
		public int SharedCacheSeconds { get; set; } = 0;

		/// <summary>
		/// Whether a cache or proxy must NOT change any aspect of the entity body.
		/// </summary>
		public bool NoTransform { get; set; } = false;

		/// <summary>
		/// A comma separated list of header names used to vary the cached responses. The client must use these as part of the key when creating/using cached responses.
		/// </summary>
		public string VaryHeaders { get; set; } = null;

		/// <summary>
		/// A comma separated list of header names placed in the private cache-control header.
		/// </summary>
		public string PrivateHeaders { get; set; } = null;

		/// <summary>
		/// Whether a shared cache or proxy must revalidate with the origin server when a response is stale.
		/// </summary>
		public bool ProxyRevalidate { get; set; } = false;

		#endregion

		#region Overrides

		public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
		{
			if (ShouldSetCacheHeader(actionExecutedContext))
				SetCacheHeaders(actionExecutedContext.Response, actionExecutedContext.Request);

			base.OnActionExecuted(actionExecutedContext);
		}

		#endregion

		#region Private Methids

		private static bool ShouldSetCacheHeader(HttpActionExecutedContext actionExecutedContext)
		{
			return actionExecutedContext.Response != null
				&& actionExecutedContext.Response.Headers.CacheControl == null
				&& actionExecutedContext.Response.IsSuccessStatusCode;
		}

		private void SetCacheHeaders(HttpResponseMessage response, HttpRequestMessage request)
		{
			response.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue();
			response.Headers.CacheControl.NoStore = NoStore;
			response.Headers.CacheControl.NoCache = NoCache;

			if (NoCache && response.Version.Major == 1 && response.Version.Minor == 0)
				response.Headers.Add("Pragma", "no-cache");

			if (!NoCache && !NoStore) // Settings only apply if caching allowed.
			{
				response.Headers.CacheControl.MaxAge = TimeSpan.FromSeconds(ClientCacheSeconds);
				if (ClientCacheSeconds > 0 && response.Content != null)
					response.Content.Headers.Expires = DateTimeOffset.UtcNow.AddSeconds(ClientCacheSeconds);
				
				response.Headers.CacheControl.SharedMaxAge = TimeSpan.FromSeconds(SharedCacheSeconds);

				response.Headers.CacheControl.NoTransform = NoTransform;
				response.Headers.CacheControl.MustRevalidate = MustRevalidate;
				response.Headers.CacheControl.Public = IsPubliclyCacheable(PublicCache, request);
				response.Headers.CacheControl.ProxyRevalidate = ProxyRevalidate;

				if (!String.IsNullOrEmpty(PrivateHeaders))
					ApplyCommaSeparatedListToHeader(response.Headers.CacheControl.PrivateHeaders, PrivateHeaders);
			}

			// Set the vary header on response.
			if (!String.IsNullOrEmpty(VaryHeaders))
				ApplyCommaSeparatedListToHeader(response.Headers.Vary, VaryHeaders);
		}

		private void ApplyCommaSeparatedListToHeader(ICollection<string> header, string commaSeparatedList)
		{
			foreach (var varyHeader in commaSeparatedList.Split(InternalConstants.HeaderSplitChars))
			{
				if (!header.Contains(varyHeader))
					header.Add(varyHeader);
			}
		}

		private bool IsPubliclyCacheable(PublicCacheAllowed publicCacheSetting, HttpRequestMessage request)
		{
			switch (publicCacheSetting)
			{
				case PublicCacheAllowed.Always:
					return true;
				case PublicCacheAllowed.Anonymous:
					return !(System.Threading.Thread.CurrentPrincipal?.Identity?.IsAuthenticated ?? false);
				case PublicCacheAllowed.AnonymousAndNoAuthHeader:
					return !(System.Threading.Thread.CurrentPrincipal?.Identity?.IsAuthenticated ?? false)
						&& IsAuthHeaderEmpty(request?.Headers?.Authorization);
				case PublicCacheAllowed.NoAuthHeader:
					return IsAuthHeaderEmpty(request?.Headers?.Authorization);
				default:
					return false;
			}
		}

		private bool IsAuthHeaderEmpty(AuthenticationHeaderValue authorization)
		{
			return authorization == null ||
				(String.IsNullOrEmpty(authorization.Scheme) && String.IsNullOrEmpty(authorization.Parameter));
		}

		#endregion

	}
}