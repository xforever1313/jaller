// Sample Jaller Config file.
// This uses C# syntax, and is compiled once at the startup of Jaller.
// This means you have all the power of C# to make your configuration file.

// If you like environment variables, you should be able to get values out of them
// via C#'s Environment.GetEnvironmentVariable( string ) method.

// There is no need to add a "using" statement at the top.  That should
// be taken care of by Jaller

// ---------------- File Data Database Settings ----------------

// These settings configure the database where all file data for Jaller is stored.

// Location of were the Jaller database should created.
// Comment this out to use the default location of your user's
// application data directory, which is
// c:\Users\<you>\AppData\Jaller\jaller.ldb on Windows
// or /home/<you>/.config/Jaller/jaller.ldb on Unix systems.
this.Database.DatabaseLocation = new FileInfo( @"/var/jaller/jaller.ldb" );

// Set this to true unless you need the database to be shared
// by multiple applications (most folks should leave this set to true).
this.Database.DirectConnection = true;

// If the last close database exception results in an invalid data state,
// the data file will be rebuild on the next open.
//
// Should probably leave this to false unless your database gets corrupted for some
// reason and you need to try to recover it.
this.Database.AutoRebuild = false;

// Check if datafile is of an older version and upgrade it before opening.
// This should really only be set to true if instructed to in the release notes
// when upgrading releases.
this.Database.AutoUpgradeDb = false;

// Set to a non-null string value ("Some Value in Quotes") to encrypt the datafile
// with this password.  This uses AES encryption.
this.Database.EncryptionPassword = null;

// ---------------- IPFS Network Configuration ----------------

// The URL to Kubo (or something that implements the IPFS protocol).
// This URL should be set to the RPC API (usually port 5001).
//
// NOTE: If you do not want users to be able to download files, set this value to null.
// This will make Jaller an "IPFS Hash" database only, where users can search for
// hashes but can not download anything.
this.Ipfs.KuboUrl = new Uri( "http://localhost:5001" );

// How much to multiply the default timeout when downloading something from the IPFS gateway
// in the event the network between Jaller and Kubo is too slow.
// If this is set to 0, this makes an infinite timeout (not recommended).
this.Ipfs.TimeoutMultiplier = 2;

// ---------------- Logging ----------------

// For any setting that sets a Log Level, possible values are:
// - JallerLogLevel.Verbose
// - JallerLogLevel.Debug
// - JallerLogLevel.Information
// - JallerLogLevel.Warning
// - JallerLogLevel.Error
// - JallerLogLevel.Fatal
//
// Verbose having the most logging (and more space taken up),
// while Fatal has the least amount of logging.

// The minimum log level that gets logged to the console.
this.Logging.ConsoleLogLevel = JallerLogLevel.Information;

// If it is desired to log to a file, set this variable to a file path.
// Comment out or set to null to not log to a file.
this.Logging.LogFile = new FileInfo( "/var/jaller/jaller.log" );

// The minimum log level that gets logged to the file.  Ignored
// if LogFile is null or not set.
this.Logging.LogFileLevel = JallerLogLevel.Information;

// The Telegram bot token to use for logging messages.  See more information
// on how to do that:
// https://docs.teleirc.com/en/latest/user/quick-start/#create-a-telegram-bot
//
// If set to null or commented out, log messages are not sent to Telegram.
this.Logging.TelegramBotToken = null;

// The Telegram chat ID to use for logging messages.
// If set to null or commented out, log messages are not sent to Telegram.
this.Logging.TelegramChatId = null;

// The minimum log level that gets logged to Telegram.  Ignored
// if either of the Telegram settings are null, not specified, or commented out.
this.Logging.TelegramLogLevel = JallerLogLevel.Warning;

// ---------------- Search Settings ----------------

// These settings configure the search index of Jaller.
// The search cache is stored in a separate database in case a user wants
// to backup the user and file data, but does not care about the search data.

// Location of were the Jaller search cache should created.
// Comment this out to use the default location of your user's
// application data directory, which is
// c:\Users\<you>\AppData\Jaller\jaller_search_cache.ldb on Windows
// or /home/<you>/.config/Jaller/jaller_search_cache.ldb on Unix systems.
this.Search.DatabaseLocation = new FileInfo( @"/var/jaller/jaller_search_cache.ldb" );

// If set to true, this will update the search index right when
// Jaller starts up.  However, this may delay the app from fully starting up.
// Set to false to not have have the search index at startup; but this may
// mean that the search data may be out-of-date.
this.Search.UpdateIndexOnStartup = true;

// A cron-string on often to update the search index.
// 
// See https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html#cron-expressions
// for more information on how to make this string.
//
// Defaulted to every day at midnight.
this.Search.IndexUpdateRate = "0 0 0 * * ?";

// Set this to true unless you need the search cache to be shared
// by multiple applications (most folks should leave this set to true).
this.Search.DirectConnection = true;

// If the last close database exception results in an invalid data state,
// the data file will be rebuild on the next open.
//
// Should probably leave this to false unless your search cache gets corrupted for some
// reason and you need to try to recover it.
this.Search.AutoRebuild = false;

