using Microsoft.Extensions.Logging;
using wan24.Core;
using wan24.ObjectValidation;
using wan24.StreamSerializerExtensions;

namespace Stream_Serializer_Extensions_Tests
{
    [TestClass]
    public class A_Initialization
    {
        public static ILoggerFactory LoggerFactory { get; private set; } = null!;

        [AssemblyInitialize]
        public static async Task Init(TestContext tc)
        {
            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(b => b.AddConsole());
            Logging.Logger = LoggerFactory.CreateLogger("Tests");
            ValidateObject.Logger = (message) => Console.WriteLine(message);
            TypeHelper.Instance.ScanAssemblies(typeof(A_Initialization).Assembly);
            await Bootstrap.Async();
            StreamSerializer.OnLoadType += (e) => e.Type ??= TypeHelper.Instance.GetType(e.Name);
            Logging.WriteDebug("Stream-Serializer-Extensions Tests initialized");
        }
    }
}
