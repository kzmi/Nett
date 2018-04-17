using System.Collections.Generic;
using System.Linq;

namespace Nett.Parser.Ast
{
    internal sealed class IntValueNode : ValueNode
    {
        public IntValueNode(Token token)
        {
            this.Value = token;
        }

        public Token Value { get; }

        public override IEnumerable<Node> Children
            => Enumerable.Empty<Node>();

        public override string ToString()
            => $"V~{this.Value.value}";
    }
}
