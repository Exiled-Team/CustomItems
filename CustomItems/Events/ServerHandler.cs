// -----------------------------------------------------------------------
// <copyright file="ServerHandler.cs" company="Galaxy119 and iopietro">
// Copyright (c) Galaxy119 and iopietro. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Events
{
    using static CustomItems;

    /// <summary>
    /// Event Handlers.
    /// </summary>
    public class ServerHandler
    {
        /// <summary>
        /// OnReloadingConfigs handler.
        /// </summary>
        public void OnReloadingConfigs()
        {
            Instance.Config.LoadItems();
            Instance.Config.ParseSubclassList();
        }
    }
}
