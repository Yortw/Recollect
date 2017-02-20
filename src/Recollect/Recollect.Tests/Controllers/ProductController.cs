using Recollect.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Recollect.Tests.Controllers
{
	[RoutePrefix("api/product")]
	public class ProductController : ApiController
	{

		#region Static Members

		private static List<Product> _Products = new List<Product>();

		static ProductController()
		{
			_Products.Add(new Product()
			{
				Code = "JLF10",
				Description = "Suit Jacket",
				Id = 1,
				VariationNames = new string[] { "Colour", "Size" },
				Skus = new List<Sku>()
				{
					new Sku()
					{
						Code = "20000332",
						Cost = 56M,
						Price = 99.95M,
						QuantityOnHand = 10,
						VariationValues = new string[] { "Navy", "102L" }
					},
					new Sku()
					{
						Code = "20000334",
						Cost = 56M,
						Price = 99.95M,
						QuantityOnHand = 10,
						VariationValues = new string[] { "Red", "102M" }
					},
					new Sku()
					{
						Code = "20000335",
						Cost = 56M,
						Price = 99.95M,
						QuantityOnHand = 16,
						VariationValues = new string[] { "Red", "102S" }
					},
					new Sku()
					{
						Code = "20000336",
						Cost = 56M,
						Price = 99.95M,
						QuantityOnHand = 9,
						VariationValues = new string[] { "Red", "102L" }
					},
					new Sku()
					{
						Code = "20000335",
						Cost = 56M,
						Price = 99.95M,
						QuantityOnHand = 8,
						VariationValues = new string[] { "Red", "102M" }
					},
					new Sku()
					{
						Code = "20000336",
						Cost = 56M,
						Price = 99.95M,
						QuantityOnHand = 12,
						VariationValues = new string[] { "Red", "102S" }
					},
				}
			});

		}

		#endregion

		[Route("{id}")]
		[ApiOutputClientCache(ClientCacheSeconds = 60, MustRevalidate = true, ProxyRevalidate = true, NoTransform = true,
			PublicCache = PublicCacheAllowed.Always,
			SharedCacheSeconds = 60,
			VaryHeaders = "accept,accept-encoding,accept-language,accept-charset",
			PrivateHeaders = "X-Custom-ResponseId,X-Custom-RequestId")
		]
		public Product Get(int id)
		{
			var product = (from p in _Products where p.Id == id select p).SingleOrDefault();
			if (product == null) throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
			return product;
		}
	}
}