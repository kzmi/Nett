using System.Collections.Generic;
using Nett.Parser.Cst;

namespace Nett.Parser
{
    internal sealed class AstParser
    {
        private readonly MultiParseInput input;

        public AstParser(IParseInput input)
        {
            this.input = new MultiParseInput(input, new SkippingParseInput(input, toSkip: TokenType.NewLine));
        }

        public IOpt<StartNode> Parse()
        {
            this.input.AcceptNewLines();

            if (this.input.IsFinished) { return Opt<StartNode>.None; }

            var expressions = new List<IReq<ExpressionNode>>();

            var exp = this.Expression();
            var next = this.NextExpression();

            return new StartNode(exp, next).Opt();
        }

        private static bool IsKey(Token t)
            => t.type == TokenType.Key || t.type == TokenType.BareKey;

        private IReq<ExpressionNode> Expression()
        {
            return Comment().As<ExpressionNode>()
                .Or(KeyValueExpression)
                .Or(Table)
                .OrNode(this.input.CreateErrorNode());

            IOpt<CommentExpressionNode> Comment()
                => this.input
                    .Accept(t => t.type == TokenType.Comment)
                    .CreateNode(t => new CommentExpressionNode(t).Opt());

            IOpt<KeyValueExpressionNode> KeyValueExpression()
            {
                var key = this.input.Accept(IsKey)
                    .CreateNode(t => new KeyNode(t, this.KeySeparator()).Opt());
                if (key.SyntaxNode != null)
                {
                    return this.input.Expect(t => t.type == TokenType.Assign)
                        .CreateNode(a => new KeyValueExpressionNode(key.SyntaxNode.Req(), a, this.Value(), this.Comment()).Opt());
                }
                else
                {
                    return Opt<KeyValueExpressionNode>.None;
                }
            }

            IOpt<TableNode> Table()
                => this.input
                    .Accept(t => t.type == TokenType.LBrac)
                    .CreateNode(lbrac => this.Table(lbrac).AsOpt());
        }

        private IReq<TableNode> Table(Token lbrac)
        {
            var tbl = this.TableArray()
                .Orr<Node>(this.StandardTable);

            return this.input.Expect(t => t.type == TokenType.RBrac)
                .CreateNode(rbrac => new TableNode(lbrac, tbl, rbrac).Req());
        }

        private IReq<StandardTableNode> StandardTable()
            => new StandardTableNode(this.Key()).Req();

        private IOpt<TableArrayNode> TableArray()
        {
            return this.input.Accept(t => t.type == TokenType.LBrac)
                .CreateNode(lb => CreateNode(lb));

            IOpt<TableArrayNode> CreateNode(Token lbrac)
            {
                var key = this.Key();
                return this.input.Expect(t => t.type == TokenType.RBrac)
                    .CreateNode(rb => new TableArrayNode(lbrac, key, rb).Opt());
            }
        }

        private IReq<KeyNode> Key()
            => this.input
                .Expect(IsKey)
                .CreateNode(t => new KeyNode(t, this.KeySeparator()).Req());

        private IOpt<KeySeparatorNode> KeySeparator()
        {
            if (Epsilon()) { return Opt<KeySeparatorNode>.None; }

            return this.input.Expect(t => t.type == TokenType.Dot)
                .CreateNode(t => new KeySeparatorNode(t, this.Key()).Opt());

            bool Epsilon()
                => this.input.Peek(t => t.type == TokenType.RBrac || t.type == TokenType.Assign);
        }

        private IOpt<NextExpressionNode> NextExpression()
        {
            if (this.input.AcceptNewLines() && !this.input.IsFinished)
            {
                return new NextExpressionNode(this.Expression(), this.NextExpression()).Opt();
            }

            return Opt<NextExpressionNode>.None;
        }

        private IReq<ValueNode> Value()
        {
            return SimpleValue()
                .Or(Array)
                .Or(InlineTable)
                .OrNode(this.input.CreateErrorNode());

            IOpt<ValueNode> SimpleValue()
                => this.input
                    .Accept(t => t.type == TokenType.Float
                        || t.type == TokenType.Integer
                        || t.type == TokenType.Bool
                        || t.type == TokenType.Date
                        || t.type == TokenType.DateTime
                        || t.type == TokenType.Duration
                        || t.type == TokenType.String
                        || t.type == TokenType.LiteralString
                        || t.type == TokenType.MultilineString
                        || t.type == TokenType.MultilineLiteralString)
                    .CreateNode(t => ValueNode.CreateTerminalValue(t).Opt());

            IOpt<ValueNode> Array()
                => this.input
                    .Accept(t => t.type == TokenType.LBrac)
                    .CreateNode(t => ValueNode.CreateNonTerminalValue(this.Array(t)).Opt());

            IOpt<ValueNode> InlineTable()
                => this.input
                    .Accept(t => t.type == TokenType.LCurly)
                    .CreateNode(t => ValueNode.CreateNonTerminalValue(this.InlineTable(t)).Opt());
        }

        private IOpt<TerminalNode> Comment()
            => this.input.Accept(t => t.type == TokenType.Comment)
                .CreateNode(t => new TerminalNode(t).Opt());

        private IReq<ArrayNode> Array(Token lbrac)
        {
            using (this.input.UseIgnorewNewlinesInput())
            {
                var item = this.ArrayItem();
                return this.input.Expect(t => t.type == TokenType.RBrac)
                    .CreateNode(t => ArrayNode.Create(lbrac, t, item).Req());
            }
        }

        private IOpt<ArrayItemNode> ArrayItem()
        {
            if (Epsilon()) { return Opt<ArrayItemNode>.None; }

            var value = this.Value();
            var sep = this.ArraySeparator();

            return new ArrayItemNode(value, sep).Opt();

            bool Epsilon()
                => this.input.Peek(t => t.type == TokenType.RBrac);
        }

        private IOpt<ArraySeparatorNode> ArraySeparator()
        {
            if (Epsilon()) { return Opt<ArraySeparatorNode>.None; }

            return this.input.Expect(t => t.type == TokenType.Comma)
                .CreateNode(t => new ArraySeparatorNode(t, this.ArrayItem()).Opt());

            bool Epsilon()
                => this.input.Peek(t => t.type == TokenType.RBrac);
        }

        private IReq<InlineTableNode> InlineTable(Token lcurly)
        {
            var item = this.InlineTableItem();

            return this.input.Expect(t => t.type == TokenType.RCurly)
                .CreateNode(t => new InlineTableNode(lcurly, t, item).Req());
        }

        private IOpt<InlineTableItemNode> InlineTableItem()
        {
            if (Epsilon()) { return Opt<InlineTableItemNode>.None; }

            var key = this.Key();
            var kve = this.input
                .Expect(t => t.type == TokenType.Assign)
                .CreateNode(a => new KeyValueExpressionNode(key, a, this.Value(), Opt<TerminalNode>.None).Req());

            return new InlineTableItemNode(kve, this.NextInlineTableItem()).Opt();

            bool Epsilon()
                => this.input.Peek(t => t.type == TokenType.RCurly);
        }

        private IOpt<InlineTableNextItemNode> NextInlineTableItem()
        {
            if (Epsilon()) { return Opt<InlineTableNextItemNode>.None; }

            return this.input.Expect(t => t.type == TokenType.Comma)
                .CreateNode(s => new InlineTableNextItemNode(s, this.InlineTableItem()).Opt());

            bool Epsilon()
                => this.input.Peek(t => t.type == TokenType.RCurly);
        }

        private void Epsilon()
        {
            // Readability method
        }
    }
}
