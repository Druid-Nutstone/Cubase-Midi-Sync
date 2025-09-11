using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Server.Tests
{
    public class BaseTest
    {
        protected System.Net.Http.HttpClient Client { get; }

        public BaseTest()
        {
            var factory = new WebFactory();
            this.Client = factory.CreateClient();
        }
    }
}
