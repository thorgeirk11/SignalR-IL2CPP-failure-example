using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;
using TMPro;
using System;
using Microsoft.Extensions.Logging;

public class HubStuff : MonoBehaviour
{
    private HubConnection connection;

    public HttpConnectionFactory factory;
    public TextMeshProUGUI Text;

    public class UnityLogger : ILoggerProvider
    {
        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
        {
            return new UnityLog();
        }
        public class UnityLog : Microsoft.Extensions.Logging.ILogger
        {
            public IDisposable BeginScope<TState>(TState state)
            {
                var id = Guid.NewGuid();
                Debug.Log($"BeginScope ({id}): {state}");
                return new Scope<TState>(state, id);
            }
            struct Scope<TState> : IDisposable
            {
                public Scope(TState state, Guid id)
                {
                    State = state;
                    Id = id;
                }

                public TState State { get; }
                public Guid Id { get; }

                public void Dispose() => Debug.Log($"EndScope ({Id}): {State}");
            }

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                switch (logLevel)
                {
                    case LogLevel.Trace:
                    case LogLevel.Debug:
                    case LogLevel.Information:
                        Debug.Log($"{logLevel}, {eventId}, {state}, {exception}");
                        break;
                    case LogLevel.Warning:
                        Debug.LogWarning($"{logLevel}, {eventId}, {state}, {exception}");
                        break;
                    case LogLevel.Error:
                    case LogLevel.Critical:
                        Debug.LogError($"{logLevel}, {eventId}, {state}, {exception}");
                        break;
                    case LogLevel.None: break;
                }
            }
        }

        public void Dispose() { }
    }

    // Start is called before the first frame update
    async Task Start()
    {
        try
        {
            Text.text += "Starting\n";
            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/gamehub")
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Debug);
                    logging.AddProvider(new UnityLogger());
                })
                .Build();

            connection.On<string>(nameof(MessageSent), MessageSent);

            Text.text += "Connecting\n";
            await connection.StartAsync();
            Text.text += "Connected\n";
        }
        catch (Exception ex)
        {
            Text.text += ex.ToString();
            Debug.LogException(ex);
        }
    }

    // Update is called once per frame
    public void MessageSent(string message)
    {
        Text.text += $"{message} {DateTime.Now}\n";
    }
}
