using System.Collections.Generic;

namespace Nett.Parser.Cst
{
    internal sealed class ArraySeparatorNode : Node
    {
        public ArraySeparatorNode(Token symbol, IOpt<ArrayItemNode> nextItem)
        {
            this.Seprator = new TerminalNode(symbol).Req();
            this.NextItem = nextItem;
        }

        public IReq<TerminalNode> Seprator { get; }

        public IOpt<ArrayItemNode> NextItem { get; }

        public override IEnumerable<Node> Children
            => NonNullNodesAsEnumerable(this.Seprator, this.NextItem);

        public override string ToString()
            => "AS";
    }
}
