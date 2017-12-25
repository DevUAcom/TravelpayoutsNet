using System;
using System.Collections.Specialized;
using System.Web;
using DevUa.TravelpayoutsNet.TicketsCacheApi.Helpers;
using Ploeh.AutoFixture;
using Shouldly;
using Xunit;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Test
{
    public class NameValueCollectionHelperTests
    {
        private readonly NameValueCollection _query;

        public NameValueCollectionHelperTests()
        {
            _query = HttpUtility.ParseQueryString(String.Empty);
        }

        [Fact]
        public void ShouldAddStringValue()
        {
            // Arrange
            Fixture fixture = new Fixture();
            string name = fixture.Create<string>();
            string value = fixture.Create<string>();

            // Act
            _query.AddValueIfNotNull(name, value);

            // Assert
            _query.ToString().ShouldBe($"{name}={value}");
        }
        
        [Fact]
        public void ShouldNotAddNullStringValue()
        {
            // Arrange
            Fixture fixture = new Fixture();
            string name = fixture.Create<string>();
            string value = null;

            // Act
            _query.AddValueIfNotNull(name, value);

            // Assert
            _query.ToString().ShouldBe("");
        }
        
        [Fact]
        public void ShouldAddIntValue()
        {
            // Arrange
            Fixture fixture = new Fixture();
            string name = fixture.Create<string>();
            int? value = fixture.Create<int?>();

            // Act
            _query.AddValueIfNotNull(name, value);

            // Assert
            _query.ToString().ShouldBe($"{name}={value}");
        }
        
        [Fact]
        public void ShouldNotAddNullIntValue()
        {
            // Arrange
            Fixture fixture = new Fixture();
            string name = fixture.Create<string>();

            // Act
            _query.AddValueIfNotNull(name, (int?) null);

            // Assert
            _query.ToString().ShouldBe("");
        }

        [Fact]
        public void ShouldAddSeveralValues()
        {
            // Arrange
            Fixture fixture = new Fixture();
            string name1 = fixture.Create<string>();
            int? value1 = fixture.Create<int?>();
            string name2 = fixture.Create<string>();
            string value2 = fixture.Create<string>();

            // Act
            _query.AddValueIfNotNull(name1, value1);
            _query.AddValueIfNotNull(name2, value2);

            // Assert
            _query.ToString().ShouldBe($"{name1}={value1}&{name2}={value2}");

        }

    }
}
