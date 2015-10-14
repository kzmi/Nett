﻿using System.Linq;
using FluentAssertions;
using Xunit;

namespace Nett.UnitTests
{
    public class TomlTableTests
    {
        [Fact]
        public void Overwrite_WhenSourceTableHasNoComments_SourceCommentStayIntact()
        {
            var from = this.CreateEmpty();
            var to = this.CreateSingleProp("x");

            to.OverwriteCommentsWithCommentsFrom(from, overwriteWithEmpty: false);

            to.Rows.Count().Should().Be(1);
            to.Get("r1").As<TomlInt>().Value.Should().Be(1);
            to.Get("r1").Comments.Single().Text.Should().Be("x");
        }

        [Fact]
        public void OverwriteComments_WhenSourceTableHasCommentsButTargetNot_SourceTableCommentsAreUsed()
        {
            var from = CreateSingleProp("expected");
            var to = this.CreateSingleProp("willbeoverwritten");

            to.OverwriteCommentsWithCommentsFrom(from, overwriteWithEmpty: false);

            to.Rows.Count().Should().Be(1);
            to.Get("r1").As<TomlInt>().Value.Should().Be(1);
            to.Get("r1").Comments.Single().Text.Should().Be("expected");
        }

        [Fact]
        public void OverwriteComment_WithComplexTables_ProcudesCorrectResult()
        {
            var from = this.CreateComplexFrom();
            var to = this.CreateComplexTo();

            to.OverwriteCommentsWithCommentsFrom(from, overwriteWithEmpty: false);

            to.Get("I").Comments.Single().Text.Should().Be("to", "Because the source comment was empty");
            to.Get("F").Comments.Single().Text.Should().Be("from", "Because only source comment existed");
            to.Get("SubTable").Comments.Single().Text.Should().Be("from", "Because when both comments exist, source comment should overwrite target comment");
            to.Get<TomlTable>("SubTable").Get("A").Comments.Count().Should().Be(2);
            to.Get<TomlTable>("SubTable").Get("A").Comments[0].Text.Should().Be("fa1");
            to.Get<TomlTable>("SubTable").Get("A").Comments[0].Location.Should().Be(CommentLocation.Prepend);
            to.Get<TomlTable>("SubTable").Get("A").Comments[1].Text.Should().Be("fa2");
            to.Get<TomlTable>("SubTable").Get("A").Comments[1].Location.Should().Be(CommentLocation.Append);
        }

        [Fact]
        public void OverwriteComment_WithComplexTablesAndOverwriteAlwaysEnabled_ProcudesCorrectResult()
        {
            var from = this.CreateComplexFrom();
            var to = this.CreateComplexTo();

            to.OverwriteCommentsWithCommentsFrom(from, overwriteWithEmpty: true);

            to.Get("I").Comments.Count().Should().Be(0, "Because comment is always overwritten, event when empty");
            to.Get("F").Comments.Single().Text.Should().Be("from", "Because only source comment existed");
            to.Get("SubTable").Comments.Single().Text.Should().Be("from", "Because when both comments exist, source comment should overwrite target comment");
            to.Get<TomlTable>("SubTable").Get("A").Comments.Count().Should().Be(2);
            to.Get<TomlTable>("SubTable").Get("A").Comments[0].Text.Should().Be("fa1");
            to.Get<TomlTable>("SubTable").Get("A").Comments[0].Location.Should().Be(CommentLocation.Prepend);
            to.Get<TomlTable>("SubTable").Get("A").Comments[1].Text.Should().Be("fa2");
            to.Get<TomlTable>("SubTable").Get("A").Comments[1].Location.Should().Be(CommentLocation.Append);
        }

        private TomlTable CreateEmpty()
        {
            return new TomlTable();
        }

        private TomlTable CreateSingleProp()
        {
            var tt = new TomlTable();
            tt.Add("r1", new TomlInt(1));
            return tt;
        }

        private TomlTable CreateSingleProp(string comment)
        {
            var tt = new TomlTable();
            var i = new TomlInt(1);
            i.Comments.Add(new TomlComment(comment, CommentLocation.Prepend));
            tt.Add("r1", i);
            return tt;
        }

        private TomlTable CreateComplex()
        {
            var t = new TomlTable();
            var tt = new TomlTable();
            t.Add("I", new TomlInt(1));
            t.Add("F", new TomlFloat(1.0));

            tt.Add("A", new TomlArray(new TomlInt(1), new TomlInt(2), new TomlInt(3)));

            t.Add("SubTable", tt);
            return t;
        }

        private TomlTable CreateComplexFrom()
        {
            var t = this.CreateComplex();
            //t.Get("I").Comments.Add(new TomlComment("from", CommentLocation.Prepend));
            t.Get("F").Comments.Add(new TomlComment("from", CommentLocation.Prepend));
            t.Get("SubTable").Comments.Add(new TomlComment("from", CommentLocation.Prepend));

            t.Get<TomlTable>("SubTable").Get("A").Comments.Add(new TomlComment("fa1", CommentLocation.Prepend));
            t.Get<TomlTable>("SubTable").Get("A").Comments.Add(new TomlComment("fa2", CommentLocation.Append));

            return t;
        }

        private TomlTable CreateComplexTo()
        {
            var t = this.CreateComplex();
            t.Get("I").Comments.Add(new TomlComment("to", CommentLocation.Prepend));
            //t.Get("F").Comments.Add(new TomlComment("to", CommentLocation.Prepend));
            t.Get("SubTable").Comments.Add(new TomlComment("to", CommentLocation.Prepend));
            return t;
        }
    }
}
