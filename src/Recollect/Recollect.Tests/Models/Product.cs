﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recollect.Tests.Models
{
	public class Product
	{
		public int Id { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }
		
		public string[] VariationNames { get; set; }

		public List<Sku> Skus { get; set; }
	}
}