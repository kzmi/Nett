using System.Collections.Generic;

namespace Nett.Parser.Cst
{
    internal sealed class ArrayNode : Node
    {
        private ArrayNode(IReq<TerminalNode> lbrac, IReq<TerminalNode> rbrac)
            : this(lbrac, rbrac, AstNode.None<ArrayItemNode>())
        {
        }

        private ArrayNode(IReq<TerminalNode> lbrac, IReq<TerminalNode> rbrac, IOpt<ArrayItemNode> item)
        {
            this.LBrac = lbrac;
            this.RBrac = rbrac;
            this.Item = item;
        }

        public IReq<TerminalNode> LBrac { get; }

        public IReq<TerminalNode> RBrac { get; }

        public IOpt<ArrayItemNode> Item { get; }

        public override IEnumerable<Node> Children
            => NonNullNodesAsEnumerable(this.LBrac, this.Item, this.RBrac);

        public static ArrayNode Empty(Token lbrac, Token rbrac)
            => new ArrayNode(AstNode.Required(new TerminalNode(lbrac)), AstNode.Required(new TerminalNode(rbrac)));

        public static ArrayNode Create(Token lbrac, Token rbrac, IOpt<ArrayItemNode> item)
            => new ArrayNode(new TerminalNode(lbrac).Req(), new TerminalNode(rbrac).Req(), item);

        public override string ToString()
            => "A";
    }
}
