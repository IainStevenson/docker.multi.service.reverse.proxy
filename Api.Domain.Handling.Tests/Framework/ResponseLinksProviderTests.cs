using Api.Domain.Handling.Framework;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Domain.Handling.Tests.Framework
{
    [TestFixture]
    public class ResponseLinksProviderTests
    {
        private IResponseLinksProvider _unit;

        private Guid id = Guid.NewGuid();

        private string scheme = "https";
        private string host = "localhost";
        private string pathBase = "/api";
        private string path = "/resources";
        private string @namespace = "health/measurements/bloodpressure";
        private List<string> parentLinkActions;

        [SetUp]
        public void Setup()
        {
            _unit = new ResponseLinksProvider();
            parentLinkActions = new List<string>() { "post", "list", "get", "put", "delete" };

        }

        [Test]
        public async Task BuildRelatedLinksReturnsAFullSetOfLinks()
        {
            // Arrange 
            // as per setup

            var actual = await _unit.BuildLinks(scheme, host, pathBase, path, @namespace, $"{id}");

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(actual, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);

            Assert.That(actual, Is.Not.Empty); ;
            Assert.That(actual, Has.Count.EqualTo(parentLinkActions.Count));


            int index = 0;
            foreach (var action in actual)
            {
                Assert.That(actual[index].Action, Is.EqualTo(parentLinkActions[index]));
                Assert.That(actual[index].Rel, Is.EqualTo("health/measurements/bloodpressure"));
                Assert.That(actual[index].Href, Is.EqualTo(

                        index <= 1 ?
                        "https://localhost/api/resources/health/measurements/bloodpressure" : 
                        $"https://localhost/api/resources/{id}/health/measurements/bloodpressure"

                    ), "Failed testing HATEOAS index {0}", index);
                index++;
            }
        }
    }
}
