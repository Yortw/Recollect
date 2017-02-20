using Recollect.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Recollect.Tests.Controllers
{
	[RoutePrefix("api/info")]
	public class InfoController : ApiController
	{

		[Route("ServerTime")]
		[ApiOutputClientCache(NoCache = true, NoStore = true, NoTransform = true, MustRevalidate = true, PublicCache = PublicCacheAllowed.Never)]
		public DateTimeOffset GetServerTime()
		{
			return DateTimeOffset.Now;
		}
	}
}