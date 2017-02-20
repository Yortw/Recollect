using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace Recollect.Tests
{
	[RoutePrefix("api/")]
	[TestClass]
	public class ClientCacheClientTests
	{

		#region Test Support 

		private HttpServer _Server;
		private string _ProductsRootUrl = "http://localhost/api/product";
		private string _InfoRootUrl = "http://localhost/api/info";

		[TestInitialize]
		public void Initialise()
		{
			var serverConfig = new HttpConfiguration();

			serverConfig.MapHttpAttributeRoutes();

			serverConfig.Routes.MapHttpRoute
			(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{action}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);

			_Server = new HttpServer(serverConfig);
		}

		[TestCleanup]
		public void Cleanup()
		{
			_Server.Dispose();
		}

		#endregion

		[TestMethod]
		public async Task ApiOutputClientCache_SetsCacheControlHeader()
		{
			var client = new HttpClient(_Server);
			var result = await client.GetAsync(_ProductsRootUrl + "/1").ConfigureAwait(false);
			result.EnsureSuccessStatusCode();

			Assert.IsNotNull(result.Headers.CacheControl, "CacheControl header is null");
		}

		[TestMethod]
		public async Task ApiOutputClientCache_CacheControlHeader_SetsMaxAge()
		{
			var client = new HttpClient(_Server);
			var result = await client.GetAsync(_ProductsRootUrl + "/1").ConfigureAwait(false);
			result.EnsureSuccessStatusCode();

			Assert.AreEqual(TimeSpan.FromMinutes(1), result.Headers.CacheControl.MaxAge, "MaxAge has unexpected value");
		}

		[TestMethod]
		public async Task ApiOutputClientCache_CacheControlHeader_SetsContentExpires()
		{
			var client = new HttpClient(_Server);
			var result = await client.GetAsync(_ProductsRootUrl + "/1").ConfigureAwait(false);
			result.EnsureSuccessStatusCode();

			Assert.IsNotNull(result.Content.Headers.Expires, "Content.Expires is null");
		}

		[TestMethod]
		public async Task ApiOutputClientCache_CacheControlHeader_SetsVaryHeader()
		{
			var client = new HttpClient(_Server);
			var result = await client.GetAsync(_ProductsRootUrl + "/1").ConfigureAwait(false);
			result.EnsureSuccessStatusCode();

			Assert.IsNotNull(result.Headers.Vary, "Vary header is null");
			Assert.AreEqual(4, result.Headers.Vary.Count);
			Assert.IsTrue(result.Headers.Vary.Contains("accept"), "accept missing from vary header.");
			Assert.IsTrue(result.Headers.Vary.Contains("accept-encoding"), "accept-encoding missing from vary header.");
			Assert.IsTrue(result.Headers.Vary.Contains("accept-charset"), "accept-charset missing from vary header.");
			Assert.IsTrue(result.Headers.Vary.Contains("accept-language"), "accept-language missing from vary header.");
		}

		[TestMethod]
		public async Task ApiOutputClientCache_CacheControlHeader_SetsSharedMaxAge()
		{
			var client = new HttpClient(_Server);
			var result = await client.GetAsync(_ProductsRootUrl + "/1").ConfigureAwait(false);
			result.EnsureSuccessStatusCode();

			Assert.AreEqual(TimeSpan.FromMinutes(1), result.Headers.CacheControl.SharedMaxAge, "SharedMaxAge has unexpected value");
		}

		[TestMethod]
		public async Task ApiOutputClientCache_CacheControlHeader_SetsPublicCache()
		{
			var client = new HttpClient(_Server);
			var result = await client.GetAsync(_ProductsRootUrl + "/1").ConfigureAwait(false);
			result.EnsureSuccessStatusCode();

			Assert.IsTrue(result.Headers.CacheControl.Public, "Public has unexpected value");
		}

		[TestMethod]
		public async Task ApiOutputClientCache_CacheControlHeader_SetsMustRevalidate()
		{
			var client = new HttpClient(_Server);
			var result = await client.GetAsync(_ProductsRootUrl + "/1").ConfigureAwait(false);
			result.EnsureSuccessStatusCode();

			Assert.IsTrue(result.Headers.CacheControl.MustRevalidate, "MustRevalidate has unexpected value");
		}

		[TestMethod]
		public async Task ApiOutputClientCache_CacheControlHeader_SetsProxyRevalidate()
		{
			var client = new HttpClient(_Server);
			var result = await client.GetAsync(_ProductsRootUrl + "/1").ConfigureAwait(false);
			result.EnsureSuccessStatusCode();

			Assert.IsTrue(result.Headers.CacheControl.ProxyRevalidate, "ProxyRevalidate has unexpected value");
		}

		[TestMethod]
		public async Task ApiOutputClientCache_CacheControlHeader_SetsNoTransform()
		{
			var client = new HttpClient(_Server);
			var result = await client.GetAsync(_ProductsRootUrl + "/1").ConfigureAwait(false);
			result.EnsureSuccessStatusCode();

			Assert.IsTrue(result.Headers.CacheControl.NoTransform, "NoTransform has unexpected value");
		}

		[TestMethod]
		public async Task ApiOutputClientCache_CacheControlHeader_SetsPrivateHeaders()
		{
			var client = new HttpClient(_Server);
			var result = await client.GetAsync(_ProductsRootUrl + "/1").ConfigureAwait(false);
			result.EnsureSuccessStatusCode();

			Assert.IsNotNull(result.Headers.CacheControl.PrivateHeaders);
			Assert.AreEqual(2, result.Headers.CacheControl.PrivateHeaders.Count);
			Assert.IsTrue(result.Headers.CacheControl.PrivateHeaders.Contains("X-Custom-ResponseId"));
			Assert.IsTrue(result.Headers.CacheControl.PrivateHeaders.Contains("X-Custom-RequestId"));
		}

		[TestMethod]
		public async Task ApiOutputClientCache_CacheControlHeader_SetsNoCache()
		{
			var client = new HttpClient(_Server);
			var result = await client.GetAsync(_InfoRootUrl + "/ServerTime").ConfigureAwait(false);
			result.EnsureSuccessStatusCode();

			Assert.IsTrue(result.Headers.CacheControl.NoCache);
		}

		[TestMethod]
		public async Task ApiOutputClientCache_CacheControlHeader_SetsNoStore()
		{
			var client = new HttpClient(_Server);
			var result = await client.GetAsync(_InfoRootUrl + "/ServerTime").ConfigureAwait(false);
			result.EnsureSuccessStatusCode();

			Assert.IsTrue(result.Headers.CacheControl.NoStore);
			Assert.IsFalse(result.Headers.CacheControl.Public);
		}

	}
}