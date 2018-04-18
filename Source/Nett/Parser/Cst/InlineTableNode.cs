using System.Collections.Generic;

namespace Nett.Parser.Cst
{
    internal sealed class InlineTableNode : Node
    {
        public InlineTableNode(Token lcurly, Token rcurly, IOpt<InlineTableItemNode> first)
        {
            this.LCurly = new TerminalNode(lcurly).Req();
            this.RCurly = new TerminalNode(rcurly).Req();
            this.First = first;
        }

        public IReq<TerminalNode> LCurly { get; }

        public IReq<TerminalNode> RCurly { get; }

        public IOpt<InlineTableItemNode> First { get; }

        public override IEnumerable<Node> Children
            => NonNullNodesAsEnumerable(this.LCurly, this.First, this.RCurly);

        public override string ToString()
            => "I";
    }
}
