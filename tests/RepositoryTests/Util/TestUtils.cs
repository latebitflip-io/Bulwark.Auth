using System;
namespace RepositoryTests.Util
{
    public static class TestUtils
    {
        public static string GenerateEmail()
        {
            return $"{Guid.NewGuid()}@lateflip.io";
        }
    }
}
