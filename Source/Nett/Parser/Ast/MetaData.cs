using System.Collections.Generic;

namespace Nett.Parser.Ast
{
    internal sealed class MetaNode
    {
        private readonly IEnumerable<Token> newlines;

        public MetaNode(IEnumerable<Token> newlines)
        {
            this.newlines = newlines;
        }
    }
}
