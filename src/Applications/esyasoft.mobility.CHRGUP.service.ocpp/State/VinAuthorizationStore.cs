//using System.Collections.Concurrent;

//namespace OcppMicroservice.State
//{
//    public class VinAuthorizationStore
//    {
//        private static readonly ConcurrentDictionary<string, TaskCompletionSource<bool>> _pending =
//            new();

//        public static Task<bool> Wait(string messageId)
//        {
//            var tcs = new TaskCompletionSource<bool>(
//                TaskCreationOptions.RunContinuationsAsynchronously);

//            _pending[messageId] = tcs;
//            return tcs.Task;
//        }

//        public static void Resolve(string messageId, bool accepted)
//        {
//            if (_pending.TryRemove(messageId, out var tcs))
//            {
//                tcs.TrySetResult(accepted);
//            }
//        }
//    }
//}

//using esyasoft.mobility.CHRGUP.service.ocpp.Ocpp;
//using System.Collections.Concurrent;
//using System.Net.WebSockets;
//using System.Text;


//namespace esyasoft.mobility.CHRGUP.service.ocpp.State
//{
//    public static class VinAuthorizationStore
//    {
//        private static readonly ConcurrentDictionary<string, WebSocket> _pending = new();

//        public static void Register(string messageId, WebSocket socket)
//        {
//            _pending[messageId] = socket;
//        }

//        public static async Task Resolve(string messageId, bool accepted)
//        {
//            if (!_pending.TryRemove(messageId, out var socket))
//                return;

//            var response = OcppMessage.CreateCallResult(messageId, new
//            {
//                idTokenInfo = new
//                {
//                    status = accepted ? "Accepted" : "Rejected"
//                }
//            });

//            var bytes = Encoding.UTF8.GetBytes(response);

//            await socket.SendAsync(
//                bytes,
//                WebSocketMessageType.Text,
//                true,
//                CancellationToken.None);
//        }
//    }
//}
