﻿using System.Text;

namespace Microsoft.AzureKinect.Test.StubGenerator
{
    /// <summary>
    /// A string representation of C code
    /// </summary>
    /// <remarks>
    /// This class captures source and line metadata for the location where the string was assigned.
    /// 
    /// This information is used when compiling the code, or in debugging the code, to reference back to the
    /// source location where the string was assigned.
    /// </remarks>
    public class CodeString
    {
        private CodeString()
        {
        }

        /// <summary>
        /// Create a CodeString with some C code
        /// </summary>
        /// <param name="code">The C code string to represent.</param>
        public CodeString(string code)
        {
            AssignCode(code);
        }

        private string code = "";

        /// <summary>
        /// Performs the assignment of the code block and looks through the stack to find the origin information, filename and line number.
        /// </summary>
        /// <param name="code">The C code string to represent.</param>
        private void AssignCode(string code)
        {
            int frameDepth = 0;
            var trace = new System.Diagnostics.StackTrace(true);
            var frame = trace.GetFrame(frameDepth);

            while (frame.GetMethod().DeclaringType == typeof(CodeString))
            {
                frame = trace.GetFrame(++frameDepth);
            }

            SourceLineNumber = frame.GetFileLineNumber();
            SourceFileName = frame.GetFileName();

            this.code = code;
        }

        public string Code
        {
            get
            {
                return code;
            }
            set
            {
                AssignCode(code);
            }
        }

        public string SourceFileName { get; private set; } = "";
        public int SourceLineNumber { get; private set; } = 0;

        public static implicit operator string(CodeString s)
        {
            return s.Code;
        }

        public static implicit operator CodeString(string s)
        {
            CodeString c = new CodeString();
            c.AssignCode(s);
            return c;
        }

        /// <summary>
        /// Returns a version of the code with #line data
        /// </summary>
        public string EmbdededLineData
        {
            get
            {
                StringBuilder source = new StringBuilder();

                source.AppendLine($"#line {SourceLineNumber} \"{SourceFileName.Replace("\\", "\\\\")}\"");
                source.AppendLine(Code);
                return source.ToString();
            }
        }

        public override string ToString()
        {
            return Code;
        }
    }
}