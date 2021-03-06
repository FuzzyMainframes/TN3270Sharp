// This file is part of https://github.com/roblthegreat/TN3270Sharp
// Copyright 2020 by Robert J. Lawrence (roblthegreat), licensed under the MIT license. See
// LICENSE in the project root for license information.
//
//  Portions of this code may have originated elsewhere and will be noted in the comments as needed.

using System;

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

