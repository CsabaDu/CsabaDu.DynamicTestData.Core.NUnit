// SPDX-License-Identifier: MIT
// Copyright (c) 2025. Csaba Dudas (CsabaDu)

using static CsabaDu.DynamicTestData.Core.NUnit.TestDataTypes.TestCaseTestData;

namespace CsabaDu.DynamicTestData.Core.NUnit.TestDataTypes.Converters;

public static class TestDataConverter
{
    public static TestCaseTestData<TTestData> ToTestCaseTestData<TTestData>(
        this TTestData testData,
        ArgsCode argsCode,
        string? testMethodName = null)
    where TTestData : notnull, ITestData
    => new(testData, argsCode, testMethodName);

    public static TestCaseData ToTestCaseData<TTestData>(
        this TTestData testData,
        ArgsCode argsCode,
        string? testMethodName)
    where TTestData : notnull, ITestData
    {
        var row = ConvertToReturnsParams(testData, argsCode);
        var testCaseData = new TestCaseData(row)
        {
            TypeArgs = testData.GetTypeArgs(argsCode),
        }
        .SetDescription(testData.TestCaseName);

        if (!string.IsNullOrEmpty(testMethodName))
        {
            var displayName = testData.GetDisplayName(testMethodName);
            testCaseData = testCaseData.SetName(displayName);
        }

        return testData is IReturns returns ?
            testCaseData.Returns(returns.GetExpected())
            : testCaseData;
    }
}
