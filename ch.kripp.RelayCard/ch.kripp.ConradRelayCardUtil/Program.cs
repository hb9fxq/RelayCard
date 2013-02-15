// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="kripp.ch GmbH, Frank Werner-Krippendorf">
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
namespace ch.kripp.ConradRelayCardUtil
{
    using System;
    using System.Reflection;
    using System.Threading;

    using ch.kripp.RelayCard;
    using CommandLine;
    using CommandLine.Text;

    using log4net;
    using log4net.Core;
    using log4net.Repository.Hierarchy;

    /// <summary>
    /// Utilizes the methods provided by ch.kripp.ConradRelayCardUtil.dll 
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The runtime options, parsed from args
        /// </summary>
        private static readonly Options RuntimeOptions = new Options();

        /// <summary>
        /// The console logger
        /// </summary>
        private static ILog consoleLog;

        /// <summary>
        /// The card
        /// </summary>
        private static Conrad8RelayCard conradCard; 

        /// <summary>
        /// available option commands
        /// </summary>
        public enum CommandOption
        {
            /// <summary>
            /// SetPort card-wide
            /// </summary>
            Setport,

            /// <summary>
            /// GetPort card-wide
            /// </summary>
            Getport,
            
            /// <summary>
            /// Hardware test / command simulation
            /// </summary>
            HardwareTest,

            /// <summary>
            /// Switch on single relay
            /// </summary>
            SetSingle,

            /// <summary>
            /// Switch off single relay
            /// </summary>
            DelSingle,

            /// <summary>
            /// toggle card-wide
            /// </summary>
            Toggle
        }

        /// <summary>
        /// Application's Entry Point.
        /// </summary>
        /// <param name="args">See usage (--help).</param>
        protected static void Main(string[] args)
        {
            /* configure loggging */ 
            var consoleAppender = new log4net.Appender.ConsoleAppender { Layout = new log4net.Layout.SimpleLayout() };
            log4net.Config.BasicConfigurator.Configure(consoleAppender);
            var rootLogger = ((Hierarchy)LogManager.GetRepository()).Root;
            rootLogger.Level = Level.Info;
            
            consoleLog = LogManager.GetLogger("ConradRelayCardUtil");
            
            /* parse RuntimeOptions */ 
            if (!Parser.Default.ParseArguments(args, RuntimeOptions))
            {
                return;
            }

            if (RuntimeOptions.OptionDebug)
            {
                rootLogger.Level = Level.Debug;
            }

            /* create card instance and initialize with serial port from RuntimeOptions*/ 
            consoleLog.DebugFormat("Setting card port to {0}", RuntimeOptions.SerialPortName);
            conradCard = new Conrad8RelayCard(RuntimeOptions.SerialPortName);

            /* init card if not DoNotInit*/ 
            if (!RuntimeOptions.OptionDoNotInit)
            {
                try
                {
                    consoleLog.Debug("Initializing card(s)");
                    var numberOfCards = conradCard.InitializeCard(); 
                    consoleLog.InfoFormat("Found ({0}) card(s)", numberOfCards);
                }
                catch (Exception ex)
                {
                    consoleLog.Error("Could not initialize card(s)", ex);
                    return;
                }
            }
            else
            {
                consoleLog.Debug("Option 'DoNotInit'has been set, card not initialized");
            }
            
            switch (RuntimeOptions.CommandOption)
            {
                case CommandOption.Getport:
                    GetPort();
                    break;
                
                case CommandOption.Setport:
                    SetPort();
                    break;
                case CommandOption.HardwareTest:
                    HardwareTest();
                    break;
                case CommandOption.SetSingle:
                    SetSingle();
                    break;
                case CommandOption.DelSingle:
                    DelSingle();
                    break;
                case CommandOption.Toggle:
                    Toggle();
                    break;
                default:
                    Console.WriteLine("Invalid command or not implemented.");
                    break;
            }
        }

