// -----------------------------------------------------------------------
// <copyright file="ServerHandler.cs" company="Joker119">
// Copyright (c) Joker119. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CustomItems.Events;

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
    }
}