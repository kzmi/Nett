using System;
using System.Collections.Generic;
using System.Linq;

namespace Nett.Parser.Ast
{
    internal sealed class CommentNode : ExpressionNode
    {
        public CommentNode(Token comment)
        {
            if (comment.type != TokenType.Comment)
            {
                throw new ArgumentException("Token is not a comment token.");
            }

            this.Comment = comment;
        }

        public Token Comment { get; }

        public override IEnumerable<Node> Children
            => Enumerable.Empty<Node>();

        public override string ToString()
            => $"c~{this.Comment.value}";
    }
}
