// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Conrad8RelayCardIntegrationFixture.cs" company="kripp.ch GmbH, Frank Werner-Krippendorf">
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
    using System.Threading;
    using ch.kripp.RelayCard;
    using NUnit.Framework;

    /// <summary>
    /// CardState8Relay card integration tests
    /// </summary>
    [TestFixture]
    public class Conrad8RelayCardIntegrationFixture
    {
        /// <summary>
        ///  tesstee board's address
        /// </summary>
        private const int ConstTesteeCardAddress = 1;
        
        /// <summary>
        /// Tests shall execute methods on this instance
        /// </summary>
        private readonly Conrad8RelayCard testee = new Conrad8RelayCard("/dev/ttyUSB0");

        /// <summary>
        /// Initializes the card for integration test.
        /// </summary>
        [TestFixtureSetUp]
        public void InitCardTest()
        {
            // act
            this.testee.InitializeCard();

            // assert
            Assert.AreEqual(1, this.testee.DetectedCardCount);
        }

        /// <summary>
        /// Do SetPort iteration
        /// </summary>
        [Test]
        public void SetPortCommandIterationTest()
        {
            // arrange
            const int Sleep = 60;

            // act
            for (var i = 256; i > 192; i--)
            {
                var state = new CardRelayState(Conrad8RelayCard.ConstNumberOfCardPorts, ConstTesteeCardAddress);
                state.FromByte((byte)i);

                this.testee.SetPortCommand(state);
                Thread.Sleep(Sleep);
            }
        }

        /// <summary>
        /// Do SetsPort twiche and evaluate response belongs to SetsPort action
        /// </summary>
        [Test]
        public void SetPortCommandSingleTest()
        {
            // arrange
            this.testee.SetPortCommand(new CardRelayState(Conrad8RelayCard.ConstNumberOfCardPorts, ConstTesteeCardAddress, 0, 0, 0, 0, 0, 0, 0, 1));

            // act
            var res = this.testee.SetPortCommand(new CardRelayState(Conrad8RelayCard.ConstNumberOfCardPorts, ConstTesteeCardAddress, 0, 0, 0, 0, 0, 0, 1, 0));
          
            // assert
            Assert.AreEqual(Conrad8RelayCard.ResponseCommand.SetPort, res.ResponseCommand);
        }

        /// <summary>
        /// Set card to a known state and evaluate GetPort results
        /// </summary>
        [Test]
        public void GetPortCommandTest()
        {
            // arrange
            var measureRelayState = new CardRelayState(Conrad8RelayCard.ConstNumberOfCardPorts, ConstTesteeCardAddress, 1, 0, 0, 0, 0, 0, 1, 0);
            this.testee.SetPortCommand(new CardRelayState(Conrad8RelayCard.ConstNumberOfCardPorts, ConstTesteeCardAddress, 0, 0, 0, 0, 0, 0, 0, 1));
            this.testee.SetPortCommand(measureRelayState);

            // act
            var result = this.testee.GetPortCommand(ConstTesteeCardAddress);

            // assert
            Assert.AreEqual(65, result.CardResponseFrame.DataByte);
            Assert.AreEqual(measureRelayState.ToByteArray(), result.RelayState.ToByteArray());
        }

        /// <summary>
        /// Toggle each single relay
        /// </summary>
        [Test]
        public void ToggleCommandSingleTest()
        {
            // act
            for (var i = 0; i < 8; i++)
            {
                this.testee.ToggleCommand(1, i);
                Thread.Sleep(500);
                this.testee.ToggleCommand(1, i);
                Thread.Sleep(500);
            }

            // assert 
            // ...Verify events on card! 
        }

        /// <summary>
        /// Toggle a pair of 4
        /// </summary>
        [Test]
        public void ToggleCommandManyTest()
        {
            // arrange
            var zeroState = new CardRelayState(8, 1);
            zeroState.FromByte(0);
            this.testee.SetPortCommand(zeroState);
            Thread.Sleep(500);

            // act
            for (var i = 0; i < 4; i++)
            {
                this.testee.ToggleCommand(new CardRelayState(Conrad8RelayCard.ConstNumberOfCardPorts, ConstTesteeCardAddress, 1, 0, 1, 0, 1, 0, 1, 0));
                Thread.Sleep(500);
                this.testee.ToggleCommand(new CardRelayState(Conrad8RelayCard.ConstNumberOfCardPorts, ConstTesteeCardAddress, 1, 0, 1, 0, 1, 0, 1, 0));
                Thread.Sleep(500);

                this.testee.ToggleCommand(new CardRelayState(Conrad8RelayCard.ConstNumberOfCardPorts, ConstTesteeCardAddress, 0, 1, 0, 1, 0, 1, 0, 1));
                Thread.Sleep(500);
                this.testee.ToggleCommand(new CardRelayState(Conrad8RelayCard.ConstNumberOfCardPorts, ConstTesteeCardAddress, 0, 1, 0, 1, 0, 1, 0, 1));
                Thread.Sleep(500);
            }

            // assert 
            // ...Verify events on card! 
        }

        /// <summary>
        /// SetSingle command twice.
        /// </summary>
        [Test]
        public void SetSingleCommandTest()
        {
            // arrange
            var zeroState = new CardRelayState(8, 1);
            zeroState.FromByte(0);
            this.testee.SetPortCommand(zeroState);
            Thread.Sleep(500);

            // act
            this.testee.SetSingleCommand(new CardRelayState(Conrad8RelayCard.ConstNumberOfCardPorts, ConstTesteeCardAddress, 1, 0, 1, 0, 1, 0, 1, 0));

            Thread.Sleep(500);
            this.testee.SetSingleCommand(new CardRelayState(Conrad8RelayCard.ConstNumberOfCardPorts, ConstTesteeCardAddress, 0, 1, 0, 1, 0, 1, 0, 1));

            // assert 
            // ...Verify events on card! 
        }

        /// <summary>
        /// DelSingle command by switching off each relay
        /// </summary>
        [Test]
        public void DelSingleCommandTest()
        {
            // arrange
            var fullState = new CardRelayState(Conrad8RelayCard.ConstNumberOfCardPorts, ConstTesteeCardAddress);
            fullState.FromByte(255);
            this.testee.SetPortCommand(fullState);
            Thread.Sleep(500);

            // act
            for (var i = 0; i < 8; i++)
            {
                var state = new CardRelayState(Conrad8RelayCard.ConstNumberOfCardPorts, ConstTesteeCardAddress);
                state.CardState[i] = true;
                this.testee.DelSingleCommand(state);
                
                Thread.Sleep(500);
            }

            // assert 
            // ...Verify events on card! 
        }
    }
}
