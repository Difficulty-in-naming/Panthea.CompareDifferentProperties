using System.Collections.Generic;
using System.Text;

namespace Panthea.CompareDifferentProperties.Editor
{
    internal class InternalStringBuilder
    {
        private StringBuilder SB { get; }
        private List<InternalStringBuilder> Child { get; }
        public int Indeed { get; set; } = 0;
        public InternalStringBuilder()
        {
            SB = new StringBuilder();
            Child = new List<InternalStringBuilder>();
        }

        public InternalStringBuilder AppendLine(string value)
        {
            for (int i = 0; i < Indeed; i++)
            {
                value = "   " + value;
            }

            SB.AppendLine(value);
            return this;
        }
        
        public override string ToString()
        {
            return SB.ToString();
        }
    }
}