using System.Collections.Generic;

namespace Nett.Parser.Ast
{
    internal sealed class InlineTableNode : ValueNode
    {
        public InlineTableNode(Token lcurly, Token rcurly, IOpt<InlineTableItemNode> first)
        {
            this.LCurly = new SymbolNode(lcurly).Req();
            this.RCurly = new SymbolNode(rcurly).Req();
            this.First = first;
        }

        public IReq<SymbolNode> LCurly { get; }

        public IReq<SymbolNode> RCurly { get; }

        public IOpt<InlineTableItemNode> First { get; }

        public override IEnumerable<Node> Children
            => NonNullNodesAsEnumerable(this.LCurly, this.First, this.RCurly);

        public override string ToString()
            => "I";
    }
}
