// Copyright (c) Arjen Post. See LICENSE in the project root for license information.

using System;
using System.IO;
using Xunit;

namespace PartialResponse.Core.Tests
{
    public class ParserContextTests
    {
        [Fact]
        public void TheConstructorShouldThrowIfSourceIsNull()
        {
            // Arrange
            TextReader source = null;

            // Act
            Assert.Throws<ArgumentNullException>(() => new ParserContext(source));
        }
    }
}
