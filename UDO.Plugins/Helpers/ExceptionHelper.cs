using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace VA.AppInsights
{
    public class ExceptionHelper
    {
        public static List<ParsedStack> GetParsedStacked(Exception e)
        {
            if (string.IsNullOrEmpty(e.StackTrace))
                return null;

            List<ParsedStack> parsedStacks = new List<ParsedStack>();

            Exception currentException = e;
            while (currentException != null)
            {
                ParsedStack parsedStack = ParseStackTrace(e);
                parsedStacks.Add(parsedStack);

                currentException = currentException.InnerException;
            }

            return parsedStacks;
        }

        private static ParsedStack ParseStackTrace(Exception e)
        {
            StackTrace stackTrace = new StackTrace(e);
            StackFrame stackFrame = stackTrace.GetFrame(0);
            ParsedStack aiParsedStack = new ParsedStack
            {
                Method = stackFrame.GetMethod().Name,
                FileName = stackFrame.GetFileName(),
                Line = stackFrame.GetFileLineNumber()
            };

            return aiParsedStack;
        }
    }
}