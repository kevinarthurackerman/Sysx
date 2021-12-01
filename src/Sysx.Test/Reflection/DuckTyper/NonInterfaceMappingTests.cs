﻿using System;

namespace Sysx.Test.Reflection.DuckTyper
{
    using Xunit;

    public class NonInterfaceMappingTests
    {
        [Fact]
        public void Should_Not_Wrap_Property_To_Missing_Field_Or_Property()
        {
            var value = new Duck();
            var success = Sysx.Reflection.DuckTyper.TryWrap<AlsoDuck>(value, out var result);

            Assert.False(success);
            Assert.Null(result);

            var exception = Assert.Throws<ArgumentException>(() => Sysx.Reflection.DuckTyper.Wrap<AlsoDuck>(value));

#if NET48
            var expectedExceptionMessage = "TWithInterface must be an interface type.\r\nParameter name: TWithInterface";
#endif

#if NET5_0 || NETCOREAPP3_1
            var expectedExceptionMessage = "TWithInterface must be an interface type. (Parameter 'TWithInterface')";
#endif

            Assert.Equal(expectedExceptionMessage, exception.Message);

        }

        public class AlsoDuck
        {
            public string? Quack { get; set; }
        }

        public class Duck
        {
            public string? Quack { get; set; }
        }
    }
}