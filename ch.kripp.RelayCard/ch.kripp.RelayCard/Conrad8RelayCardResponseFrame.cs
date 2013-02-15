// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Conrad8RelayCardResponseFrame.cs" company="kripp.ch GmbH, Frank Werner-Krippendorf">
//   Copyright (c) 2013
//  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
//  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
//  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
//  OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary>
//   Defines the Conrad8RelayCard type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ch.kripp.RelayCard
{
    /// <summary>
    /// Holds all contents of a card response frame
    /// </summary>
    public class Conrad8RelayCardResponseFrame
    {
        /// <summary>
        /// Gets or sets the response command.
        /// </summary>
        /// <value>
        /// The response command.
        /// </value>
        public Conrad8RelayCard.ResponseCommand ResponseCommand { get; set; }

        /// <summary>
        /// Gets or sets the address byte.
        /// </summary>
        /// <value>
        /// The address byte.
        /// </value>
        public byte AddressByte { get; set; }

        /// <summary>
        /// Gets or sets the data byte.
        /// </summary>
        /// <value>
        /// The data byte.
        /// </value>
        public byte DataByte { get; set; }

        /// <summary>
        /// Gets or sets the CRC byte.
        /// </summary>
        /// <value>
        /// The CRC byte.
        /// </value>
        public byte CrcByte { get; set; }

        public override string ToString()
        {
            return string.Format("[ADR:{0} DAT:{2}, CRC:{3}]", this.AddressByte, this.ResponseCommand, this.DataByte, this.CrcByte);
        }

    }
}
