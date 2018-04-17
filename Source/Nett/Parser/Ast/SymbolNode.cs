using System.Collections.Generic;
using System.Linq;

namespace Nett.Parser.Ast
{
    internal class SymbolNode : Node
    {
        public SymbolNode(Token token)
        {
            this.Token = token;
        }

        public Token Token { get; }

        public override IEnumerable<Node> Children
            => Enumerable.Empty<Node>();

        public override string ToString()
            => $"s~{this.Token.value}";
    }
}
