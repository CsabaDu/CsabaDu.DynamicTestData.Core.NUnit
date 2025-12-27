// SPDX-License-Identifier: MIT
// Copyright (c) 2025. Csaba Dudas (CsabaDu)

namespace CsabaDu.DynamicTestData.Core.NUnit.TestDataTypes;

/// <summary>
/// Represents a test case data class for NUnit.
/// It inherits from <see cref="TestCaseData"/>
/// </summary>
public abstract class TestCaseTestData
: TestCaseData
{
    private protected TestCaseTestData(object?[] args)
    : base(args)
    {
    }

    public static Type[]? GetTypeArgs<TTestData>(
        TTestData testData,
        ArgsCode argsCode)
    where TTestData : notnull, ITestData
    {
        var testDataType = typeof(TTestData);
        var typeArgs = testDataType.GetGenericArguments();

        typeArgs = testData is IReturns ?
            typeArgs[1..]
            : typeArgs;

        return argsCode == ArgsCode.Properties ?
            typeArgs
            : null;
    }


    public static object?[] ConvertToReturnsParams(
        ITestData testData,
        ArgsCode argsCode)
    => testData.ToParams(argsCode, PropsCode.Returns);
}

/// <summary>
/// Represents test case data for a specific test, parameterized by a type that implements <see cref="ITestData"/>.
/// </summary>
/// <remarks>This class encapsulates the test data, argument code, and optional test method name for a test case.
/// It also determines the type arguments based on the provided <typeparamref name="TTestData"/> and the argument
/// code.</remarks>
/// <typeparam name="TTestData">The type of the test data, which must implement <see cref="ITestData"/>.</typeparam>
public sealed class TestCaseTestData<TTestData>
: TestCaseTestData, INamedTestCase
where TTestData : notnull, ITestData
{
    public TestCaseTestData(
        TTestData testData,
        ArgsCode argsCode,
        string? testMethodName)
    : base(ConvertToReturnsParams(testData, argsCode))
    {
        TestCaseName = testData.TestCaseName;
        TypeArgs = GetTypeArgs(testData, argsCode);
        Properties.Set(PropertyNames.Description, TestCaseName);

        if (!string.IsNullOrEmpty(testMethodName))
        {
            TestName = GetDisplayName(testMethodName);
        }

        if (testData is IReturns returns)
        {
            ExpectedResult = returns.GetExpected();
        }
    }

    public string TestCaseName { get; init; }

    public bool ContainedBy(IEnumerable<INamedTestCase>? namedTestCases)
    => namedTestCases?.Any(Equals) == true;

    public override bool Equals(object? obj)
    => Equals(obj as INamedTestCase);

    public bool Equals(INamedTestCase? other)
    => TestCaseName == other?.TestCaseName;

    public string? GetDisplayName(string? testMethodName)
    => NamedTestCase.CreateDisplayName(testMethodName, TestCaseName);

    public override int GetHashCode()
    => TestCaseName.GetHashCode();
}