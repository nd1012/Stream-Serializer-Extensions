using wan24.ObjectValidation;

namespace Stream_Serializer_Extensions_Tests
{
    [TestClass]
    public class A_Initialization
    {
        public A_Initialization() => ValidateObject.Logger = (message) => Console.WriteLine(message);

        [TestMethod]
        public void Logger_Test() => ValidateObject.Logger("Stream-Serializer-Tests initialized");
    }
}
