﻿using System;
using System.Numerics;
using Nethereum.Hex.HexConvertors.Extensions;

namespace BayroWallet
{
    /// <summary>
    /// Convenience extensions for converting to/from decimal values.
    /// </summary>
    internal static class DecimalExtensions
    {
        /// <summary>
        /// Converts an ether-denominated decimal value to a BigInteger
        /// wei-denominated value.
        /// </summary>
        /// <param name="val">An ether-denominated decimal value.</param>
        /// <returns>A wei-denominated BigInteger value.</returns>
        internal static BigInteger EthToWei(this decimal val)
        {
            return BigInteger.Parse((val * 1E18M).ToString("G29"));
        }
    }
}
