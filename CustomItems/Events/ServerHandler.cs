// -----------------------------------------------------------------------
// <copyright file="ServerHandler.cs" company="Galaxy199 and iopietro">
// Copyright (c) Galaxy199 and iopietro. All rights reserved.
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
            Instance.Config.LoadItemConfigs();
            Instance.Config.ParseSubclassList();
        }
    }
}