        /// <summary>
        /// Executes a ToggleCommand on the card
        /// </summary>
        private static void Toggle()
        {
            consoleLog.Debug("Command: Toggle");
            var address = Convert.ToInt16(RuntimeOptions.CommandMask.Split(';')[0]);
            var portPattern = Convert.ToByte(RuntimeOptions.CommandMask.Split(';')[1], 2);
            consoleLog.Debug("Toggle address param is: " + address + ", data byte: " + portPattern.ToString());

            var state = new CardRelayState(Conrad8RelayCard.ConstNumberOfCardPorts, address);
            state.FromByte(portPattern);
            var res = conradCard.ToggleCommand(state);
            consoleLog.Info("Result frame: " + res.CardResponseFrame);
            consoleLog.Info("Result state: " + res.RelayState);
        }

        /// <summary>
        /// Executes a DelSingleCommand on the card
        /// </summary>
        private static void DelSingle()
        {
            consoleLog.Debug("Command: DelSingle");
            var address = Convert.ToInt16(RuntimeOptions.CommandMask.Split(';')[0]);
            var portPattern = Convert.ToByte(RuntimeOptions.CommandMask.Split(';')[1], 2);
            consoleLog.Debug("DelSingle address param is: " + address + ", data byte: " + portPattern.ToString());

            var state = new CardRelayState(Conrad8RelayCard.ConstNumberOfCardPorts, address);
            state.FromByte(portPattern);
            var res = conradCard.DelSingleCommand(state);
            consoleLog.Info("Result frame: " + res.CardResponseFrame);
            consoleLog.Info("Result state: " + res.RelayState);
        }

        /// <summary>
        /// Executes a SetSingle on the card
        /// </summary>
        private static void SetSingle()
        {
            consoleLog.Debug("Command: SetSingle");
            var address = Convert.ToInt16(RuntimeOptions.CommandMask.Split(';')[0]);
            var portPattern = Convert.ToByte(RuntimeOptions.CommandMask.Split(';')[1], 2);
            consoleLog.Debug("SetSingle address param is: " + address + ", data byte: " + portPattern.ToString());

            var state = new CardRelayState(Conrad8RelayCard.ConstNumberOfCardPorts, address);
            state.FromByte(portPattern);
            var res = conradCard.SetSingleCommand(state);
            consoleLog.Info("Result frame: " + res.CardResponseFrame);
            consoleLog.Info("Result state: " + res.RelayState);
        }

        /// <summary>
        /// Executes a hardware test
        /// </summary>
        private static void HardwareTest()
        {
            HardwareTestSwitchSingleValue(1, 255, 1000);
            HardwareTestSwitchSingleValue(1, 0, 500);

            uint value = 1;

            while (value <= 128)
            {
                HardwareTestSwitchSingleValue(1, (byte)value, 250);
                value *= 2; 
            }

            value = 64; 

            while (value >= 1)
            {
                HardwareTestSwitchSingleValue(1, (byte)value, 250);
                value /= 2;
            }

            HardwareTestSwitchSingleValue(1, 255, 1000);
            HardwareTestSwitchSingleValue(1, 0, 20);
        }

        /// <summary>
        /// Single switch command issued by setPort command
        /// </summary>
        /// <param name="addr">The card addr.</param>
        /// <param name="value">The data byte.</param>
        /// <param name="holdTime">The hold time in ms.</param>
        private static void HardwareTestSwitchSingleValue(int addr, byte value, int holdTime)
        {
            var newState = new CardRelayState(Conrad8RelayCard.ConstNumberOfCardPorts, addr);
            consoleLog.Debug("Hardwaretest, newState: " + value);
            newState.FromByte(value);
            var res = conradCard.SetPortCommand(newState);
            consoleLog.Info("Result frame: " + res);
            Thread.Sleep(holdTime - 20);
        }

        /// <summary>
        /// Executes a GetPortCommand on the card
        /// </summary>
        private static void GetPort()
        {
            consoleLog.Debug("Command: GetPort");
            var address = Convert.ToInt16(RuntimeOptions.CommandMask);
            consoleLog.Debug("GetPort address param is: " + address);
            var res = conradCard.GetPortCommand(address);
            consoleLog.Info("Result frame: " + res.CardResponseFrame); 
            consoleLog.Info("Result state: " + res.RelayState); 
        }

