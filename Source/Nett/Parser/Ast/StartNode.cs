namespace Nett.Parser.Ast
{
    internal sealed class StartNode : NextExpressionNode
    {
        public StartNode(IReq<ExpressionNode> expression, IOpt<NextExpressionNode> next)
            : base(expression, next)
        {
        }

        public override string ToString()
            => "S";
    }
}
