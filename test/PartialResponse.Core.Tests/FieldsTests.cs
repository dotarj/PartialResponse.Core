// Copyright (c) Arjen Post. See LICENSE in the project root for license information.

using System;
using Xunit;

namespace PartialResponse.Core.Tests
{
    public class FieldsTests
    {
        [Fact]
        public void TheConstructorShouldThrowIfValueIsNull()
        {
            // Arrange
            string value = null;

            // Act
            Assert.Throws<ArgumentNullException>(() => Fields.TryParse(value, out Fields fields));
        }

        [Fact]
        public void TheValuesPropertyShouldReturnEmptyValuesIfDefault()
        {
            // Arrange
            var fields = default(Fields);

            // Assert
            Assert.Empty(fields.Values);
        }

        [Fact]
        public void TheTryParseMethodShouldSetResultToDefaultIfValueIsInvalid()
        {
            // Act
            Fields.TryParse("foo//", out Fields fields);

            // Assert
            Assert.Equal(default(Fields), fields);
        }

        [Fact]
        public void TheTryParseMethodShouldReturnFalseIfValueIsInvalid()
        {
            // Act
            var result = Fields.TryParse("foo//", out Fields fields);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TheMatchesMethodShouldThrowIfValueIsNull()
        {
            // Arrange
            string value = null;
            Fields.TryParse("foo", out Fields fields);

            // Assert
            Assert.Throws<ArgumentNullException>(() => fields.Matches(value));
        }

        [Fact]
        public void TheMatchesMethodShouldReturnFalseForDifferentValues()
        {
            // Arrange
            var value = "foo";

            // Act
            Fields.TryParse(value, out Fields fields);

            // Assert
            Assert.False(fields.Matches("bar"));
        }

        [Fact]
        public void TheMatchesMethodShouldReturnTrueForSameValues()
        {
            // Arrange
            var value = "foo";

            // Act
            Fields.TryParse(value, out Fields fields);

            // Assert
            Assert.True(fields.Matches("foo"));
        }

        [Fact]
        public void TheMatchesMethodShouldIgnoreCase()
        {
            // Arrange
            var value = "foo";

            // Act
            Fields.TryParse(value, out Fields fields);

            // Assert
            Assert.True(fields.Matches("FOO", true));
        }
    }
}