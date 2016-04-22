using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketBase;

namespace Wps2Pdf.Command
{
    public class Convert : StringCommandBase<AppSession>
    {
        public override void ExecuteCommand(AppSession session, StringRequestInfo requestInfo)
        {
            lock (Converter.queue)
            {
                if (Converter.queue.Count < Converter.maxQueueLength)
                {
                    QueueItem item = new QueueItem();
                    item.session = session;
                    item.filePath = requestInfo.Body;

                    Converter.queue.Enqueue(item);
                    Converter.sem.Release();

                    session.Send("WaitConvert");
                }
                else
                {
                    session.Send("QueueFulled");
                }
            }
        }
    }
}
