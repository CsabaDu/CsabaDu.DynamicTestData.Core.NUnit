// SPDX-License-Identifier: MIT
// Copyright (c) 2025. Csaba Dudas (CsabaDu)

using CsabaDu.DynamicTestData.Core.NUnit.TestDataTypes;
using static CsabaDu.DynamicTestData.Core.NUnit.TestDataTypes.TestCaseTestData;

namespace CsabaDu.DynamicTestData.Core.NUnit.Extensions;

public static class TestDataExtensions
{
    public static TestCaseTestData<TTestData> ToTestCaseTestData<TTestData>(
        this TTestData testData,
        ArgsCode argsCode,
        string? testMethodName = null)
    where TTestData : notnull, ITestData
    => new(
        testData,
        argsCode,
        testMethodName);

    public static TestCaseData ToTestCaseData<TTestData>(
        this TTestData testData,
        ArgsCode argsCode,
        string? testMethodName)
    where TTestData : notnull, ITestData
    {
        var row = testData.ToParams(
            argsCode,
            PropsCode.Returns,
            out string testCaseName);
        var displayName = CreateDisplayName(
            testMethodName,
            testCaseName);
        var testCaseData = new TestCaseData(row)
            .SetDescription(testCaseName)
            .SetName(displayName);
        var isReturns = IsReturns(
            testData,
            out IReturns? returns);
        var testDataType = GetTestDataType<TTestData>(
            isReturns,
            out Type[] genericArgs);
        testCaseData.TypeArgs = argsCode switch
        {
            ArgsCode.Instance => [testDataType],
            ArgsCode.Properties => genericArgs,
            _ => null,
        };

        return isReturns ?
            testCaseData.Returns(returns!.GetExpected())
            : testCaseData;
    }
}
