using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recollect.Tests.Models
{
	public class Sku
	{
		public string Code { get; set; }

		public string[] VariationValues {get;set;}

		public decimal Price { get; set; }
		public decimal Cost { get; set; }
		public decimal QuantityOnHand { get; set; }

	}
}