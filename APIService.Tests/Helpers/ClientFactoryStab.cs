using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIService.Tests.Helpers
{
	internal class ClientFactoryStab : IHttpClientFactory
	{
		public HttpClient CreateClient(string name)
		{
			throw new NotImplementedException();
		}
	}
}