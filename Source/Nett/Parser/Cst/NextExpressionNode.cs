using System.Collections.Generic;

namespace Nett.Parser.Cst
{
    internal class NextExpressionNode : Node
    {
        public NextExpressionNode(IReq<ExpressionNode> expression, IOpt<NextExpressionNode> next)
        {
            this.Expression = expression;
            this.Next = next;
        }

        public IReq<ExpressionNode> Expression { get; }

        public IOpt<NextExpressionNode> Next { get; }

        public override IEnumerable<Node> Children
            => NonNullNodesAsEnumerable(this.Expression, this.Next);

        public override string ToString()
            => "NE";
    }
}
