using System;
using Nett.Parser.Cst;

namespace Nett.Parser
{
    internal interface IParseInput
    {
        bool IsFinished { get; }

        SyntaxErrorNode CreateErrorNode();

        Token Advance();

        bool Peek(Func<Token, bool> predicate);
    }

    internal static class ParseInputExtensions
    {
        public static IProduction1 Accept(this IParseInput input, Func<Token, bool> predicate)
        {
            IProduction production = new Production(input);
            return production.Accept(predicate);
        }

        public static IProduction1 Expect(this IParseInput input, Func<Token, bool> predicate)
        {
            IProduction production = new Production(input);
            return production.Expect(predicate);
        }

        public static bool AcceptNewLines(this IParseInput input)
        {
            int accpted = 0;
            while (input.Peek(t => t.type == TokenType.NewLine))
            {
                ++accpted;
                input.Advance();
            }

            return accpted > 0;
        }
    }
}