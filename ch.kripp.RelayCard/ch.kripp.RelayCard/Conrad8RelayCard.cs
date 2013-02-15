// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Conrad8RelayCard.cs" company="kripp.ch GmbH, Frank Werner-Krippendorf">
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
    using System;
    using System.IO.Ports;
    using System.Threading;

    /// <summary>
    /// Utility class to interface the Conrad 8-Port Relay Card. (Conrad Item no. 197730)
    /// </summary>
    public class Conrad8RelayCard
    {
        /// <summary>
        /// The const number of card ports
        /// </summary>
        public const ushort ConstNumberOfCardPorts = 8;

        /// <summary>
        /// Serial interface dataBits
        /// </summary>
        private const ushort ConstDataBits = 8; 
        
        /// <summary>
        /// Serial interface Baud rate
        /// </summary>
        private const ushort ConstBaudRate = 19200;

        /// <summary>
        /// The port
        /// </summary>
        private readonly SerialPort port;

        /// <summary>
        /// Number of the detected cards. 
        /// </summary>
        private int detectedCardCount = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="Conrad8RelayCard"/> class.
        /// </summary>
        /// <param name="portName">Name of the serial port. "COM1", "/dev/ttyUSB0", "..." </param>
       public Conrad8RelayCard(string portName)
        {
            this.port = new SerialPort(portName, ConstBaudRate, Parity.None, ConstDataBits, StopBits.One);
        }

       /// <summary>
       /// The following commands are defined for requests - Command frame (x stands for "no significance" each time)
       /// </summary>
       public enum RequestCommand
       {
           /// <summary>
           /// no action (NOP), "NO OPERATION" (Frame: 0 - Adr. - x - XOR)
           /// </summary>
           NoOperation = 0,

           /// <summary>
           /// Initialization, "SETUP" (Frame: 1 - Adr. - x - XOR)
           /// </summary>
           Setup = 1,

           /// <summary>
           /// Query switching statuses, "GET PORT" (Frame: 2 - Adr. - x - XOR)
           /// </summary>
           GetPort = 2,

           /// <summary>
           /// Switch relay, "SET PORT" (Frame: 3 - Adr. - Data - XOR)
           /// </summary>
           SetPort = 3,

           /// <summary>
           /// Query options, "GET OPTION" (Frame: 4 - Adr. - x - XOR)
           /// </summary>
           GetOption = 4,

           /// <summary>
           /// Set options, "SET OPTION" (Frame: 5 - Adr. - Opt. - XOR)
           /// </summary>
           SetOption = 5,

           /// <summary>
           /// Switch on relay without changing the other outputs, "SET SINGLE" (Frame: 6 - Adr. - Data - XOR)
           /// </summary>
           SetSingle = 6,

           /// <summary>
           /// Switch off relay without changing the other outputs, "DEL SINGLE" (Frame: 7 - Adr. - Data - XOR)
           /// </summary>
           DelSingle = 7,

           /// <summary>
           /// Changing the switching status without changing the remaining outputs, "TOGGLE" (Frame: 8 - Adr. - Data - XOR)
           /// </summary>
           Toggle = 8
       }

       /// <summary>
       /// The following commands are defined - Response frame (x stands for "no significance" each time):
       /// inverted command ID - own address - data - new checksum
       /// </summary>
       public enum ResponseCommand
       {
           /// <summary>
           /// no action (NOP) response, "NO OPERATION" (Frame: 255 - Adr. - x - XOR)
           /// </summary>
           NoOperation = 255,

           /// <summary>
           /// Initialization response, "SETUP" (Frame: 254 - Adr. - Info - XOR)
           /// </summary>
           Setup = 254,

           /// <summary>
           /// Query switching statuses response, "GET PORT" (Frame: 253 - Adr. - Data - XOR)
           /// </summary>
           GetPort = 253,

           /// <summary>
           /// Switch relay response, "SET PORT" (Frame: 252 - Adr. - x - XOR)
           /// </summary>
           SetPort = 252,

           /// <summary>
           /// Query options response, "GET OPTION" (Frame: 251 - Adr. - Opt. - XOR)
           /// </summary>
           GetOption = 251,

           /// <summary>
           /// Set options response, "SET OPTION" (Frame: 250 - Adr. - x - XOR)
           /// </summary>
           SetOption = 250,

           /// <summary>
           /// Switch on relay without changing the other outputs response, "SET SINGLE" (Frame: 249 - Adr. - Data - XOR)
           /// </summary>
           SetSingle = 249,

           /// <summary>
           /// Switch off relay without changing the other outputs, "DEL SINGLE" (Frame: 248 - Adr. - Data - XOR)
           /// </summary>
           DelSingle = 248,

           /// <summary>
           /// Changing the switching status without changing the remaining outputs, "TOGGLE" (Frame: 247 - Adr. - Data - XOR)
           /// </summary>
           Toggle = 247
       }

       /// <summary>
       /// Gets a value indicating whether the card has been initialized by this instance.
       /// </summary>
       /// <value>
       /// <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
       /// </value>
       public bool IsInitialized { get; private set; }

       /// <summary>
       /// Gets the number of detected cards.
       /// </summary>
       /// <value>
       /// The detected card count.
       /// </value>
       public int DetectedCardCount
       {
           get { return this.detectedCardCount; }
       }

       /// <summary>
       /// Initializes the card.
       /// From Manual: 
       ///  Command 1 - Initialization
       ///  The first relay board contains the address sent in the frame as "Adr.".
       ///  The response frame provides information about the version of the microcontroller software.
       ///  After sending the response frame, the controller generates an initialization command with an
       ///  address increased by 1 and passes this on to the following board (or returns it to the control
       ///  computer in case of single operation). Therefore, the control computer receives N+1 response
       ///  frames with N connected relay boards.
       /// </summary>
       /// <returns>The number of detected cards</returns>
        public int InitializeCard()
        {
            lock (this.port)
            {
                if (!this.port.IsOpen)
                {
                    this.port.Open();
                }

                this.port.DiscardInBuffer();

                /* Initialize cards*/ 
                for (var x = 0; x < 4; x++)
                {
                    this.port.Write(new byte[] { (int)RequestCommand.Setup }, 0, 1);
                    Thread.Sleep(5);   
                
                    if (this.port.BytesToRead > 3)
                    {
                        break;
                    }
                }

                /* wait for the card's response*/ 
                Thread.Sleep(1023);

                /* Sync cards */ 
                for (var x = 0; x < 4; x++)
                {
                    this.port.Write(new byte[] { 1 }, 0, 1);
                }

                /* wait for the card's response*/ 
                Thread.Sleep(1023);

                /* holds the init response*/ 
                var initBufferIncoming = new byte[4];
                
                /* evaluate the number of available cars*/ 
                for (var i = 0; i < 256; i++)
                {
                    this.port.Read(initBufferIncoming, 0, 4);

                    // is initialize/setup response? 
                    if (initBufferIncoming[0] == 1)
                    {
                        /* Evaluate Data byte, who has 255 cards? Schould return 0 if so */ 
                        if (initBufferIncoming[1] == 0)
                        {
                            this.detectedCardCount = 255;
                        }
                        else if (initBufferIncoming[1] > 0)
                        {
                            /* no. of relay cards: ddressbyte of last feedback-frame  - 1 */ 
                            this.detectedCardCount = initBufferIncoming[1] - 1;
                        }

                        break;
                    }
                }
            }
            
            // clean-up and close port
            this.port.DiscardInBuffer();
            this.port.DiscardOutBuffer();
            this.port.Close();

            // mark as initializes and close
            this.IsInitialized = this.detectedCardCount >= 1; 
            return this.detectedCardCount;
        }

        /// <summary>
        /// Transmit a "SetPort" Command
        /// </summary>
        /// <param name="newState">The new state.</param>
        /// <returns>card response frame</returns>
        public Conrad8RelayCardResponseFrame SetPortCommand(CardRelayState newState)
        {
            // submit command
            return SendCommandToCard(this.port, (int)RequestCommand.SetPort, (byte)newState.CardAddress, newState.ToByteArray()[0]);
        }

        /// <summary>
        /// Transmit a "GetPort" Command
        /// </summary>
        /// <param name="cardAddress">The card address.</param>
        /// <returns>card response frame</returns>
        public RelayCardStateResponse GetPortCommand(int cardAddress)
        {
            var result = new RelayCardStateResponse
                             {
                                 CardResponseFrame =
                                     SendCommandToCard(
                                         this.port,
                                         (int)RequestCommand.GetPort,
                                         (byte)cardAddress,
                                         0)
                             };

            result.RelayState = new CardRelayState(ConstNumberOfCardPorts, result.CardResponseFrame.AddressByte); 
            result.RelayState.FromByte(result.CardResponseFrame.DataByte);

            return result;
        }

        /// <summary>
        /// Transmit a "Toggle" command
        /// </summary>
        /// <param name="address">The card address.</param>
        /// <param name="portNumber">The port(relay) number.</param>
        /// <returns>relay card state</returns>
        public RelayCardStateResponse ToggleCommand(int address, int portNumber)
        {
            var toggleState = new CardRelayState(ConstNumberOfCardPorts, address);
                toggleState.CardState[portNumber] = true;

                return this.ToggleCommand(toggleState);
        }

        /// <summary>
        /// Transmit a "Toggle" command
        /// </summary>
        /// <param name="toggleState">Card state to be toggled.</param>
        /// <returns>relay card state</returns>
        public RelayCardStateResponse ToggleCommand(CardRelayState toggleState)
        {
            var result = new RelayCardStateResponse
                                {
                                    CardResponseFrame = SendCommandToCard(
                                                                            this.port,
                                                                            (int)RequestCommand.Toggle,
                                                                            (byte)toggleState.CardAddress,
                                                                            toggleState.ToByteArray()[0])
                                };

            result.RelayState = new CardRelayState(ConstNumberOfCardPorts, result.CardResponseFrame.AddressByte);
            result.RelayState.FromByte(result.CardResponseFrame.DataByte);

            return result;
        }

        /// <summary>
        /// Transmit a "SetSingle" command
        /// </summary>
        /// <param name="setSingleState">State of the set single.</param>
        /// <returns>relay card state</returns>
        public RelayCardStateResponse SetSingleCommand(CardRelayState setSingleState)
        {
            var result = new RelayCardStateResponse
            {
                CardResponseFrame = SendCommandToCard(
                                                        this.port,
                                                        (int)RequestCommand.SetSingle, 
                                                        (byte)setSingleState.CardAddress,
                                                        setSingleState.ToByteArray()[0])
            };

            result.RelayState = new CardRelayState(ConstNumberOfCardPorts, result.CardResponseFrame.AddressByte);
            result.RelayState.FromByte(result.CardResponseFrame.DataByte);

            return result;
        }

        /// <summary>
        /// Transmit a "SetSingle" command
        /// </summary>
        /// <param name="delSingleState">State of the set single.</param>
        /// <returns>relay card state</returns>
        public RelayCardStateResponse DelSingleCommand(CardRelayState delSingleState)
        {
            var result = new RelayCardStateResponse
            {
                CardResponseFrame = SendCommandToCard(
                                                        this.port,
                                                        (int)RequestCommand.DelSingle,
                                                        (byte)delSingleState.CardAddress,
                                                        delSingleState.ToByteArray()[0])
            };

            result.RelayState = new CardRelayState(ConstNumberOfCardPorts, result.CardResponseFrame.AddressByte);
            result.RelayState.FromByte(result.CardResponseFrame.DataByte);

            return result;
        }

        /// <summary>
        /// Sends a comand frame to the card
        /// </summary>
        /// <param name="serialPort">The serial port.</param>
        /// <param name="commandByte">The command byte.</param>
        /// <param name="addressByte">The address byte.</param>
        /// <param name="dataByte">The data byte.</param>
        /// <returns>A single response command frame</returns>
        private static Conrad8RelayCardResponseFrame SendCommandToCard(SerialPort serialPort, byte commandByte, byte addressByte, byte dataByte)
        {
            lock (serialPort)
            {
                serialPort.Open();

                // clean up
                serialPort.DiscardOutBuffer();

                /* calculate command frame*/
                var commandFrame = CalculateCommandFrame(commandByte, addressByte, dataByte);

                /* transmit command */
                serialPort.Write(commandFrame, 0, 4);

                /* read and return card response frame */
                var result = ReadCardResponseFrame(serialPort);

                serialPort.Close();

                return result;
            }
        }

        /// <summary>
        /// Calculates the command frame incl. CRC.
        /// From Manual:
        ///     * Frame structure * 
        ///     Byte 0 Command
        ///     Byte 1 Board address
        ///     Byte 2 Data
        ///     Byte 3 Check sum (XOR from Byte0, Byte1 and Byte2 )
        /// </summary>
        /// <param name="commandByte">The command byte.</param>
        /// <param name="addressByte">The address byte.</param>
        /// <param name="dataByte">The data byte.</param>
        /// <returns>Byte array of 4 bytes</returns>
        private static byte[] CalculateCommandFrame(byte commandByte, byte addressByte, byte dataByte)
        {
            var result = new byte[4];
            result[0] = commandByte;
            result[1] = addressByte;
            result[2] = dataByte;
            result[3] = (byte)(commandByte ^ addressByte ^ dataByte);
            return result;
        }

        /// <summary>
        /// Reads one card response frame
        /// </summary>
        /// <param name="serialPort">The serial port.</param>
        /// <returns>A single response command frame</returns>
        private static Conrad8RelayCardResponseFrame ReadCardResponseFrame(SerialPort serialPort)
        {
            Thread.Sleep(20);

            var inBuffer = new byte[4];
            serialPort.Read(inBuffer, 0, 4);
            var result = new Conrad8RelayCardResponseFrame
            {
                ResponseCommand = (ResponseCommand)Enum.Parse(typeof(ResponseCommand), Convert.ToInt16(inBuffer[0]).ToString()),
                AddressByte = inBuffer[1],
                DataByte = inBuffer[2],
                CrcByte = inBuffer[3]
            };

            // clean up
            serialPort.DiscardInBuffer();

            return result;
        }
    }
}
