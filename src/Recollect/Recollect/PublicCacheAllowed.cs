using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recollect
{
	/// <summary>
	/// Determines if/when public caching of a response is allowed.
	/// </summary>
	public enum PublicCacheAllowed
	{
		/// <summary>
		/// No responses should be stored in public caches.
		/// </summary>
		Never = 0,
		/// <summary>
		/// All responses should be stored in public caches.
		/// </summary>
		Always,
		/// <summary>
		/// Only responses where <see cref="Thread.CurrentPrincipal.Identity.IsAuthenticated"/> returns false are cached. This normally indicates an anonymous user.
		/// </summary>
		Anonymous,
		/// <summary>
		/// Only responses where the Authorization header on the request is empty are cached. This can also indicate an anonymous user for auth systems that don't integrate to identites/principals within ASP.Net.
		/// </summary>
		NoAuthHeader,
		/// <summary>
		/// A combination of <see cref="Anonymous"/> and <see cref="NoAuthHeader"/>, responses are only cached if the current identity is not authenticated AND the Authorization header is empty on the request.
		/// </summary>
		AnonymousAndNoAuthHeader
	}
}