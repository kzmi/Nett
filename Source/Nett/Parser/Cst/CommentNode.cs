using System.Collections.Generic;

namespace Nett.Parser.Cst
{
    internal sealed class CommentNode : Node
    {
        public CommentNode(Token comment)
        {
            this.Comment = new TerminalNode(comment).Req();
        }

        public IReq<TerminalNode> Comment { get; }

        public override IEnumerable<Node> Children
            => NodesAsEnumerable(this.Comment);

        public override string ToString()
            => "C";
    }

    internal sealed class CommentExpressionNode : ExpressionNode
    {
        public CommentExpressionNode(Token comment)
        {
            this.Comment = new TerminalNode(comment).Req();
        }

        public IReq<TerminalNode> Comment { get; }

        public override IEnumerable<Node> Children
            => NodesAsEnumerable(this.Comment);

        public override string ToString()
            => "CE";
    }
}
