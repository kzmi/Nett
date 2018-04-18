using System.Collections.Generic;
using System.Linq;

namespace Nett.Parser.Cst
{
    internal sealed class SyntaxErrorNode : Node
    {
        public SyntaxErrorNode(string error, int line, int column)
        {
            this.Message = error;
            this.Line = line;
            this.Column = column;
        }

        public int Line { get; }

        public int Column { get; }

        public string Message { get; }

        public override IEnumerable<Node> Children
            => Enumerable.Empty<Node>();

        public static SyntaxErrorNode Unexpected(Token unexpected)
        {
            return new SyntaxErrorNode(
                $"Encountered unexpected token of type '{unexpected.type}' with value '{unexpected.value}'",
                unexpected.line,
                unexpected.col);
        }

        public override string ToString()
            => "X";
    }
}
