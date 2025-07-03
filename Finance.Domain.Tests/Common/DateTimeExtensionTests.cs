using Finance.Domain.Common;
using FluentAssertions;

namespace Finance.Domain.Tests.Common;

public class DateTimeExtensionTests
{
    [Fact]
    public void GetFirstDay_Should_ReturnFirstDayOfGivenMonthAndYear()
    {
        var date = new DateTime(2024, 5, 20); 
        var expected = new DateTime(2024, 5, 1);

        var result = date.GetFirstDay();

        result.Should().Be(expected);
    }

    [Fact]
    public void GetFirstDay_Should_ReturnFirstDay_WhenOverridingYearAndMonth()
    {
        var date = new DateTime(2024, 5, 20);
        var expected = new DateTime(2025, 1, 1); 

        var result = date.GetFirstDay(year: 2025, month: 1);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(2024, 1, 31)]  
    [InlineData(2023, 2, 28)]  
    [InlineData(2024, 2, 29)]  
    [InlineData(2024, 4, 30)]  
    [InlineData(2024, 12, 31)] 
    public void GetLastDay_Should_ReturnCorrectLastDayOfMonth(int year, int month, int expectedDay)
    {
        var date = new DateTime(year, month, 10);
        var expected = new DateTime(year, month, expectedDay);

        var result = date.GetLastDay();

        result.Should().Be(expected);
    }

    [Fact]
    public void GetLastDay_Should_ReturnLastDay_WhenOverridingYearAndMonth()
    {
        var date = new DateTime(2024, 1, 1);
        var expected = new DateTime(2024, 2, 29);

        var result = date.GetLastDay(year: 2024, month: 2);

        result.Should().Be(expected);
    }
}