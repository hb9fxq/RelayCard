// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CardState8RelayFixture.cs" company="kripp.ch GmbH, Frank Werner-Krippendorf">
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
namespace ch.kripp.RelayCardTests
{
    using System;
    using ch.kripp.RelayCard;
    using NUnit.Framework;

    /// <summary>
    /// CardState8Relay unit tests
    /// </summary>
    [TestFixture]
    public class CardState8RelayFixture
    {
        /// <summary>
        /// Initialize testee from byte and evaluate port state
        /// </summary>
        [Test]
        public void FromByteTest()
        {
            // arrange 
            var testee = new CardRelayState(8, 1);
            
            // act
            testee.FromByte(Convert.ToByte(164));

            // assert 
            Assert.AreEqual(false, testee.CardState[0]); 
            Assert.AreEqual(false, testee.CardState[1]); 
            Assert.AreEqual(true, testee.CardState[2]); 
            Assert.AreEqual(false, testee.CardState[3]); 
            Assert.AreEqual(false, testee.CardState[4]); 
            Assert.AreEqual(true, testee.CardState[5]); 
            Assert.AreEqual(false, testee.CardState[6]); 
            Assert.AreEqual(true, testee.CardState[7]); 
        }

        /// <summary>
        /// initialize by card state and evaluate card byte result
        /// </summary>
        [Test]
        public void ToByteArrayTest()
        {
            // arrange
            var testee = new CardRelayState(8, 1);

            testee.CardState[0] = true; 
            testee.CardState[1] = false; 
            testee.CardState[2] = false;
            testee.CardState[3] = false; 
            testee.CardState[4] = true; 
            testee.CardState[5] = true; 
            testee.CardState[6] = false; 
            testee.CardState[7] = false; 

            // act
            var result = testee.ToByteArray()[0];

            // assert
            Assert.AreEqual(Convert.ToByte(49), result);
        }
    }
}
