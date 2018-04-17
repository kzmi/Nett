using System;
using System.Collections.Generic;
using System.Linq;

namespace Nett.Parser.Ast
{
    internal sealed class TableNode : ExpressionNode
    {
        public TableNode(Token key)
        {
            if (key.type != TokenType.Key && key.type != TokenType.BareKey)
            {
                throw new ArgumentException($"Token has to be of type 'key' but is '{key.type}'.");
            }

            this.Key = key;
        }

        public Token Key { get; }

        public override IEnumerable<Node> Children
            => Enumerable.Empty<Node>();

        public override string ToString()
            => $"E~[{this.Key.value}]";
    }
}
