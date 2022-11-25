using Api.Domain.Storage.Delete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.TestHelper;


namespace Api.Domain.Storage.Tests.Delete
{
    [TestFixture]
    public class ResourceStorageDeleteRequestValidatorTests
    {
        private ResourceStorageDeleteRequestValidator _unit;
        private ResourceStorageDeleteRequest _subject;

        [SetUp]
        public void Setup()
        {
            _unit = new ResourceStorageDeleteRequestValidator();
            _subject = new ResourceStorageDeleteRequest()
            {
                Id = Guid.NewGuid(),
                OwnerId = Guid.NewGuid(),
                RequestId = Guid.NewGuid(),
            };

            // Rules are:
            //RuleFor(x => x.Id).NotEmpty();
            //RuleFor(x => x.Namespace).NotNull();
            //RuleFor(x => x.Namespace).Matches("^(?:(?:((?![0-9_])[a-zA-Z0-9_]+)\\.?)+)(?<!\\.)$");
            //RuleFor(x => x.OwnerId).NotEmpty();
            //RuleFor(x => x.RequestId).NotEmpty();
        }

        [Test]
        public void ValidateThatADefaultRequestIsInvalid() {
        
            var result = _unit.Validate(new ResourceStorageDeleteRequest());
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
            Assert.That(result.Errors.Count, Is.EqualTo(3)); 
            Assert.That(result.Errors[0].PropertyName, Is.EqualTo("Id")); 
            Assert.That(result.Errors[1].PropertyName, Is.EqualTo("OwnerId")); 
            Assert.That(result.Errors[2].PropertyName, Is.EqualTo("RequestId")); 

        }

        [Test]
        public void ValidateThatACorrecltySetupRequestIsValid()
        {
           
            var result = _unit.Validate(_subject);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Errors, Is.Empty);
        }

        [Test]
        public void ValidateThatARequestWithoutANamespaceIsInvalid()
        {
            _subject.Namespace = string.Empty;
            var result = _unit.Validate(_subject);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
            Assert.That(result.Errors[0].PropertyName, Is.EqualTo("Namespace"));
        }

        [Test]
        public void ValidateThatARequestWithaBadNamespaceIsInvalid()
        {
            _subject.Namespace = "my/namespace";
            var result = _unit.Validate(_subject);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
            Assert.That(result.Errors[0].PropertyName, Is.EqualTo("Namespace"));
        }
        [Test]
        public void ValidateThatARequestWithaGoodamespaceIsValid()
        {
            _subject.Namespace = "my.namespace";
            var result = _unit.Validate(_subject);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Errors, Is.Empty);

        }
    }
}
