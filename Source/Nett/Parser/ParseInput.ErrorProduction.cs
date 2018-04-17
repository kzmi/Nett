using System;
using Nett.Parser.Ast;

namespace Nett.Parser
{
    internal sealed class ErrorProduction : IProduction1, IProduction2, IProduction3
    {
        private readonly SyntaxErrorNode error;

        public ErrorProduction(SyntaxErrorNode error)
        {
            this.error = error;
        }

        IProduction2 IProduction1.Accept(Func<Token, bool> predicate)
            => this;

        IProduction3 IProduction2.Accept(Func<Token, bool> predicate)
            => this;

        IReq<T> IProduction1.CreateNode<T>(Func<Token, IReq<T>> onSuccess, Func<SyntaxErrorNode> onError)
            => new Req<T>(this.error);

        IOpt<T> IProduction1.CreateNode<T>(Func<Token, IOpt<T>> onSuccess, Func<SyntaxErrorNode> onError)
            => new Opt<T>(this.error);

        IReq<T> IProduction2.CreateNode<T>(Func<Token, Token, IReq<T>> onSuccess, Func<SyntaxErrorNode> onError)
            => new Req<T>(this.error);

        IOpt<T> IProduction2.CreateNode<T>(Func<Token, Token, IOpt<T>> onSuccess, Func<SyntaxErrorNode> onError)
            => new Opt<T>(this.error);

        IReq<T> IProduction3.CreateNode<T>(Func<Token, Token, Token, IReq<T>> onSuccess, Func<SyntaxErrorNode> onError)
            => new Req<T>(this.error);

        IOpt<T> IProduction3.CreateNode<T>(Func<Token, Token, Token, IOpt<T>> onSuccess, Func<SyntaxErrorNode> onError)
            => new Req<T>(this.error);

        IProduction2 IProduction1.Expect(Func<Token, bool> predicate)
            => this;

        IProduction3 IProduction2.Expect(Func<Token, bool> predicate)
            => this;
    }
}
