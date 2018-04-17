using System.Collections.Generic;

namespace Nett.Parser.Ast
{
    internal sealed class ArrayItemNode : ValueNode
    {
        public ArrayItemNode(IReq<ValueNode> value, IOpt<ArraySeparatorNode> separator)
        {
            this.Value = value;
            this.Separator = separator;
        }

        public IReq<ValueNode> Value { get; }

        public IOpt<ArraySeparatorNode> Separator { get; }

        public override IEnumerable<Node> Children
            => NonNullNodesAsEnumerable(this.Value, this.Separator);

        public override string ToString()
            => "AI";
    }
}
