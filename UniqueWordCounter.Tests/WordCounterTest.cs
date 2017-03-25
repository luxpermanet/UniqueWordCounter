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

            Assert.AreEqual(1 * repeatCount, result.ContainsKey("go") ? result["go"].OccurrenceCount : 0);
            Assert.AreEqual(2 * repeatCount, result.ContainsKey("do") ? result["do"].OccurrenceCount : 0);
            Assert.AreEqual(2 * repeatCount, result.ContainsKey("that") ? result["that"].OccurrenceCount : 0);
            Assert.AreEqual(1 * repeatCount, result.ContainsKey("thing") ? result["thing"].OccurrenceCount : 0);
            Assert.AreEqual(1 * repeatCount, result.ContainsKey("you") ? result["you"].OccurrenceCount : 0);
            Assert.AreEqual(1 * repeatCount, result.ContainsKey("so") ? result["so"].OccurrenceCount : 0);
            Assert.AreEqual(1 * repeatCount, result.ContainsKey("well") ? result["well"].OccurrenceCount : 0);

            DeleteTestInputFile(testFilePath);
        }

        [TestMethod]
        public void TestNoLine()
        {
            TestMethodBase(1, 1, 0);
        }

        [TestMethod]
        public void TestOneLine()
        {
            TestMethodBase(1, 1, 1);
        }

        [TestMethod]
        public void TestHundredLines()
        {
            TestMethodBase(1, 1, 100);
        }

        [TestMethod]
        public void TestTenThousandLines()
        {
            TestMethodBase(1, 100, 10000);
        }

        [TestMethod]
        public void TestMillionLines()
        {
            TestMethodBase(10, 100, 1000000);
        }
    }
}
