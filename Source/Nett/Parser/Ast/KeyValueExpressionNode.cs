using System.Collections.Generic;

namespace Nett.Parser.Ast
{
    internal sealed class KeyValueExpressionNode : ExpressionNode
    {
        public KeyValueExpressionNode(Token key, Token assignment, IReq<ValueNode> value, IOpt<CommentNode> comment)
        {
            this.Key = new KeyNode(key).Req();
            this.Assignment = new SymbolNode(assignment).Req();
            this.Value = value;
            this.Comment = comment;
        }

        public IReq<KeyNode> Key { get; }

        public IReq<SymbolNode> Assignment { get; }

        public IReq<ValueNode> Value { get; }

        public IOpt<CommentNode> Comment { get; set; }

        public override IEnumerable<Node> Children
            => NodesAsEnumerable(this.Key, this.Assignment, this.Value);
    }
}
