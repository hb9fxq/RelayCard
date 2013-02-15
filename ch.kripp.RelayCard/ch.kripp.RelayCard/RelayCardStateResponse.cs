// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelayCardStateResponse.cs" company="kripp.ch GmbH, Frank Werner-Krippendorf">
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
    ///  Holds a card state response, along with the source response frame 
    /// </summary>
    public class RelayCardStateResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCardStateResponse"/> class.
        /// </summary>
        public RelayCardStateResponse()
        {
            this.CardResponseFrame = new Conrad8RelayCardResponseFrame();
        }

        /// <summary>
        /// Gets or sets the card response frame.
        /// </summary>
        /// <value>
        /// The card response frame.
        /// </value>
        public Conrad8RelayCardResponseFrame CardResponseFrame { get; set; }

        /// <summary>
        /// Gets or sets the state of the relay.
        /// </summary>
        /// <value>
        /// The state of the relay.
        /// </value>
        public CardRelayState RelayState { get; set; }
    }
}