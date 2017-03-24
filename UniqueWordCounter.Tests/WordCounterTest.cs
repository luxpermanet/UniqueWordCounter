using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace UniqueWordCounter.Tests
{
    [TestClass]
    public class WordCounterTest
    {
        private const string TestFilesFolderName = "TestFiles";
        private const string TestLine = "Go do that thing that you do so well";

        private void CreateFolder(string folderName)
        {
            if (!Directory.Exists(TestFilesFolderName))
                Directory.CreateDirectory(TestFilesFolderName);
        }

        private void CreateTestInputFile(string filePath, string lineToWrite, int repeatCount)
        {
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var sw = new StreamWriter(fs, Encoding.UTF8))
                for(int i = 0; i < repeatCount; i++)
                    sw.WriteLine(lineToWrite);
        }

        private void DeleteTestInputFile(string filePath)
        {
            File.Delete(filePath);
        }

        private void TestMethodBase(int fileReadParallelism, int lineProcessParallelism, int repeatCount)
        {
            var testFilePath = Path.Combine(TestFilesFolderName, $"TestInputFilex{repeatCount}.txt");

            CreateFolder(TestFilesFolderName);
            CreateTestInputFile(testFilePath, TestLine, repeatCount);

            var wordCounter = new WordCounter(testFilePath, Encoding.UTF8, fileReadParallelism, lineProcessParallelism);
            wordCounter.RunVerbose();
            //wordCounter.Run();

            var result = wordCounter.ReportRaw();

            Assert.AreEqual(1 * repeatCount, result["go"].OccurrenceCount);
            Assert.AreEqual(2 * repeatCount, result["do"].OccurrenceCount);
            Assert.AreEqual(2 * repeatCount, result["that"].OccurrenceCount);
            Assert.AreEqual(1 * repeatCount, result["thing"].OccurrenceCount);
            Assert.AreEqual(1 * repeatCount, result["you"].OccurrenceCount);
            Assert.AreEqual(1 * repeatCount, result["so"].OccurrenceCount);
            Assert.AreEqual(1 * repeatCount, result["well"].OccurrenceCount);

            DeleteTestInputFile(testFilePath);
        }

        [TestMethod]
        public void TestMethodx1()
        {
            TestMethodBase(1, 1, 1);
        }

        [TestMethod]
        public void TestMethodx100()
        {
            TestMethodBase(1, 1, 100);
        }

        [TestMethod]
        public void TestMethodx10000()
        {
            TestMethodBase(1, 100, 10000);
        }

        [TestMethod]
        public void TestMethodx100000()
        {
            TestMethodBase(10, 100, 100000);
        }
    }
}
