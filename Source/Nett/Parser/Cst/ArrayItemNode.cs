using System.Collections.Generic;

namespace Nett.Parser.Cst
{
    internal sealed class ArrayItemNode : Node
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
