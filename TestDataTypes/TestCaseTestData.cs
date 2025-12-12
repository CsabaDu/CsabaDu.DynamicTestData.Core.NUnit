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
    /// <summary>
    /// Initializes a new instance of the <see cref="TestCaseTestData"/> class.
    /// <remark>
    /// The constructor sets the <see cref="TestParameters.Arguments"/> property.
    /// and the value of the <see cref="TestParameters.Description"/> key
    /// of the <see cref="TestParameters.Properties"/> dictionary.
    /// </remark>
    /// </summary>
    /// <param name="testData">The <see cref="TestData"/> instance having the necessary test parameters.</param>
    /// <param name="argsCode">The <see cref="ArgsCode"/> enum to determine the conversion method.</param>
    /// <param name="testMethodName">The name of the test method which proceeds the TestCaseData.</param>
    private protected TestCaseTestData(
        ITestData testData,
        ArgsCode argsCode,
        string? testMethodName)
    : base(testData.ToParams(
        argsCode,
        PropsCode.Returns,
        out string testCaseName))
    {
        Properties.Set(PropertyNames.Description, testCaseName);

        if (!string.IsNullOrEmpty(testMethodName))
        {
            TestName = CreateDisplayName(testMethodName, testCaseName);
        }

        if (testData is IReturns returns)
        {
            ExpectedResult = returns.GetExpected();
        }
    }

    public static Type GetTestDataType<TTestData>(
        bool isReturns,
        out Type[] genericArgs)
    where TTestData : notnull, ITestData
    {
        Type testDataType = typeof(TTestData);
        genericArgs = testDataType.GetGenericArguments();

        genericArgs = isReturns ?
            genericArgs[1..]
            : genericArgs;

        return testDataType;
    }
}

/// <summary>
/// Represents test case data for a specific test, parameterized by a type that implements <see cref="ITestData"/>.
/// </summary>
/// <remarks>This class encapsulates the test data, argument code, and optional test method name for a test case.
/// It also determines the type arguments based on the provided <typeparamref name="TTestData"/> and the argument
/// code.</remarks>
/// <typeparam name="TTestData">The type of the test data, which must implement <see cref="ITestData"/>.</typeparam>
public sealed class TestCaseTestData<TTestData>
: TestCaseTestData
where TTestData : notnull, ITestData
{
    public TestCaseTestData(
        TTestData testData,
        ArgsCode argsCode,
        string? testMethodName)
    : base(
        testData,
        argsCode,
        testMethodName)
    {
        Type testDataType = GetTestDataType<TTestData>(
            HasExpectedResult,
            out Type[] genericArgs);

        TypeArgs = argsCode switch
        {
            ArgsCode.Instance => [testDataType],
            ArgsCode.Properties => genericArgs,
            _ => null,
        };
    }
}