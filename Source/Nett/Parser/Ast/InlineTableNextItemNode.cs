using System;
using System.Collections.Generic;

namespace Nett.Parser.Ast
{
    internal sealed class InlineTableNextItemNode : Node
    {
        public InlineTableNextItemNode(Token separator, IOpt<InlineTableItemNode> next)
        {
            this.Separator = new SymbolNode(separator).Req();
            this.Next = next;
        }

        public IReq<SymbolNode> Separator { get; }

        public IOpt<InlineTableItemNode> Next { get; }

        public override IEnumerable<Node> Children => throw new NotImplementedException();
    }
}
