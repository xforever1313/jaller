//
// Jaller - An advanced IPFS Gateway
// Copyright (C) 2025 Seth Hendrick
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using Jaller.Server.Logging;
using Jaller.Standard.Configuration;
using Serilog;
using Serilog.Sinks.Telegram.Alternative;

namespace Jaller.Server;

internal static class HostingExtensions
{
    public static Serilog.ILogger CreateLog(
        IJallerConfig config,
        Action<Exception> onTelegramFailure
    )
    {
        var logger = new LoggerConfiguration()
            // Use all levels, but each sink will
            // specify the level to use individually.
            .MinimumLevel.Verbose()
            .WriteTo.Console( config.Logging.ConsoleLogLevel.ToSerilogLevel() );

        bool useFileLogger = false;
        bool useTelegramLogger = false;

        FileInfo? logFile = config.Logging.LogFile;
        if( logFile is not null )
        {
            useFileLogger = true;
            logger.WriteTo.File(
                logFile.FullName,
                restrictedToMinimumLevel: config.Logging.LogFileLevel.ToSerilogLevel(),
                retainedFileCountLimit: 10,
                fileSizeLimitBytes: 512 * 1000 * 1000, // 512 MB
                shared: false
            );
        }

        string? telegramBotToken = config.Logging.TelegramBotToken;
        string? telegramChatId = config.Logging.TelegramChatId;
        if(
            ( string.IsNullOrWhiteSpace( telegramBotToken ) == false ) &&
            ( string.IsNullOrWhiteSpace( telegramChatId ) == false )
        )
        {
            useTelegramLogger = true;
            var telegramOptions = new TelegramSinkOptions(
                botToken: telegramBotToken,
                chatId: telegramChatId,
                dateFormat: "dd.MM.yyyy HH:mm:sszzz",
                applicationName: nameof( Jaller ),
                failureCallback: onTelegramFailure
            );
            logger.WriteTo.Telegram(
                telegramOptions,
                restrictedToMinimumLevel: config.Logging.TelegramLogLevel.ToSerilogLevel()
            );
        }

        Serilog.ILogger log = logger.CreateLogger();
        log.Information( $"Using File Logging: {useFileLogger}." );
        log.Information( $"Using Telegram Logging: {useTelegramLogger}." );

        return log;
    }
}
