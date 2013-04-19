this.Log-NLog
=============

In your application startup, please include this line:

For C#: LoggingExtensions.Logging.Log.InitializeWith<LoggingExtensions.NLog.NLogLog>();

For VB: LoggingExtensions.Logging.Log.InitializeWith(Of LoggingExtensions.NLog.NLogLog)()