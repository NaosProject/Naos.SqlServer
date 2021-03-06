﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlParameterRepresentationExtensions.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.SqlServer.Protocol.Client
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using Naos.SqlServer.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Database.Recipes;
    using OBeautifulCode.Reflection.Recipes;
    using OBeautifulCode.Type.Recipes;

    /// <summary>
    /// Top level .
    /// </summary>
    public static class SqlParameterRepresentationExtensions
    {
        /// <summary>
        /// Converts to a <see cref="SqlParameter"/>.
        /// </summary>
        /// <param name="parameterRepresentation">The parameter representation.</param>
        /// <returns>SqlParameter.</returns>
        public static SqlParameter ToSqlParameter(
            this SqlParameterRepresentationBase parameterRepresentation)
        {
            parameterRepresentation.MustForArg(nameof(parameterRepresentation)).NotBeNull();

            if (parameterRepresentation is SqlInputParameterRepresentation<byte[]> binaryRepresentationInput)
            {
                return binaryRepresentationInput.ToSqlParameter();
            }
            else if (parameterRepresentation is SqlInputParameterRepresentation<decimal> decimalRepresentationInput)
            {
                return decimalRepresentationInput.ToSqlParameter();
            }
            else if (parameterRepresentation is SqlInputParameterRepresentation<int> intRepresentationInput)
            {
                return intRepresentationInput.ToSqlParameter();
            }
            else if (parameterRepresentation is SqlInputParameterRepresentation<int?> intNullableRepresentationInput)
            {
                return intNullableRepresentationInput.ToSqlParameter();
            }
            else if (parameterRepresentation is SqlInputParameterRepresentation<long> longRepresentationInput)
            {
                return longRepresentationInput.ToSqlParameter();
            }
            else if (parameterRepresentation is SqlInputParameterRepresentation<long?> longNullableRepresentationInput)
            {
                return longNullableRepresentationInput.ToSqlParameter();
            }
            else if (parameterRepresentation is SqlInputParameterRepresentation<string> stringRepresentationInput)
            {
                return stringRepresentationInput.ToSqlParameter();
            }
            else if (parameterRepresentation is SqlInputParameterRepresentation<DateTime> dateTimeRepresentationInput)
            {
                return dateTimeRepresentationInput.ToSqlParameter();
            }
            else if (parameterRepresentation is SqlInputParameterRepresentation<DateTime?> dateTimeNullableRepresentationInput)
            {
                return dateTimeNullableRepresentationInput.ToSqlParameter();
            }
            else if (parameterRepresentation is SqlOutputParameterRepresentation<byte[]> binaryRepresentationOutput)
            {
                return binaryRepresentationOutput.ToSqlParameter();
            }
            else if (parameterRepresentation is SqlOutputParameterRepresentation<decimal> decimalRepresentationOutput)
            {
                return decimalRepresentationOutput.ToSqlParameter();
            }
            else if (parameterRepresentation is SqlOutputParameterRepresentation<int> intRepresentationOutput)
            {
                return intRepresentationOutput.ToSqlParameter();
            }
            else if (parameterRepresentation is SqlOutputParameterRepresentation<int?> intNullableRepresentationOutput)
            {
                return intNullableRepresentationOutput.ToSqlParameter();
            }
            else if (parameterRepresentation is SqlOutputParameterRepresentation<long> longRepresentationOutput)
            {
                return longRepresentationOutput.ToSqlParameter();
            }
            else if (parameterRepresentation is SqlOutputParameterRepresentation<long?> longNullableRepresentationOutput)
            {
                return longNullableRepresentationOutput.ToSqlParameter();
            }
            else if (parameterRepresentation is SqlOutputParameterRepresentation<string> stringRepresentationOutput)
            {
                return stringRepresentationOutput.ToSqlParameter();
            }
            else if (parameterRepresentation is SqlOutputParameterRepresentation<DateTime> dateTimeRepresentationOutput)
            {
                return dateTimeRepresentationOutput.ToSqlParameter();
            }
            else if (parameterRepresentation is SqlOutputParameterRepresentation<DateTime?> dateTimeNullableRepresentationOutput)
            {
                return dateTimeNullableRepresentationOutput.ToSqlParameter();
            }
            else if (parameterRepresentation.GetType().GetGenericArguments().SingleOrDefault()?.IsEnum ?? false)
            {
                var genericDefinition = parameterRepresentation.GetType().GetGenericTypeDefinition();
                if (genericDefinition == typeof(SqlInputParameterRepresentation<>))
                {
                    var enumValue = parameterRepresentation.GetType()
                                                           .GetProperty(nameof(SqlInputParameterRepresentation<string>.Value))
                                                          ?.GetValue(parameterRepresentation);
                    var stringParameter = new SqlInputParameterRepresentation<string>(parameterRepresentation.Name, parameterRepresentation.DataType, enumValue?.ToString());
                    return stringParameter.ToSqlParameter();
                }
                else if (genericDefinition == typeof(SqlOutputParameterRepresentation<>))
                {
                    var stringParameter = new SqlOutputParameterRepresentation<string>(parameterRepresentation.Name, parameterRepresentation.DataType);
                    return stringParameter.ToSqlParameter();
                }
                else
                {
                    throw new NotSupportedException(FormattableString.Invariant($"Param type {parameterRepresentation.GetType().ToStringReadable()} is not supported."));
                }
            }
            else
            {
                throw new NotSupportedException(FormattableString.Invariant($"{nameof(SqlParameterRepresentationBase)} {nameof(parameterRepresentation)} of type {parameterRepresentation.GetType().ToStringReadable()} is not a supported type parameter."));
            }
        }

        /// <summary>
        /// Converts to a <see cref="SqlParameter"/>.
        /// </summary>
        /// <param name="binaryInputParameter">The decimal input parameter.</param>
        /// <returns>SqlParameter.</returns>
        public static SqlParameter ToSqlParameter(
            this SqlInputParameterRepresentation<byte[]> binaryInputParameter)
        {
            binaryInputParameter.MustForArg(nameof(binaryInputParameter)).NotBeNull();
            var name = binaryInputParameter.Name;
            name = name.StartsWith("@", StringComparison.Ordinal) ? name : "@" + name;
            var result = binaryInputParameter.Value.CreateInputSqlParameter(name);
            return result;
        }

        /// <summary>
        /// Converts to a <see cref="SqlParameter"/>.
        /// </summary>
        /// <param name="decimalInputParameter">The decimal input parameter.</param>
        /// <returns>SqlParameter.</returns>
        public static SqlParameter ToSqlParameter(
            this SqlInputParameterRepresentation<decimal> decimalInputParameter)
        {
            decimalInputParameter.MustForArg(nameof(decimalInputParameter)).NotBeNull();
            var name = decimalInputParameter.Name;
            name = name.StartsWith("@", StringComparison.Ordinal) ? name : "@" + name;
            var result = decimalInputParameter.Value.CreateInputSqlParameter(name);
            return result;
        }

        /// <summary>
        /// Converts to a <see cref="SqlParameter"/>.
        /// </summary>
        /// <param name="intInputParameter">The int input parameter.</param>
        /// <returns>SqlParameter.</returns>
        public static SqlParameter ToSqlParameter(
            this SqlInputParameterRepresentation<int> intInputParameter)
        {
            intInputParameter.MustForArg(nameof(intInputParameter)).NotBeNull();
            var name = intInputParameter.Name;
            name = name.StartsWith("@", StringComparison.Ordinal) ? name : "@" + name;
            var result = intInputParameter.Value.CreateInputSqlParameter(name);
            return result;
        }

        /// <summary>
        /// Converts to a <see cref="SqlParameter"/>.
        /// </summary>
        /// <param name="intInputParameter">The int input parameter.</param>
        /// <returns>SqlParameter.</returns>
        public static SqlParameter ToSqlParameter(
            this SqlInputParameterRepresentation<int?> intInputParameter)
        {
            intInputParameter.MustForArg(nameof(intInputParameter)).NotBeNull();
            var name = intInputParameter.Name;
            name = name.StartsWith("@", StringComparison.Ordinal) ? name : "@" + name;
            var result = intInputParameter.Value.CreateInputSqlParameter(name);
            return result;
        }

        /// <summary>
        /// Converts to a <see cref="SqlParameter"/>.
        /// </summary>
        /// <param name="longInputParameter">The long input parameter.</param>
        /// <returns>SqlParameter.</returns>
        public static SqlParameter ToSqlParameter(
            this SqlInputParameterRepresentation<long> longInputParameter)
        {
            longInputParameter.MustForArg(nameof(longInputParameter)).NotBeNull();
            var name = longInputParameter.Name;
            name = name.StartsWith("@", StringComparison.Ordinal) ? name : "@" + name;
            var result = longInputParameter.Value.CreateInputSqlParameter(name);
            return result;
        }

        /// <summary>
        /// Converts to a <see cref="SqlParameter"/>.
        /// </summary>
        /// <param name="longInputParameter">The long input parameter.</param>
        /// <returns>SqlParameter.</returns>
        public static SqlParameter ToSqlParameter(
            this SqlInputParameterRepresentation<long?> longInputParameter)
        {
            longInputParameter.MustForArg(nameof(longInputParameter)).NotBeNull();
            var name = longInputParameter.Name;
            name = name.StartsWith("@", StringComparison.Ordinal) ? name : "@" + name;
            var result = longInputParameter.Value.CreateInputSqlParameter(name);
            return result;
        }

        /// <summary>
        /// Converts to a <see cref="SqlParameter"/>.
        /// </summary>
        /// <param name="stringInputParameter">The string input parameter.</param>
        /// <returns>SqlParameter.</returns>
        public static SqlParameter ToSqlParameter(
            this SqlInputParameterRepresentation<string> stringInputParameter)
        {
            stringInputParameter.MustForArg(nameof(stringInputParameter)).NotBeNull();

            if (stringInputParameter.DataType.GetType() == typeof(StringSqlDataTypeRepresentation))
            {
                var stringDataType = (StringSqlDataTypeRepresentation)stringInputParameter.DataType;
                var name = stringInputParameter.Name;
                name = name.StartsWith("@", StringComparison.Ordinal) ? name : "@" + name;
                var result = stringInputParameter.Value.CreateInputSqlParameter(name, stringDataType.SupportUnicode ? SqlDbType.NVarChar : SqlDbType.VarChar, stringDataType.SupportedLength);
                return result;
            }
            else if (stringInputParameter.DataType.GetType() == typeof(XmlSqlDataTypeRepresentation))
            {
                var name = stringInputParameter.Name;
                name = name.StartsWith("@", StringComparison.Ordinal) ? name : "@" + name;
                var result = stringInputParameter.Value.CreateInputSqlParameter(name, SqlDbType.Xml);
                return result;
            }
            else
            {
                throw new NotSupportedException(FormattableString.Invariant($"Cannot create a {nameof(SqlParameter)} from {nameof(SqlInputParameterRepresentation<string>)} with a datatype of {stringInputParameter.DataType}."));
            }
        }

        /// <summary>
        /// Converts to a <see cref="SqlParameter"/>.
        /// </summary>
        /// <param name="dateTimeInputParameter">The <see cref="DateTime"/> input parameter.</param>
        /// <returns>SqlParameter.</returns>
        public static SqlParameter ToSqlParameter(
            this SqlInputParameterRepresentation<DateTime> dateTimeInputParameter)
        {
            dateTimeInputParameter.MustForArg(nameof(dateTimeInputParameter)).NotBeNull();
            var name = dateTimeInputParameter.Name;
            name = name.StartsWith("@", StringComparison.Ordinal) ? name : "@" + name;
            var result = dateTimeInputParameter.Value.CreateInputSqlParameter(name);
            return result;
        }

        /// <summary>
        /// Converts to a <see cref="SqlParameter"/>.
        /// </summary>
        /// <param name="dateTimeInputParameter">The nullable <see cref="DateTime"/> input parameter.</param>
        /// <returns>SqlParameter.</returns>
        public static SqlParameter ToSqlParameter(
            this SqlInputParameterRepresentation<DateTime?> dateTimeInputParameter)
        {
            dateTimeInputParameter.MustForArg(nameof(dateTimeInputParameter)).NotBeNull();
            var name = dateTimeInputParameter.Name;
            name = name.StartsWith("@", StringComparison.Ordinal) ? name : "@" + name;
            var result = dateTimeInputParameter.Value.CreateInputSqlParameter(name);
            return result;
        }

        /// <summary>
        /// Converts to a <see cref="SqlParameter"/>.
        /// </summary>
        /// <param name="binaryOutputParameter">The decimal Output parameter.</param>
        /// <returns>SqlParameter.</returns>
        public static SqlParameter ToSqlParameter(
            this SqlOutputParameterRepresentation<byte[]> binaryOutputParameter)
        {
            binaryOutputParameter.MustForArg(nameof(binaryOutputParameter)).NotBeNull();
            var dataType = (BinarySqlDataTypeRepresentation)binaryOutputParameter.DataType;
            var name = binaryOutputParameter.Name;
            name = name.StartsWith("@", StringComparison.Ordinal) ? name : "@" + name;
            var result = SqlParameterExtensions.CreateOutputByteArraySqlParameter(name, size: dataType.SupportedLength);
            return result;
        }

        /// <summary>
        /// Converts to a <see cref="SqlParameter"/>.
        /// </summary>
        /// <param name="decimalOutputParameter">The decimal Output parameter.</param>
        /// <returns>SqlParameter.</returns>
        public static SqlParameter ToSqlParameter(
            this SqlOutputParameterRepresentation<decimal> decimalOutputParameter)
        {
            decimalOutputParameter.MustForArg(nameof(decimalOutputParameter)).NotBeNull();
            var dataType = (DecimalSqlDataTypeRepresentation)decimalOutputParameter.DataType;
            var name = decimalOutputParameter.Name;
            name = name.StartsWith("@", StringComparison.Ordinal) ? name : "@" + name;
            var result = SqlParameterExtensions.CreateOutputDecimalSqlParameter(name, dataType.Precision, dataType.Scale);
            return result;
        }

        /// <summary>
        /// Converts to a <see cref="SqlParameter"/>.
        /// </summary>
        /// <param name="intOutputParameter">The int Output parameter.</param>
        /// <returns>SqlParameter.</returns>
        public static SqlParameter ToSqlParameter(
            this SqlOutputParameterRepresentation<int> intOutputParameter)
        {
            intOutputParameter.MustForArg(nameof(intOutputParameter)).NotBeNull();
            var name = intOutputParameter.Name;
            name = name.StartsWith("@", StringComparison.Ordinal) ? name : "@" + name;
            var result = SqlParameterExtensions.CreateOutputIntSqlParameter(name);
            return result;
        }

        /// <summary>
        /// Converts to a <see cref="SqlParameter"/>.
        /// </summary>
        /// <param name="intOutputParameter">The int Output parameter.</param>
        /// <returns>SqlParameter.</returns>
        public static SqlParameter ToSqlParameter(
            this SqlOutputParameterRepresentation<int?> intOutputParameter)
        {
            intOutputParameter.MustForArg(nameof(intOutputParameter)).NotBeNull();
            var name = intOutputParameter.Name;
            name = name.StartsWith("@", StringComparison.Ordinal) ? name : "@" + name;
            var result = SqlParameterExtensions.CreateOutputIntSqlParameter(name);
            return result;
        }

        /// <summary>
        /// Converts to a <see cref="SqlParameter"/>.
        /// </summary>
        /// <param name="longOutputParameter">The long Output parameter.</param>
        /// <returns>SqlParameter.</returns>
        public static SqlParameter ToSqlParameter(
            this SqlOutputParameterRepresentation<long> longOutputParameter)
        {
            longOutputParameter.MustForArg(nameof(longOutputParameter)).NotBeNull();
            var name = longOutputParameter.Name;
            name = name.StartsWith("@", StringComparison.Ordinal) ? name : "@" + name;
            var result = SqlParameterExtensions.CreateOutputLongSqlParameter(name);
            return result;
        }

        /// <summary>
        /// Converts to a <see cref="SqlParameter"/>.
        /// </summary>
        /// <param name="longOutputParameter">The long Output parameter.</param>
        /// <returns>SqlParameter.</returns>
        public static SqlParameter ToSqlParameter(
            this SqlOutputParameterRepresentation<long?> longOutputParameter)
        {
            longOutputParameter.MustForArg(nameof(longOutputParameter)).NotBeNull();
            var name = longOutputParameter.Name;
            name = name.StartsWith("@", StringComparison.Ordinal) ? name : "@" + name;
            var result = SqlParameterExtensions.CreateOutputLongSqlParameter(name);
            return result;
        }

        /// <summary>
        /// Converts to a <see cref="SqlParameter"/>.
        /// </summary>
        /// <param name="stringOutputParameter">The string Output parameter.</param>
        /// <returns>SqlParameter.</returns>
        public static SqlParameter ToSqlParameter(
            this SqlOutputParameterRepresentation<string> stringOutputParameter)
        {
            stringOutputParameter.MustForArg(nameof(stringOutputParameter)).NotBeNull();

            if (stringOutputParameter.DataType.GetType() == typeof(StringSqlDataTypeRepresentation))
            {
                var dataType = (StringSqlDataTypeRepresentation)stringOutputParameter.DataType;
                var name = stringOutputParameter.Name;
                name = name.StartsWith("@", StringComparison.Ordinal) ? name : "@" + name;
                var result = SqlParameterExtensions.CreateOutputStringSqlParameter(name, dataType.SupportUnicode ? SqlDbType.NVarChar : SqlDbType.VarChar, dataType.SupportedLength);
                return result;
            }
            else if (stringOutputParameter.DataType.GetType() == typeof(XmlSqlDataTypeRepresentation))
            {
                var name = stringOutputParameter.Name;
                name = name.StartsWith("@", StringComparison.Ordinal) ? name : "@" + name;
                var result = SqlParameterExtensions.CreateOutputStringSqlParameter(name, SqlDbType.Xml);
                return result;
            }
            else
            {
                throw new NotSupportedException(FormattableString.Invariant($"Cannot create a {nameof(SqlParameter)} from {nameof(SqlOutputParameterRepresentation<string>)} with a datatype of {stringOutputParameter.DataType}."));
            }
        }

        /// <summary>
        /// Converts to a <see cref="SqlParameter"/>.
        /// </summary>
        /// <param name="dateTimeOutputParameter">The <see cref="DateTime"/> Output parameter.</param>
        /// <returns>SqlParameter.</returns>
        public static SqlParameter ToSqlParameter(
            this SqlOutputParameterRepresentation<DateTime> dateTimeOutputParameter)
        {
            dateTimeOutputParameter.MustForArg(nameof(dateTimeOutputParameter)).NotBeNull();
            var name = dateTimeOutputParameter.Name;
            name = name.StartsWith("@", StringComparison.Ordinal) ? name : "@" + name;
            var result = SqlParameterExtensions.CreateOutputDateTimeSqlParameter(name);
            return result;
        }

        /// <summary>
        /// Converts to a <see cref="SqlParameter"/>.
        /// </summary>
        /// <param name="dateTimeOutputParameter">The nullable <see cref="DateTime"/> Output parameter.</param>
        /// <returns>SqlParameter.</returns>
        public static SqlParameter ToSqlParameter(
            this SqlOutputParameterRepresentation<DateTime?> dateTimeOutputParameter)
        {
            dateTimeOutputParameter.MustForArg(nameof(dateTimeOutputParameter)).NotBeNull();
            var name = dateTimeOutputParameter.Name;
            name = name.StartsWith("@", StringComparison.Ordinal) ? name : "@" + name;
            var result = SqlParameterExtensions.CreateOutputDateTimeSqlParameter(name);
            return result;
        }

        /// <summary>
        /// Creates a <see cref="ISqlOutputParameterRepresentationWithResult"/> with the provided result.
        /// </summary>
        /// <param name="outputParameterRepresentation">The output parameter representation.</param>
        /// <param name="valueFromActualStoredProcedureParameter">The value from actual stored procedure parameter.</param>
        /// <returns>A <see cref="ISqlOutputParameterRepresentationWithResult"/> with the provided result.</returns>
        public static ISqlOutputParameterRepresentationWithResult CreateWithResult(
            this SqlOutputParameterRepresentationBase outputParameterRepresentation,
            object valueFromActualStoredProcedureParameter)
        {
            // accommodate DBNull situation here.
            var rawValue = DBNull.Value.Equals(valueFromActualStoredProcedureParameter) ? null : valueFromActualStoredProcedureParameter;

            ISqlOutputParameterRepresentationWithResult result = null;

            if (outputParameterRepresentation is SqlOutputParameterRepresentation<byte[]>)
            {
                var byteArrayValue = (byte[])rawValue;
                result = new SqlOutputParameterRepresentationWithResult<byte[]>(outputParameterRepresentation.Name, outputParameterRepresentation.DataType, byteArrayValue);
            }
            else if (outputParameterRepresentation is SqlOutputParameterRepresentation<decimal>)
            {
                rawValue.MustForArg(nameof(rawValue)).NotBeNull();
                var decimalValue = Convert.ToDecimal(rawValue, CultureInfo.InvariantCulture);
                result = new SqlOutputParameterRepresentationWithResult<decimal>(outputParameterRepresentation.Name, outputParameterRepresentation.DataType, decimalValue);
            }
            else if (outputParameterRepresentation is SqlOutputParameterRepresentation<int>)
            {
                rawValue.MustForArg(nameof(rawValue)).NotBeNull();
                var intValue = Convert.ToInt32(rawValue, CultureInfo.InvariantCulture);
                result = new SqlOutputParameterRepresentationWithResult<int>(outputParameterRepresentation.Name, outputParameterRepresentation.DataType, intValue);
            }
            else if (outputParameterRepresentation is SqlOutputParameterRepresentation<int?>)
            {
                var intValue = rawValue == null ? (int?)null : Convert.ToInt32(rawValue, CultureInfo.InvariantCulture);
                result = new SqlOutputParameterRepresentationWithResult<int?>(outputParameterRepresentation.Name, outputParameterRepresentation.DataType, intValue);
            }
            else if (outputParameterRepresentation is SqlOutputParameterRepresentation<long>)
            {
                rawValue.MustForArg(nameof(rawValue)).NotBeNull();
                var longValue = Convert.ToInt64(rawValue, CultureInfo.InvariantCulture);
                result = new SqlOutputParameterRepresentationWithResult<long>(outputParameterRepresentation.Name, outputParameterRepresentation.DataType, longValue);
            }
            else if (outputParameterRepresentation is SqlOutputParameterRepresentation<long?>)
            {
                var longValue = rawValue == null ? (long?)null : Convert.ToInt64(rawValue, CultureInfo.InvariantCulture);
                result = new SqlOutputParameterRepresentationWithResult<long?>(outputParameterRepresentation.Name, outputParameterRepresentation.DataType, longValue);
            }
            else if (outputParameterRepresentation is SqlOutputParameterRepresentation<string>)
            {
                var stringValue = rawValue?.ToString();
                result = new SqlOutputParameterRepresentationWithResult<string>(outputParameterRepresentation.Name, outputParameterRepresentation.DataType, stringValue);
            }
            else if (outputParameterRepresentation is SqlOutputParameterRepresentation<DateTime>)
            {
                rawValue.MustForArg(nameof(rawValue)).NotBeNull();
                var dateTimeValue = (DateTime)rawValue;
                result = new SqlOutputParameterRepresentationWithResult<DateTime>(outputParameterRepresentation.Name, outputParameterRepresentation.DataType, dateTimeValue);
            }
            else if (outputParameterRepresentation is SqlOutputParameterRepresentation<DateTime?>)
            {
                var dateTimeNullableValue = (DateTime?)rawValue;
                result = new SqlOutputParameterRepresentationWithResult<DateTime?>(outputParameterRepresentation.Name, outputParameterRepresentation.DataType, dateTimeNullableValue);
            }
            else if (outputParameterRepresentation.GetType().GetGenericArguments().SingleOrDefault()?.IsEnum ?? false)
            {
                rawValue.MustForArg(nameof(rawValue)).NotBeNull();

                var genericDefinition = outputParameterRepresentation.GetType().GetGenericTypeDefinition();
                genericDefinition.MustForArg(nameof(genericDefinition)).BeEqualTo(typeof(SqlOutputParameterRepresentation<>));

                var genericArguments = outputParameterRepresentation.GetType().GetGenericArguments().ToList();
                var enumType = genericArguments.Single();
                var enumValue = Enum.Parse(enumType, rawValue.ToString());
                var resultType = typeof(SqlOutputParameterRepresentationWithResult<>).MakeGenericType(enumType);
                var rawResult = resultType.Construct(outputParameterRepresentation.Name, outputParameterRepresentation.DataType, enumValue);
                result = (ISqlOutputParameterRepresentationWithResult)rawResult;
            }
            else
            {
                throw new NotSupportedException(FormattableString.Invariant($"A {nameof(SqlDataTypeRepresentationBase)} {nameof(outputParameterRepresentation)} of type {outputParameterRepresentation.GetType().ToStringReadable()} is not supported."));
            }

            return result;
        }
    }
}
