using FluentAssertions;
using Nett.Parser;
using Nett.Parser.Ast;
using Xunit;

namespace Nett.Tests.Internal.Parser
{
    public sealed class AstParserTests
    {
        [Fact]
        public void Parse_SingleKeyValueExpression_CorrectAst()
        {
            // Act
            var parsed = Parse("x = 100");

            // Assert
            parsed.PrintTree().Trim().Should().Be(@"
S
 E
  k~x
  s~=
  V~100
".Trim());
        }

        [Fact]
        public void Parse_SingleKeyValueWithComment_CreatesCorrectAst()
        {
            // Act
            var parsed = Parse("x = 100#C");

            // Assert
            parsed.PrintTree().Trim().Should().Be(@"
S
 E
  k~x
  s~=
  V~100
  C
".Trim());
        }

        [Fact]
        public void Parse_MultipleExpressions_CreatesCorrectAst()
        {
            // Act
            var parsed = Parse(@"x = 1
y = 2");

            // Assert
            parsed.PrintTree().Trim().Should().Be(@"
S
 E
  k~x
  s~=
  V~1
 NE
  E
   k~y
   s~=
   V~2
".Trim());
        }

        [Fact]
        public void Parse_TableKey_CreatesCorrectAst()
        {
            // Act
            var parsed = Parse("[tablekey]");

            // Assert
            parsed.PrintTree().Trim().Should().Be(@"
S
 E~[tablekey]
".Trim());
        }

        [Fact]
        public void Parse_EmptyArrayValue_CreatesCorrectAst()
        {
            // Act
            var parsed = Parse("x = []");

            // Assert
            parsed.PrintTree().Trim().Should().Be(@"
S
 E
  k~x
  s~=
  A
   s~[
   s~]
".Trim());
        }

        [Fact]
        public void Parse_TwoEmptySubArrays_CreatesCorrectAst()
        {
            // Act
            var parsed = Parse("x = [[], []]");

            // Assert
            parsed.PrintTree().Trim().Should().Be(@"
S
 E
  k~x
  s~=
  A
   s~[
   AI
    A
     s~[
     s~]
    AS
     s~,
     AI
      A
       s~[
       s~]
   s~]
".Trim());
        }

        [Fact]
        public void Parse_ArrayValueWithValue_CreatesCorrectAst()
        {
            // Act
            var parsed = Parse("x = [100]");

            // Assert
            parsed.PrintTree().Trim().Should().Be(@"
S
 E
  k~x
  s~=
  A
   s~[
   AI
    V~100
   s~]
".Trim());
        }

        [Fact]
        public void Parse_ArrayWithFinalSeparator_CreatesCorrectAst()
        {
            // Act
            var parsed = Parse("x = [100,]");

            // Assert
            parsed.PrintTree().Trim().Should().Be(@"
S
 E
  k~x
  s~=
  A
   s~[
   AI
    V~100
    AS
     s~,
   s~]
".Trim());
        }

        [Fact]
        public void Parse_ArrayWithSecondValue_CreatesCorrectAst()
        {
            // Act
            var parsed = Parse("x = [100,200]");

            // Assert
            parsed.PrintTree().Trim().Should().Be(@"
S
 E
  k~x
  s~=
  A
   s~[
   AI
    V~100
    AS
     s~,
     AI
      V~200
   s~]
".Trim());
        }

        [Fact]
        public void Parse_EmptyNewlineArray_CreatesCorrectAst()
        {
            // Act
            var parsed = Parse(@"x = [
]");

            // Assert
            parsed.PrintTree().Trim().Should().Be(@"
S
 E
  k~x
  s~=
  A
   s~[
   s~]
".Trim());
        }

        [Fact]
        public void Parse_ArrayWithValuesAndNewlines_CreatesCorrectAst()
        {
            // Act
            var parsed = Parse(@"x = [
                100
,

200

]");

            // Assert
            parsed.PrintTree().Trim().Should().Be(@"
S
 E
  k~x
  s~=
  A
   s~[
   AI
    V~100
    AS
     s~,
     AI
      V~200
   s~]
".Trim());
        }

        [Fact]
        public void Parse_SingleKey_CreateSyntaxErrorNode()
        {
            // Act
            var parsed = Parse("x");

            // Assert
            parsed.PrintTree().Trim().Should().Be(@"
S
 X
".Trim());
        }

        [Fact]
        public void Parse_BrokenKeyValueExpression_CreatesAstWithSyntaxErrorNode()
        {
            // Act
            var parsed = Parse("x = ");

            // Assert
            parsed.PrintTree().Trim().Should().Be(@"
S
 E
  k~x
  s~=
  X
".Trim());
        }

        [Fact]
        public void Parse_InlineTable_ProducesCorrectAst()
        {
            // Act
            var parsed = Parse("x = {}");

            // Assert
            parsed.PrintTree().Trim().Should().Be(@"
S
 E
  k~x
  s~=
  I
   s~{
   s~}
".Trim());
        }

        private static Node Parse(string input)
        {
            var lexer = new Lexer(input);
            var tokens = lexer.Lex();
            var parser = new AstParser(new ParseInput(tokens));
            return parser.Parse().Node;
        }
    }
}