// Check if search cache is of an older version and upgrade it before opening.
// This should really only be set to true if instructed to in the release notes
// when upgrading releases.
this.Search.AutoUpgradeDb = false;

// Set to a non-null string value ("Some Value in Quotes") to encrypt the search cache
// with this password.  This uses AES encryption.
this.Search.EncryptionPassword = null;

// ---------------- Users ----------------

// These settings configure the user database of Jaller.
// User settings are purposefully separated from data
// in case an admin wants to encrypt a user database, but doesn't care to encrypt the
// data database.

// Set to true to allow for anyone to register as a user to this instance of Jaller.
// Really not recommended on public-facing instances unless you're feeling brave.
this.Users.AllowPublicRegistration = false;

// Set to true to allow a default "admin" user.
// Set to false to disable the default "admin" user.
// This should only really be set to true upon first boot
// when there are no users; or you need an admin user if all other
// admin accounts got deleted somehow.
// The admin user is re-created every time the app starts up.
//
// If this is enabled, you can login with the "admin" user, using
// the password specified below.
this.Users.AllowAdminUser = false;

// The administrator user's email.  This is used to login into the admin account.
// Ignored if admin user is disabled.
// Careful, if this happens to match an existing user, that user will be wiped
// and given admin access.
this.Users.AdminEmail = "admin@example.com";

// Please change this to something else if the admin user is enabled.
// Ignored if the admin user is disabled.
this.Users.AdminPassword = "Jaller@dm1nPassword";

// Location of were the Jaller user database should created.
// Comment this out to use the default location of your user's
// application data directory, which is
// c:\Users\<you>\AppData\Jaller\jaller_search_cache.ldb on Windows
// or /home/<you>/.config/Jaller/jaller_search_cache.ldb on Unix systems.
this.Users.DatabaseLocation = new FileInfo( @"/var/jaller/jaller_users.ldb" );

// Set this to true unless you need the search cache to be shared
// by multiple applications (most folks should leave this set to true).
this.Users.DirectConnection = true;

// If the last close database exception results in an invalid data state,
// the data file will be rebuild on the next open.
//
// Should probably leave this to false unless your user database gets corrupted for some
// reason and you need to try to recover it.
this.Users.AutoRebuild = false;

// Check if the user database is of an older version and upgrade it before opening.
// This should really only be set to true if instructed to in the release notes
// when upgrading releases.
this.Users.AutoUpgradeDb = false;

// Set to a non-null string value ("Some Value in Quotes") to encrypt the user database
// with this password.  This uses AES encryption.
this.Users.EncryptionPassword = null;

// ---------------- Web Configuration ----------------

// If set to true, this will have metrics that can be scraped by Prometheus
// on /Metrics.
// Note, that metrics will include counts of files and directories marked private.
// Use the "MetricsUrlPrefixes" settings to restrict which URLs can scrape
// for metrics.
this.Web.EnableMetrics = false;

// If this is set to false, a URL that contains a port number will
// be rejected.  This should generally speaking be set to true if running in a
// development environment or if behind a reverse proxy.
//
// The only reason why this setting exists is because if this is run on DreamHost
// using a Proxy, one can get access to the application either by
// going to the Proxy URL (the correct way), or just going to the PORT
// Jaller is running on (the wrong way).  Setting this to false
// only allows the Proxy URL to work.
this.Web.AllowPortsInUrl = true;

// List of endpoints Jaller will allow requests to come through.
// See for more information:
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/endpoints?view=aspnetcore-8.0
//
// Set to null to not set the setting.  This may be useful if you want to use Environment
// Variables or a JSON file instead to set this setting.
this.Web.AspNetCoreUrls = ["http://localhost:9253" ];

// If set to true, if the requested URL that contains "//" this will
// rewrite the URL so each "//" becomes "/" instead.
//
// Should probably be left alone and stay set to false.
// The only reason this exists is because if running this on DreamHost,
// DreamHost adds an extra '/' after the tld for some reason
// (So if the desired URL is https://somewhere.com/Admin/, DreamHost would have
// written the URL has https://somewhere.com//Admin/ when forwarding the request
// to Jaller for whatever reason).
this.Web.RewriteDoubleSlashes = false;

// A list of allowed hosts that can access the admin pages.  If someone tries to login
// to the admin interface and the request is not to one of these URLs,
// they will not be allowed in.
//
// The use case is if Jaller is exposed to the public internet, but it is desired
// to make the admin interface only appear on a local network, this can be used
// so only the local network URL or IP Addresses will be allowed into the admin
// interface.
//
// Set to null to have no restrictions.  An empty array means the Admin
// interface will simply not work.
this.Web.AllowedAdminHosts = ["localhost"];

// The base path at which the application runs.
// It is where the runtime looks for the executables and libraries.
// 
// Leave null to use the default location.  Unless you know what you're
// doing, you should probably leave this null.
this.Web.ContentRoot = null;

// Where the static files are located.  Set to null to use the ASP.NET Core
// default location.  For the most part, this should be left null
// unless you know what you're doing and moved the static files
// somewhere for some reason.
this.Web.WebRoot = null;