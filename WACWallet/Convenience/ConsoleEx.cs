﻿using System;
using System.Numerics;
using Nethereum.Hex.HexConvertors.Extensions;

namespace BayroWallet
{
    /// <summary>
    /// Contains helper methods for prompting for values via the console.
    /// </summary>
    internal static class ConsoleEx
    {
        /// <summary>
        /// Prompts the user for a valid Ethereum address. It must
        /// be a valid hex-string representing 20 bytes.
        /// It can optionally have a "0x" prefix, but it is not required.
        ///
        /// The user will be prompted in a loop, until a valid value
        /// is entered.
        /// </summary>
        /// <param name="message">A message to display to the user before each input prompt.</param>
        /// <returns>A hex string representing 20 bytes.</returns>
        internal static string PromptForAddress(string message)
        {
            string result;
            do
            {
                result = Prompt(message);
            }
            while (!result.IsValidEthereumAddress());

            return result;
        }

        /// <summary>
        /// The function will prompt the user to enter a value, and
        /// try to parse it as a decimal.
        ///
        /// When it succeeds, it will return the decimal.
        /// </summary>
        /// <param name="message">A message to display to the user before each input prompt.</param>
        /// <returns>The parsed decimal value.</returns>
        internal static decimal PromptForDecimal(string message)
        {
            decimal? result = null;
            do
            {
                var line = Prompt(message);

                decimal amt;
                if (decimal.TryParse(line, out amt))
                {
                    result = amt;
                }
            }
            while (!result.HasValue);

            return result.Value;
        }

        /// <summary>
        /// This function acts like PromptForOptionalBigInteger() except it will always return
        /// a BigInteger, and never null values.
        /// </summary>
        /// <param name="message">A message to display to the user before each input prompt.</param>
        /// <param name="defaultValue">Value to return if the user enters an empty string.</param>
        /// <returns>A BigInteger representing the user-entered value, or the specified default value.</returns>
        internal static BigInteger PromptForBigInteger(string message, BigInteger defaultValue)
        {
            BigInteger? result = null;
            do
            {
                result = PromptForOptionalBigInteger(message, defaultValue);
            }
            while (!result.HasValue);

            return result.Value;
        }

        /// <summary>
        /// The function will prompt the user to enter a value, and
        /// try to parse it as a BigInteger. If it succeeds, it will
        /// return the parsed value.
        ///
        /// If the user enters an empty string, the default value (which can be null)
        /// will be returned.
        /// </summary>
        /// <param name="message">A message to display to the user before each input prompt.</param>
        /// <param name="defaultValue">Value to return if the user enters an empty string.</param>
        /// <returns>
        /// An optional BigInteger. The only way it can be null, is if the defaultValue is null and the user entered an empty string.
        /// </returns>
        internal static BigInteger? PromptForOptionalBigInteger(string message, BigInteger? defaultValue)
        {
            BigInteger? result = null;
            do
            {
                var line = Prompt(message);
                if (string.IsNullOrWhiteSpace(line))
                {
                    return defaultValue;
                }

                BigInteger parsedValue;
                if (BigInteger.TryParse(line, out parsedValue))
                {
                    result = parsedValue;
                }
            }
            while (!result.HasValue);

            return result.Value;
        }

        private static string Prompt(string message)
        {
            Console.Write("{0}: ", message);
            return Console.ReadLine();
        }
    }
}
