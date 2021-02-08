using System;
using Xunit.Abstractions;

namespace Testing
{
    /// <summary>
    ///     Base test class for setting up unit or integration tests
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TestBase<T> : TestBase where T : class
    {
        protected Func<Action, bool> Executes = action =>
        {
            try
            {
                action.Invoke();
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception e)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                // Console.WriteLine(e);
                throw;
            }
            return true;
        };

        protected T Unit;

        public TestBase(ITestOutputHelper console) : base(console)
        {
            Arrange();
            Act();
        }

        /// <summary>
        ///     This Act does nothing, should override in each derived test class
        /// </summary>
        protected virtual void Act()
        {
        }

        /// <summary>
        ///     Simply creates a new instance of the type.
        ///     <remarks>
        ///         If the class has no default constructor then this method must be overrided and the Unit constructed in an
        ///         alternative way
        ///     </remarks>
        /// </summary>
        protected virtual void Arrange()
        {
            Unit = Activator.CreateInstance<T>();
        }

        /// <summary>
        ///     Disposes of the Unit if it is disposable
        /// </summary>
        protected virtual void Teardown()
        {
            (Unit as IDisposable)?.Dispose();
        }

        /// <summary>
        ///     Does nothing
        /// </summary>
        protected virtual void OneTimeSetUp()
        {
        }

        /// <summary>
        ///     Does nothing
        /// </summary>
        protected virtual void OneTimeTearDown()
        {
        }
    }
}
