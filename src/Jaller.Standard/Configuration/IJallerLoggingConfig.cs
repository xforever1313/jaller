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

using Jaller.Standard.Logging;

namespace Jaller.Standard.Configuration;

public interface IJallerLoggingConfig
{
    /// <summary>
    /// The minimum log level that gets logged to the console.
    /// </summary>
    JallerLogLevel ConsoleLogLevel { get; }

    /// <summary>
    /// File to write log messages to.
    /// Set to null to disable logging messages to a file.
    /// </summary>
    FileInfo? LogFile { get; }

    /// <summary>
    /// The minimum log level that gets logged to the log file specified
    /// in <see cref="LogFile"/>.  Ignored if <see cref="LogFile"/> is null.
    /// </summary>
    JallerLogLevel LogFileLevel { get; }

    /// <summary>
    /// The Telegram bot token to use for logging messages.  See more information
    /// on how to do that:
    /// https://docs.teleirc.com/en/latest/user/quick-start/#create-a-telegram-bot
    ///
    /// Set to null to not log to Telegram.
    /// </summary>
    string? TelegramBotToken { get; }

    /// <summary>
    /// The chat ID of the Telegram chat to log to.
    ///
    /// Set to null to not log to Telegram.
    /// </summary>
    string? TelegramChatId { get; }

    /// <summary>
    /// The minimum log level that gets logged to Telegram.  Ignored
    /// if <see cref="TelegramBotToken"/> or <see cref="TelegramChatId"/>
    /// is null.
    /// </summary>
    JallerLogLevel TelegramLogLevel { get; }
}
