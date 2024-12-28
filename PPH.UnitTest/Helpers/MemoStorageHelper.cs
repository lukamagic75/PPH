using System.IO;

namespace PPH.UnitTest.Helpers
{
    public static class MemoStorageHelper
    {
        private const string TestDatabaseFile = "test_memosdb.sqlite3";

        public static void RemoveDatabaseFile()
        {
            if (File.Exists(TestDatabaseFile))
            {
                File.Delete(TestDatabaseFile);
            }
        }
    }
}