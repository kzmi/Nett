using System.Collections.Generic;

namespace Nett.Parser.Cst
{
    internal sealed class KeyValueExpressionNode : ExpressionNode
    {
        public KeyValueExpressionNode(IReq<KeyNode> key, Token assignment, IReq<ValueNode> value, IOpt<TerminalNode> comment)
        {
            this.Key = key;
            this.Assignment = new TerminalNode(assignment).Req();
            this.Value = value;
            this.Comment = comment;
        }

        public IReq<KeyNode> Key { get; }

        public IReq<TerminalNode> Assignment { get; }

        public IReq<ValueNode> Value { get; }

        public IOpt<TerminalNode> Comment { get; set; }

        public override IEnumerable<Node> Children
            => NonNullNodesAsEnumerable(this.Key, this.Assignment, this.Value, this.Comment);
    }
}
