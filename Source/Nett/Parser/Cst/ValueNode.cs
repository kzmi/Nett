using System.Collections.Generic;

namespace Nett.Parser.Cst
{
    internal class ValueNode : Node
    {
        private ValueNode(IReq<Node> value)
        {
            this.Value = value;
        }

        public IReq<Node> Value { get; }

        public static ValueNode CreateTerminalValue(Token value)
            => new ValueNode(new TerminalNode(value).Req());

        public static ValueNode CreateNonTerminalValue(IReq<Node> value)
            => new ValueNode(value);

        public override IEnumerable<Node> Children
            => NodesAsEnumerable(this.Value);

        public override string ToString()
            => "V";
    }
}
