﻿using Nett.Extensions;

namespace Nett.Parser.Ast
{
    internal interface INode<out T>
    {
        T Node { get; }

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

        public static IOpt<T> Or<T>(this IOpt<T> x, IOpt<T> y)
            where T : Node
            => x.Node != null ? x : y;

        public static IReq<T> Or<T>(this IOpt<T> x, Node y)
            where T : Node
            => new Req<T>(x.Node ?? y);
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

            if (this.Error != null)
            {
                this.Node = (T)node;
            }
        }

        public T Node { get; }

        public SyntaxErrorNode Error { get; }

        public static implicit operator Node(Opt<T> parsed)
            => (Node)parsed.Node ?? parsed.Error;

        public IOpt<TRes> As<TRes>()
            where TRes : Node
        {
            return new Opt<TRes>((Node)this.Node ?? this.Error);
        }
    }
}
