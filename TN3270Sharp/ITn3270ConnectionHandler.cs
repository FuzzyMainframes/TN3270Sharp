using System;
using System.Collections.Generic;
using System.Text;

namespace TN3270Sharp
{
    public interface ITn3270ConnectionHandler
    {
        // ---------------------------------------------------------------
        //byte[] GetBufferBytes();
        //int GetTotalBytesReadFromBuffer();

        // ---------------------------------------------------------------
        void ShowScreen(Screen screen);
        void ShowScreen(Screen screen, bool executePredefinedAidActions);
        void ShowScreen(Screen screen, bool executePredefinedAidActions, Action beforeScreenRenderAction);
        void ShowScreen(Screen screen, bool executePredefinedAidActions, Action<AID> screenBufferProcess);
        void ShowScreen(Screen screen, bool executePredefinedAidActions, Action beforeScreenRenderAction, Action<AID> screenBufferProcess);

        // ---------------------------------------------------------------
        void SetAidAction(AID aidCommand, Action action);

        // ---------------------------------------------------------------
        void CloseConnection();
    }
}

