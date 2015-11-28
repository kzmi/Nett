﻿using System;
using FluentAssertions;
using Xunit;

namespace Nett.UnitTests
{
    public class ReadInvalidTomlTests
    {


        [Fact(DisplayName = "Reading mixed arrays should cause parse exception")]
        public void ReadToml_WhenArrayTypesMixed_ThrowsExc()
        {
            // Act
            Action a = () => Toml.ReadString(@"arrays-and-ints =  [1, [""Arrays are not integers.""]]");

            // Assert
            a.ShouldThrow<Exception>();
        }

        [Fact(DisplayName = "Reading bad byte escape should cause parse exception")]
        public void ReadToml_WithBadByteEscape_ThrowsExc()
        {
            // Act
            Action a = () => Toml.ReadString("naughty = \"\xAg\"");

            // Assert
            a.ShouldThrow<Exception>();
        }
    }
}