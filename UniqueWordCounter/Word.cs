using System;
using System.Collections.Generic;
using System.Text;

namespace UniqueWordCounter
{
    public class Word
    {
        public string Content { set; get; }
        public int FirstLineOfOccurrence { set; get; }
        public int FirstLinePosOfOccurrence { set; get; }
        public int OccurrenceCount { set; get; }

        public Word(string content)
            : this(content, 0, 0, 1)
        { }

        public Word(string content, int firstLineOfOccurrence, int firstLinePosOfOccurrence, int occurrenceCount)
        {
            Content = content;
            FirstLineOfOccurrence = firstLineOfOccurrence;
            FirstLinePosOfOccurrence = firstLinePosOfOccurrence;
            OccurrenceCount = occurrenceCount;
        }

        public override string ToString()
        {
            return Content.ToString();
        }
    }
}
