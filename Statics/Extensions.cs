// SPDX-License-Identifier: MIT
// Copyright (c) 2025. Csaba Dudas (CsabaDu)

namespace CsabaDu.DynamicTestData.Core.NUnit.Statics;

public static class Extensions
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
        var convertedTestData = testData.ToParams(
            argsCode,
            PropsCode.Returns,
            out string testCaseName);
        var displayName = GetDisplayName(
            testMethodName,
            testCaseName);
        var testDataType = typeof(TTestData);
        var testCaseData = new TestCaseData(convertedTestData)
            .SetDescription(testCaseName)
            .SetName(displayName);
        var returns = testData as IReturns;
        bool isReturns = returns is not null;
        testCaseData.TypeArgs = argsCode switch
        {
            ArgsCode.Instance => [testDataType],
            ArgsCode.Properties => getGenericArgs(),
            _ => null,
        };

        return isReturns ?
            testCaseData.Returns(returns!.GetExpected())
            : testCaseData;

        #region Local methods
        Type[] getGenericArgs()
        {
            var genericArgs =
                testDataType.GetGenericArguments();

            return isReturns ?
                genericArgs[1..]
                : genericArgs;
        }
        #endregion
    }
}
