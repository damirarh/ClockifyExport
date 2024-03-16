using ClockifyExport.Cli.Validation;
using FluentAssertions;

namespace ClockifyExport.Tests.Validation;

public class FactorOfAttributeTests
{
    [Test]
    [TestCase(1, 60)]
    [TestCase(5, 60)]
    [TestCase(15, 60)]
    [TestCase(30, 60)]
    [TestCase(60, 60)]
    [TestCase(30, 30)]
    public void IsValidForFactorsOfN(int value, int n)
    {
        var attribute = new FactorOfAttribute(n);
        attribute.IsValid(value).Should().BeTrue();
    }

    [Test]
    [TestCase(-1, 60)]
    [TestCase(0, 60)]
    [TestCase(16, 60)]
    [TestCase(61, 60)]
    [TestCase(120, 60)]
    [TestCase(60, 30)]
    public void IsNotValidForValuesNotFactorsOfN(int value, int n)
    {
        var attribute = new FactorOfAttribute(n);
        attribute.IsValid(value).Should().BeFalse();
    }

    [Test]
    [TestCase(null, 60)]
    [TestCase("non-int", 60)]
    public void IsNotValidForNotIntValues(object? value, int n)
    {
        var attribute = new FactorOfAttribute(n);
        attribute.IsValid(value).Should().BeFalse();
    }

    [Test]
    [TestCase(60, "--round-up-to")]
    [TestCase(30, "--round-up")]
    public void FormatsErrorMessage(int n, string name)
    {
        var attribute = new FactorOfAttribute(n);
        var message = attribute.FormatErrorMessage(name);

        message.Should().Be($"The {name} field must be a factor of {n}.");
    }

    [Test]
    public void NPropertyReturnsNParameterValue()
    {
        var n = 60;
        var attribute = new FactorOfAttribute(n);

        attribute.N.Should().Be(n);
    }
}
