using System;
using System.Numerics;
using Newtonsoft.Json;

namespace BayroWallet
{
    /// <summary>
    /// Contains information about an Ethereum block.
    /// In the case of our wallet, we are interested in the block-number
    /// (for synchronization purposes) and the timestamp (for displaying
    /// to the user).
    /// </summary>
    internal struct Block
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Block"/> struct.
        /// </summary>
        /// <param name="number">BigInteger representing the block number.</param>
        /// <param name="timestamp">DateTimeOffset representing the block's timestamp (i.e. time of mining).</param>
        [JsonConstructor]
        public Block(BigInteger number, DateTimeOffset timestamp)
        {
            this.Number = number;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Gets the block number.
        /// </summary>
        public BigInteger Number { get; }

        /// <summary>
        /// Gets the timestamp of the block (i.e. when it was mined).
        /// </summary>
        public DateTimeOffset Timestamp { get; }
    }
}
