using System;
namespace BulwarkCoreTests.Util
{
    public static class TestUtils
    {
        public static string GenerateEmail()
        {
            return Guid.NewGuid().ToString() + "@bulwark.test";
        }
    }
}
