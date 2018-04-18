using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nett.Collections;

namespace Nett.Parser.Cst
{
    internal abstract class Node : IGetChildren<Node>
    {
        protected Token CheckTokenType(TokenType expected, Token t)
        {
            if (t.type != expected)
            {
                throw new ArgumentException($"Expected token of type '{expected}' but actual token has type '{t.type}'.");
            }

            return t;
        }

        public abstract IEnumerable<Node> Children { get; }

        IEnumerable<Node> IGetChildren<Node>.GetChildren()
            => this.Children;

        public string PrintTree()
        {
            var builder = new StringBuilder();
            var allNodes = this.TraversePreOrderWithDepth();

            foreach (var n in allNodes)
            {
                builder.Append(new string(' ', n.Depth))
                    .AppendLine(n.Node.ToString());
            }

            return builder.ToString();
        }

        protected static IEnumerable<Node> NodesAsEnumerable(params IReq<Node>[] nodes)
            => nodes.Select(n => n.Node);

        protected static IEnumerable<Node> NonNullNodesAsEnumerable(params INode<Node>[] nodes)
            => nodes.Select(n => n.Node).Where(n => n != null);
    }
}