        /// <summary>
        /// Executes a SetPortCommand on the card
        /// </summary>
        private static void SetPort()
        {
            consoleLog.Debug("Command: SetPort");
            var address = Convert.ToInt16(RuntimeOptions.CommandMask.Split(';')[0]);
            var portPattern = Convert.ToByte(RuntimeOptions.CommandMask.Split(';')[1], 2);
            consoleLog.Debug("SetPort address param is: " + address + ", data byte: " + portPattern.ToString());

            var state = new CardRelayState(Conrad8RelayCard.ConstNumberOfCardPorts, address);
            state.FromByte(portPattern);
            var res = conradCard.SetPortCommand(state);
            consoleLog.Info("Result frame: " + res);
        }

        /// <summary>
        /// Command line RuntimeOptions
        /// </summary>
        protected class Options
        {
            /// <summary>
            /// Gets or sets the command option.
            /// </summary>
            /// <value>
            /// The command option.
            /// </value>
            [Option('c', "command", Required = true)]
            public CommandOption CommandOption { get; set; }

            /// <summary>
            /// Gets or sets the SerialPortName option.
            /// </summary>
            [Option('p', "serial port name", Required = true)]
            public string SerialPortName { get; set; }

            /// <summary>
            /// Gets or sets the command mask option.
            /// </summary>
            [Option('m', "mask")]
            public string CommandMask { get; set; }
            
            /// <summary>
            /// Gets or sets a value indicating whether the card should be initialized.
            /// </summary>
            [Option('n', "noinit", HelpText = "do not initialize card, use this, if card has already been initialized since power on. Not well testet how the card behaves when powerd on long time!")]
            public bool OptionDoNotInit { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the program should run in debug mode.
            /// </summary>
            [Option('d', "debug", HelpText = "print debug messages")]
            public bool OptionDebug { get; set; }

            /// <summary>
            /// Gets the usage.
            /// </summary>
            /// <returns>Stirng with usage instructions</returns>
            [HelpOption]
            public string GetUsage()
            {
                var help = new HelpText
                {
                    Heading = new HeadingInfo(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString()),
                    Copyright = new CopyrightInfo("kripp.ch GmbH, Frank Werner-Krippendorf", 2013),
                    AdditionalNewLineAfterOption = true,
                    AddDashesToOption = true
                };
                
                help.AddPreOptionsLine(Environment.NewLine + "THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE." + Environment.NewLine);
                help.AddPreOptionsLine("Usage: " + Assembly.GetExecutingAssembly().GetName().Name + " -c [COMMAND] (-m [MASK] -s -n)");
                help.AddPreOptionsLine("Available CommandOption (-c): ");
                help.AddPreOptionsLine(string.Empty);

                help.AddPreOptionsLine("mask parameter (-m): a string, containing the card's address (first card is 1) and the 8-bit binary state");
                help.AddPreOptionsLine("e.g. for adressing relay K6, relay K5 and relay K1, one byte is 49 (binary/8-bit mask =  '00110001').");
                help.AddPreOptionsLine("for read only commands like GETPORT the mask only needs to contain the address of the card.");
                help.AddPreOptionsLine(string.Empty);

                help.AddPostOptionsLine("Available values for the command option: ");
                help.AddPostOptionsLine(string.Empty);
                help.AddPostOptionsLine("-c SETPORT: Set ports by mask, requires address and 8-bit mask");
                help.AddPostOptionsLine(string.Empty);
                help.AddPostOptionsLine("-c GETPORT: Get ports for address, requires address in mask parameter");
                help.AddPostOptionsLine(string.Empty);
                help.AddPostOptionsLine("-c SETSINGLE: Switch on relay without changing the other outputs, requires address and 8-bit mask");
                help.AddPostOptionsLine(string.Empty);
                help.AddPostOptionsLine("-c DELSINGLE: Switch off relay without changing the other outputs, requires address and 8-bit mask");
                help.AddPostOptionsLine(string.Empty);
                help.AddPostOptionsLine("-c TOGGLE: Changing the switching status without changing the remaining outputs, requires address and 8-bit mask");
                help.AddPostOptionsLine(string.Empty);

                help.AddPostOptionsLine("-c HARDWARETEST: Switches all relays one-by-one, requires address in mask parameter");

                help.AddPostOptionsLine(string.Empty); 

                help.AddOptions(this);

                return help;
            }
        }
    }
}