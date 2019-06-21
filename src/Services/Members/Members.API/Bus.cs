using Members.DataAccess;
using Members.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Members.API
{
    public class Bus : IBus
    {
        private readonly ILogger<Bus> _logger;
        private readonly IQueueClient queueClient;

        bool _disposed;

        public Bus(string connectionString, string queueName, ILogger<Bus> logger)
        {
            queueClient = new QueueClient(connectionString, queueName, ReceiveMode.ReceiveAndDelete);

            _logger = logger;

            RegisterOnMessageHandlerAndReceiveMessages();
        }

        private void RegisterOnMessageHandlerAndReceiveMessages()
        {
            // Configure the MessageHandler Options in terms of exception handling, number of concurrent messages to deliver etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of Concurrent calls to the callback `ProcessMessagesAsync`, set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether MessagePump should automatically complete the messages after returning from User Callback.
                // False below indicates the Complete will be handled by the User Callback as in `ProcessMessagesAsync` below.
                AutoComplete = true
            };

            // Register the function that will process messages
            queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var messageBody = Encoding.UTF8.GetString(message.Body);

            // Process the message
            _logger.LogInformation($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body: {messageBody}");
            
            Member member = JsonConvert.DeserializeObject<Member>(messageBody);

            using (var context = new MemberContext())
            {
                var entity = await context.Members.Where(p => member.ToastmastersId == p.ToastmastersId).SingleOrDefaultAsync();

                if (null == entity)
                {
                    member.Active = true;

                    context.Members.Add(member);

                    _logger.LogInformation($"Adding member {member.Name}.");
                }
                else
                {
                    entity.Active = true;
                    entity.Email = member.Email;
                    entity.Name = member.Name;                    
                    entity.Rank = member.Rank;

                    _logger.LogInformation($"Updating member {member.Name}.");
                }

                await context.SaveChangesAsync();
            }

            _logger.LogInformation($"Member {member.Name} saved.");


            // Complete the message so that it is not received again.
            // This can be done only if the queueClient is created in ReceiveMode.PeekLock mode (which is default).
            //await queueClient.CompleteAsync(message.SystemProperties.LockToken);
            //await queueClient.CompleteAsync(message.SystemProperties.LockToken);

            // Note: Use the cancellationToken passed as necessary to determine if the queueClient has already been closed.
            // If queueClient has already been Closed, you may chose to not call CompleteAsync() or AbandonAsync() etc. calls 
            // to avoid unnecessary exceptions.
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

            _logger.LogError($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            _logger.LogError("Exception context for troubleshooting:");
            _logger.LogError($"- Endpoint: {context.Endpoint}");
            _logger.LogError($"- Entity Path: {context.EntityPath}");
            _logger.LogError($"- Executing Action: {context.Action}");

            return Task.CompletedTask;
        }
        
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
        }
    }
}
