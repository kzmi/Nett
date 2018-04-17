using System.Collections.Generic;
using Nett.Parser.Ast;

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
            if (this.input.IsFinished) { return Opt<StartNode>.None; }

            var expressions = new List<IReq<ExpressionNode>>();

            var exp = this.Expression();
            var next = this.NextExpression();

            return new StartNode(exp, next).Opt();
        }

        private IReq<ExpressionNode> Expression()
        {
            return Comment().As<ExpressionNode>()
                .Or(KeyValueExpression())
                .Or(Table())
                .Or(this.input.CreateErrorNode());

            IOpt<CommentNode> Comment()
                => this.input
                    .Accept(t => t.type == TokenType.Comment)
                    .CreateNode(t => new CommentNode(t).Opt());

            IOpt<KeyValueExpressionNode> KeyValueExpression()
                => this.input
                    .Accept(t => t.type == TokenType.BareKey)
                    .Expect(t => t.type == TokenType.Assign)
                    .CreateNode((k, a) => new KeyValueExpressionNode(k, a, this.Value(), this.Comment()).Opt());

            IOpt<TableNode> Table()
                => this.input
                    .Accept(t => t.type == TokenType.LBrac)
                    .Expect(t => t.type == TokenType.BareKey)
                    .Expect(t => t.type == TokenType.RBrac)
                    .CreateNode((_, k, __) => new TableNode(k).Opt());
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
            return IntValue().As<ValueNode>()
                .Or(FloatValue())
                .Or(Array())
                .Or(InlineTable())
                .Or(this.input.CreateErrorNode());

            IOpt<FloatValueNode> FloatValue()
                => this.input
                    .Accept(t => t.type == TokenType.Float)
                    .CreateNode(t => new FloatValueNode(t).Opt());

            IOpt<IntValueNode> IntValue()
                => this.input
                    .Accept(t => t.type == TokenType.Integer)
                    .CreateNode(t => new IntValueNode(t).Opt());

            IOpt<ArrayNode> Array()
                => this.input
                    .Accept(t => t.type == TokenType.LBrac)
                    .CreateNode(t => this.Array(t).AsOpt());

            IOpt<InlineTableNode> InlineTable()
                => this.input
                    .Accept(t => t.type == TokenType.LCurly)
                    .CreateNode(t => this.InlineTable(t).AsOpt());
        }

        private IOpt<CommentNode> Comment()
            => this.input.Accept(t => t.type == TokenType.Comment)
                .CreateNode(t => new CommentNode(t).Opt());

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

            var kve = this.input.Expect(t => t.type == TokenType.Key)
                .Expect(t => t.type == TokenType.Assign)
                .CreateNode((k, a) => new KeyValueExpressionNode(k, a, this.Value(), Opt<CommentNode>.None).Req());

            var next = this.NextInlineTableItem();

            return new InlineTableItemNode(kve, next).Opt();

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
