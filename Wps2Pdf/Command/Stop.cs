using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketBase;

namespace Wps2Pdf.Command
{
    public class Stop : StringCommandBase<AppSession>
    {
        public override void ExecuteCommand(AppSession session, StringRequestInfo requestInfo)
        {
            Deamon.sem.Release();
        }
    }
}
