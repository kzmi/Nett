using System;
using Nett.Extensions;

namespace Nett.Parser.Cst
{
    internal interface INode<out T>
    {
        Node Node { get; }

        T SyntaxNode { get; }

        SyntaxErrorNode Error { get; }
    }

    internal interface IOpt<out T> : INode<T>
        where T : Node
    {
        IOpt<TRes> As<TRes>()
            where TRes : Node;
    }

    internal interface IReq<out T> : INode<T>
        where T : Node
    {
        IOpt<T> AsOpt();
    }

    internal static class AstNode
    {
        public static IOpt<T> Optional<T>(T node)
            where T : Node
            => new Opt<T>(node);

        public static IOpt<T> None<T>()
            where T : Node
            => new Opt<T>(null);

        public static IReq<T> Required<T>(T node)
            where T : Node
            => new Req<T>(node);

        public static IOpt<T> Opt<T>(this T node)
            where T : Node
            => Optional(node);

        public static IReq<T> Req<T>(this T node)
            where T : Node
            => Required(node);

        public static IOpt<T> Or<T>(this IOpt<T> x, Func<IOpt<T>> y)
            where T : Node
            => x.SyntaxNode != null ? x : y();

        public static IReq<T> Orr<T>(this IOpt<T> x, Func<IReq<T>> y)
            where T : Node
            => x.Node != null ? new Req<T>(x.Node) : y();

        public static IReq<T> OrNode<T>(this IOpt<T> x, Node y)
            where T : Node
            => new Req<T>(x.SyntaxNode ?? y);
    }

    internal sealed class Req<T> : Opt<T>, IReq<T>
        where T : Node
    {
        public Req(Node node)
            : base(node)
        {
            node.CheckNotNull(nameof(node));
        }

        IOpt<T> IReq<T>.AsOpt()
            => this;
    }

    internal class Opt<T> : IOpt<T>
      where T : Node
    {
        public static readonly Opt<T> None = new Opt<T>(null);

        public Opt(Node node)
        {
            this.Error = node as SyntaxErrorNode;

            if (this.Error == null)
            {
                this.SyntaxNode = (T)node;
            }
        }

        public T SyntaxNode { get; }

        public SyntaxErrorNode Error { get; }

        public Node Node
            => (Node)this.SyntaxNode ?? this.Error;

        public IOpt<TRes> As<TRes>()
            where TRes : Node
        {
            return new Opt<TRes>((Node)this.SyntaxNode ?? this.Error);
        }

        public IReq<T> ToReq()
            => new Req<T>((Node)this.SyntaxNode ?? this.Error);
    }
}
