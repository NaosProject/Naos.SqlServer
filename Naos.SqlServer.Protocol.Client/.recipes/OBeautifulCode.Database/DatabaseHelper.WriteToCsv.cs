﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseHelper.WriteToCsv.cs" company="OBeautifulCode">
//   Copyright (c) OBeautifulCode 2018. All rights reserved.
// </copyright>
// <auto-generated>
//   Sourced from NuGet package. Will be overwritten with package update except in OBeautifulCode.Database.Recipes source.
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace OBeautifulCode.Database.Recipes
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Data;
    using global::System.Data.SqlClient;
    using global::System.Globalization;
    using global::System.IO;
    using global::System.Linq;
    using global::System.Threading.Tasks;

    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.String.Recipes;

    using static global::System.FormattableString;

#if !OBeautifulCodeDatabaseSolution
    internal
#else
    public
#endif
    static partial class DatabaseHelper
    {
        /// <summary>
        /// Opens a connection to the database, executes the <paramref name="commandText"/>, and writes the results to a CSV file.
        /// </summary>
        /// <remarks>
        /// Sets CommandBehavior = CommandBehavior.CloseConnection so that the created connection is closed when the data reader is closed.
        /// </remarks>
        /// <param name="connectionString">String used to open a connection to the database.</param>
        /// <param name="commandText">The SQL statement, table name, or stored procedure to execute at the data source.</param>
        /// <param name="outputFilePath">Path to file where CSV data should be written.</param>
        /// <param name="includeColumnNames">Indicates whether the first row should be populated with column names.</param>
        /// <param name="commandTimeoutInSeconds">OPTIONAL value with the wait time, in seconds, before terminating an attempt to execute the command and generating an error.  DEFAULT is 30 seconds.  A value of 0 indicates no limit (an attempt to execute a command will wait indefinitely).</param>
        /// <param name="commandParameters">OPTIONAL set of parameters to associate with the command.  DEFAULT is null (no parameters).</param>
        /// <param name="commandType">OPTIONAL value that determines how the command text is to be interpreted.  DEFAULT is <see cref="CommandType.Text"/>; a SQL text command.</param>
        /// <param name="commandBehavior">OPTIONAL value providing a description of the results of the query and its effect on the database.  DEFAULT is <see cref="CommandBehavior.Default"/>; the query may return multiple result sets and execution of the query may affect the database state.  This enumeration has a FlagsAttribute attribute that allows a bitwise combination of its member values.</param>
        /// <param name="prepareCommand">OPTIONAL value indicating whether to prepared (or compile) the command on the data source.</param>
        /// <param name="sqlInfoMessageEventHandler">OPTIONAL method that will handle the <see cref="SqlConnection.InfoMessage"/> event.</param>
        public static void WriteToCsv(
            this string connectionString,
            string commandText,
            string outputFilePath,
            bool includeColumnNames = true,
            int commandTimeoutInSeconds = 30,
            IReadOnlyList<SqlParameter> commandParameters = null,
            CommandType commandType = CommandType.Text,
            CommandBehavior commandBehavior = CommandBehavior.CloseConnection,
            bool prepareCommand = false,
            SqlInfoMessageEventHandler sqlInfoMessageEventHandler = null)
        {
            if (outputFilePath == null)
            {
                throw new ArgumentNullException(nameof(outputFilePath));
            }

            if (string.IsNullOrWhiteSpace(outputFilePath))
            {
                throw new ArgumentException(Invariant($"'{nameof(outputFilePath)}' is white space"));
            }

            using (var writer = new StreamWriter(outputFilePath))
            {
                using (var reader = connectionString.ExecuteReader(commandText, commandTimeoutInSeconds, commandParameters, commandType, commandBehavior, prepareCommand, sqlInfoMessageEventHandler))
                {
                    reader.WriteToCsv(writer, includeColumnNames);
                }
            }
        }

        /// <summary>
        /// Opens a connection to the database, executes the <paramref name="commandText"/>, and writes the results to a CSV file.
        /// </summary>
        /// <remarks>
        /// Sets CommandBehavior = CommandBehavior.CloseConnection so that the created connection is closed when the data reader is closed.
        /// </remarks>
        /// <param name="connectionString">String used to open a connection to the database.</param>
        /// <param name="commandText">The SQL statement, table name, or stored procedure to execute at the data source.</param>
        /// <param name="outputFilePath">Path to file where CSV data should be written.</param>
        /// <param name="includeColumnNames">Indicates whether the first row should be populated with column names.</param>
        /// <param name="commandTimeoutInSeconds">OPTIONAL value with the wait time, in seconds, before terminating an attempt to execute the command and generating an error.  DEFAULT is 30 seconds.  A value of 0 indicates no limit (an attempt to execute a command will wait indefinitely).</param>
        /// <param name="commandParameters">OPTIONAL set of parameters to associate with the command.  DEFAULT is null (no parameters).</param>
        /// <param name="commandType">OPTIONAL value that determines how the command text is to be interpreted.  DEFAULT is <see cref="CommandType.Text"/>; a SQL text command.</param>
        /// <param name="commandBehavior">OPTIONAL value providing a description of the results of the query and its effect on the database.  DEFAULT is <see cref="CommandBehavior.Default"/>; the query may return multiple result sets and execution of the query may affect the database state.  This enumeration has a FlagsAttribute attribute that allows a bitwise combination of its member values.</param>
        /// <param name="prepareCommand">OPTIONAL value indicating whether to prepared (or compile) the command on the data source.</param>
        /// <param name="sqlInfoMessageEventHandler">OPTIONAL method that will handle the <see cref="SqlConnection.InfoMessage"/> event.</param>
        /// <returns>
        /// A task.
        /// </returns>
        public static async Task WriteToCsvAsync(
            this string connectionString,
            string commandText,
            string outputFilePath,
            bool includeColumnNames = true,
            int commandTimeoutInSeconds = 30,
            IReadOnlyList<SqlParameter> commandParameters = null,
            CommandType commandType = CommandType.Text,
            CommandBehavior commandBehavior = CommandBehavior.CloseConnection,
            bool prepareCommand = false,
            SqlInfoMessageEventHandler sqlInfoMessageEventHandler = null)
        {
            if (outputFilePath == null)
            {
                throw new ArgumentNullException(nameof(outputFilePath));
            }

            if (string.IsNullOrWhiteSpace(outputFilePath))
            {
                throw new ArgumentException(Invariant($"'{nameof(outputFilePath)}' is white space"));
            }

            using (var writer = new StreamWriter(outputFilePath))
            {
                using (var reader = await connectionString.ExecuteReaderAsync(commandText, commandTimeoutInSeconds, commandParameters, commandType, commandBehavior, prepareCommand, sqlInfoMessageEventHandler))
                {
                    await reader.WriteToCsvAsync(writer, includeColumnNames);
                }
            }
        }

        /// <summary>
        /// Executes the <paramref name="commandText"/> against the <paramref name="connection"/> and writes the results to a CSV file.
        /// </summary>
        /// <param name="connection">An open connection to the database.</param>
        /// <param name="commandText">The SQL statement, table name, or stored procedure to execute at the data source.</param>
        /// <param name="outputFilePath">Path to file where CSV data should be written.</param>
        /// <param name="includeColumnNames">Indicates whether the first row should be populated with column names.</param>
        /// <param name="commandTimeoutInSeconds">OPTIONAL value with the wait time, in seconds, before terminating an attempt to execute the command and generating an error.  DEFAULT is 30 seconds.  A value of 0 indicates no limit (an attempt to execute a command will wait indefinitely).</param>
        /// <param name="commandParameters">OPTIONAL set of parameters to associate with the command.  DEFAULT is null (no parameters).</param>
        /// <param name="commandType">OPTIONAL value that determines how the command text is to be interpreted.  DEFAULT is <see cref="CommandType.Text"/>; a SQL text command.</param>
        /// <param name="transaction">OPTIONAL transaction within which the command will execute.  DEFAULT is null (no transaction).</param>
        /// <param name="commandBehavior">OPTIONAL value providing a description of the results of the query and its effect on the database.  DEFAULT is <see cref="CommandBehavior.Default"/>; the query may return multiple result sets and execution of the query may affect the database state.  This enumeration has a FlagsAttribute attribute that allows a bitwise combination of its member values.</param>
        /// <param name="prepareCommand">OPTIONAL value indicating whether to prepared (or compile) the command on the data source.</param>
        public static void WriteToCsv(
            this SqlConnection connection,
            string commandText,
            string outputFilePath,
            bool includeColumnNames = true,
            int commandTimeoutInSeconds = 30,
            IReadOnlyList<SqlParameter> commandParameters = null,
            CommandType commandType = CommandType.Text,
            SqlTransaction transaction = null,
            CommandBehavior commandBehavior = CommandBehavior.Default,
            bool prepareCommand = false)
        {
            if (outputFilePath == null)
            {
                throw new ArgumentNullException(nameof(outputFilePath));
            }

            if (string.IsNullOrWhiteSpace(outputFilePath))
            {
                throw new ArgumentException(Invariant($"'{nameof(outputFilePath)}' is white space"));
            }

            using (var writer = new StreamWriter(outputFilePath))
            {
                using (var reader = ExecuteReader(connection, commandText, commandTimeoutInSeconds, commandParameters, commandType, transaction, commandBehavior, prepareCommand))
                {
                    reader.WriteToCsv(writer, includeColumnNames);
                }
            }
        }

        private static void WriteToCsv(
            this SqlDataReader reader,
            StreamWriter writer,
            bool includeColumnNames)
        {
            try
            {
                if (reader.FieldCount == 0)
                {
                    throw new InvalidOperationException("A result set wasn't found when executing the command.  Command is a non-query.");
                }

                // write headers
                if (includeColumnNames)
                {
                    var headers = new List<string>();

                    for (var x = 0; x < reader.FieldCount; x++)
                    {
                        headers.Add(reader.GetName(x));
                    }

                    writer.Write(headers.ToCsv());
                }

                // write content
                while (reader.Read())
                {
                    var rowValues = new List<string>();

                    for (var x = 0; x < reader.FieldCount; x++)
                    {
                        if (reader.IsDBNull(x))
                        {
                            rowValues.Add(null);
                        }
                        else
                        {
                            var value = reader.GetValue(x);

                            // strings, chars, and char arrays need to be made CSV-safe.
                            // other data types are guaranteed to never violate CSV-safety rules.
                            if (value is string stringValue)
                            {
                                rowValues.Add(stringValue.ToCsvSafe());
                            }
                            else if (value is char)
                            {
                                rowValues.Add(value.ToString().ToCsvSafe());
                            }
                            else if (value is char[] charArrayValue)
                            {
                                rowValues.Add(charArrayValue.Select(_ => _.ToString(CultureInfo.InvariantCulture)).ToDelimitedString(string.Empty).ToCsvSafe());
                            }
                            else if (value is DateTime valueAsDate)
                            {
                                // DateTime.ToString() will truncate time.
                                var dateAsString = string.Empty;
                                if (valueAsDate.Kind == DateTimeKind.Unspecified)
                                {
                                    // ReSharper disable once StringLiteralTypo
                                    dateAsString = valueAsDate.ToString("yyyy-MM-dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture);
                                }
                                else if (valueAsDate.Kind == DateTimeKind.Local)
                                {
                                    // ReSharper disable once StringLiteralTypo
                                    dateAsString = valueAsDate.ToString("yyyy-MM-dd HH:mm:ss.ffffffzzz", CultureInfo.InvariantCulture);
                                }
                                else if (valueAsDate.Kind == DateTimeKind.Utc)
                                {
                                    // ReSharper disable once StringLiteralTypo
                                    dateAsString = valueAsDate.ToString("yyyy-MM-dd HH:mm:ss.ffffffZ", CultureInfo.InvariantCulture);
                                }

                                rowValues.Add(dateAsString);
                            }
                            else
                            {
                                rowValues.Add(value.ToString());
                            }
                        }
                    }

                    writer.WriteLine();

                    // since we already treated strings for CSV-safety, use ToDelimitedString() instead of ToCsv()
                    writer.Write(rowValues.ToDelimitedString(","));
                }
            }
            finally
            {
                reader.Close();
            }
        }

        private static async Task WriteToCsvAsync(
            this SqlDataReader reader,
            StreamWriter writer,
            bool includeColumnNames)
        {
            try
            {
                if (reader.FieldCount == 0)
                {
                    throw new InvalidOperationException("A result set wasn't found when executing the command.  Command is a non-query.");
                }

                // write headers
                if (includeColumnNames)
                {
                    var headers = new List<string>();

                    for (var x = 0; x < reader.FieldCount; x++)
                    {
                        headers.Add(reader.GetName(x));
                    }

                    await writer.WriteAsync(headers.ToCsv());
                }

                // write content
                while (await reader.ReadAsync())
                {
                    var rowValues = new List<string>();

                    for (var x = 0; x < reader.FieldCount; x++)
                    {
                        if (reader.IsDBNull(x))
                        {
                            rowValues.Add(null);
                        }
                        else
                        {
                            var value = reader.GetValue(x);

                            // strings, chars, and char arrays need to be made CSV-safe.
                            // other data types are guaranteed to never violate CSV-safety rules.
                            if (value is string stringValue)
                            {
                                rowValues.Add(stringValue.ToCsvSafe());
                            }
                            else if (value is char)
                            {
                                rowValues.Add(value.ToString().ToCsvSafe());
                            }
                            else if (value is char[] charArrayValue)
                            {
                                rowValues.Add(charArrayValue.Select(_ => _.ToString(CultureInfo.InvariantCulture)).ToDelimitedString(string.Empty).ToCsvSafe());
                            }
                            else if (value is DateTime valueAsDate)
                            {
                                // DateTime.ToString() will truncate time.
                                var dateAsString = string.Empty;
                                if (valueAsDate.Kind == DateTimeKind.Unspecified)
                                {
                                    // ReSharper disable once StringLiteralTypo
                                    dateAsString = valueAsDate.ToString("yyyy-MM-dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture);
                                }
                                else if (valueAsDate.Kind == DateTimeKind.Local)
                                {
                                    // ReSharper disable once StringLiteralTypo
                                    dateAsString = valueAsDate.ToString("yyyy-MM-dd HH:mm:ss.ffffffzzz", CultureInfo.InvariantCulture);
                                }
                                else if (valueAsDate.Kind == DateTimeKind.Utc)
                                {
                                    // ReSharper disable once StringLiteralTypo
                                    dateAsString = valueAsDate.ToString("yyyy-MM-dd HH:mm:ss.ffffffZ", CultureInfo.InvariantCulture);
                                }

                                rowValues.Add(dateAsString);
                            }
                            else
                            {
                                rowValues.Add(value.ToString());
                            }
                        }
                    }

                    await writer.WriteLineAsync();

                    // since we already treated strings for CSV-safety, use ToDelimitedString() instead of ToCsv()
                    await writer.WriteAsync(rowValues.ToDelimitedString(","));
                }
            }
            finally
            {
                reader.Close();
            }
        }
    }
}