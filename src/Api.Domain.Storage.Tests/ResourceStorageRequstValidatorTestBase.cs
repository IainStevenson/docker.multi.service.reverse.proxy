using FluentValidation;
using FluentValidation.Results;

namespace Api.Domain.Storage.Tests
{

    [TestFixture]
    public class ResourceStorageRequstValidatorTestBase<TValidator, TRequest>
        where TValidator : AbstractValidator<TRequest>
    {
        protected TValidator Unit;
        protected TRequest Request;

        [SetUp]
        public void Setup()
        {
            Unit = Activator.CreateInstance<TValidator>();
            Request = Activator.CreateInstance<TRequest>();
        }

        /// <summary>
        /// Perform the test action
        /// </summary>
        /// <returns></returns>
        protected ValidationResult Act()
        {
            return Unit.Validate(Request);
        }
        /// <summary>
        /// Perform the test action validation
        /// </summary>
        /// <param name="result">The result value from the Act.</param>
        /// <param name="isValid">The assertion that it is valid or not</param>
        /// <param name="expectedErrorProperties">Th expected validation error properties.</param>

        protected void AssertTheResultsAreAsExpected(ValidationResult result, bool isValid, string expectedErrorProperties)
        {
            var expectedErrorCount = expectedErrorProperties.Split(',', StringSplitOptions.RemoveEmptyEntries).Count();
            var actualErrorProperties = result.Errors.Select(x => x.PropertyName).Aggregate(string.Empty, (current, next) => $"{current}{(current.Length == 0 ? "" : ",")}{next}");

            // Assert
            Assert.That(result.IsValid, Is.EqualTo(isValid));
            Assert.That(result.Errors.Count, Is.EqualTo(expectedErrorCount));
            Assert.That(actualErrorProperties, Is.EqualTo(expectedErrorProperties));
        }
    }
}
