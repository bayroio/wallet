﻿using System;
using Nethereum.Hex.HexConvertors.Extensions;

namespace BayroWallet
{
    /// <summary>
    /// Contains convenience string extensions for dealing
    /// with Ethereum-related concepts and hex-strings.
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Verifies that the given string value represents a valid Ethereum address.
        /// It attempts to convert the given value to a byte array (interpreting the
        /// string as a hex string), and if it succeeds - verifying that the byte array
        /// is 20 bytes long (which is the length of Ethereum addresses).
        /// </summary>
        /// <param name="val">The string which needs to be verified.</param>
        /// <returns>True if the string is a valid 20-byte hex string; otherwise false.</returns>
        internal static bool IsValidEthereumAddress(this string val)
        {
            try
            {
                return val.HexToByteArray().Length == 20;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}
