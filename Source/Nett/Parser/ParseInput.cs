using System;
using System.Collections.Generic;
using Nett.Parser.Cst;

namespace Nett.Parser
{
    internal sealed partial class ParseInput : IParseInput
    {
        private static readonly Token NoTokenAvailable = Token.EndOfFile(-1, -1);

        private readonly List<Token> tokens;

        private int index = 0;

        public ParseInput(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public bool IsFinished
            => this.index > this.tokens.Count - 1;

        private Token CurrentToken =>
            this.index < this.tokens.Count
                ? this.tokens[this.index]
                : NoTokenAvailable;

        public Token Advance()
           => this.tokens[this.index++];

        public SyntaxErrorNode CreateErrorNode()
            => SyntaxErrorNode.Unexpected(this.CurrentToken);

        public bool Peek(Func<Token, bool> predicate)
            => predicate(this.CurrentToken);
    }
}
