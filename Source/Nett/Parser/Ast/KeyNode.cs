using System.Collections.Generic;
using System.Linq;

namespace Nett.Parser.Ast
{
    internal sealed class KeyNode : Node
    {
        public KeyNode(Token key)
        {
            this.Key = key;
        }

        public Token Key { get; }

        public override IEnumerable<Node> Children
            => Enumerable.Empty<Node>();

        public override string ToString()
            => $"k~{this.Key.value}";
    }
}
