﻿using Lynx.Internal;
using Lynx.Model;
using Lynx.UCI.Commands.Engine;
using Lynx.UCI.Commands.GUI;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Lynx
{
    public class LinxDriver
    {
        private readonly ChannelReader<string> _uciReader;
        private readonly Channel<string> _engineWriter;
        private readonly Engine _engine;

        public LinxDriver(ChannelReader<string> uciReader, Channel<string> engineWriter, Engine engine)
        {
            _uciReader = uciReader;
            _engineWriter = engineWriter;
            _engine = engine;
            _engine.OnReady += NotifyReadyOK;
            _engine.OnSearchFinished += NotifyBestMove;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            while (await _uciReader.WaitToReadAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
            {
                if (_uciReader.TryRead(out var input) && !string.IsNullOrWhiteSpace(input))
                {
                    await HandleCommand(input, cancellationToken);
                }
            }

            Console.WriteLine($"Finishing {nameof(LinxDriver)}");
        }

        private async Task HandleCommand(string rawCommand, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[GUI]\t{rawCommand}");

            var commandItems = rawCommand.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            switch (commandItems[0].ToLowerInvariant())
            {
                case DebugCommand.Id:
                    Configuration.IsDebug = DebugCommand.Parse(rawCommand);
                    break;
                case GoCommand.Id:
                    var goCommand = new GoCommand();
                    await goCommand.Parse(rawCommand);
                    _engine.StartSearching(goCommand);
                    break;
                case IsReadyCommand.Id:
                    if (_engine.IsReady)
                    {
                        await SendCommand(ReadyOKCommand.Id, cancellationToken);
                    }
                    break;
                case PonderHitCommand.Id:
                    if (Configuration.IsPonder)
                    {
                        _engine.PonderHit();
                    }
                    break;
                case PositionCommand.Id:
                    _engine.AdjustPosition(rawCommand);
                    break;
                case QuitCommand.Id:
                    _engineWriter.Writer.Complete();
                    break;
                case RegisterCommand.Id:
                    _engine.Registration = new RegisterCommand(rawCommand);
                    break;
                case SetOptionCommand.Id:
                    switch (commandItems[1].ToLowerInvariant())
                    {
                        case "Ponder":
                            {
                                if (bool.TryParse(commandItems[3], out var value))
                                {
                                    Configuration.IsPonder = value;
                                }
                                break;
                            }
                        case "UCI_AnalyseMode":
                            {
                                if (bool.TryParse(commandItems[3], out var value))
                                {
                                    Configuration.UCI_AnalyseMode = value;
                                }
                                break;
                            }
                        default:
                            Logger.Warn($"Unsupported option: {rawCommand}");
                            break;
                    }
                    break;
                case StopCommand.Id:
                    _engine.StopSearching();
                    await NotifyBestMove(_engine.BestMove(), _engine.MoveToPonder());
                    break;
                case UCICommand.Id:
                    await SendCommand(IdCommand.Name, cancellationToken);
                    await SendCommand(IdCommand.Version, cancellationToken);

                    foreach (var availableOption in OptionCommand.AvailableOptions)
                    {
                        await SendCommand(availableOption, cancellationToken);
                    }
                    break;
                case UCINewGameCommand.Id:
                    _engine.NewGame();
                    break;

                default:
                    Logger.Warn($"Unknown command received: {rawCommand}");
                    break;
            }
        }

        private async Task NotifyReadyOK()
        {
            await _engineWriter.Writer.WriteAsync(ReadyOKCommand.Id);
        }

        private async Task NotifyBestMove(Move move, Move? moveToPonder)
        {
            await _engineWriter.Writer.WriteAsync(BestMoveCommand.BestMove(move, moveToPonder));
        }

        private async Task SendCommand(string command, CancellationToken cancellationToken)
        {
            await _engineWriter.Writer.WriteAsync(command, cancellationToken);
        }
    }
}
