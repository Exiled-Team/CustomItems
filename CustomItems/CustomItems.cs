// -----------------------------------------------------------------------
// <copyright file="CustomItems.cs" company="Joker119">
// Copyright (c) Joker119. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1200
namespace CustomItems;

using System;
using Events;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using HarmonyLib;
using Server = Exiled.Events.Handlers.Server;

/// <inheritdoc />
public class CustomItems : Plugin<Config>
{
    private ServerHandler serverHandler = null!;

    /// <summary>
    /// Gets the Plugin instance.
    /// </summary>
    public static CustomItems Instance { get; private set; } = null!;

    /// <inheritdoc/>
    public override Version RequiredExiledVersion { get; } = new(8, 0, 0);

    /// <inheritdoc/>
    public override void OnEnabled()
    {
        Instance = this;
        serverHandler = new ServerHandler();

        Config.LoadItems();

        Log.Debug("Registering items..");
        CustomItem.RegisterItems(overrideClass: Config.ItemConfigs);
        Server.ReloadedConfigs += serverHandler.OnReloadingConfigs;

        base.OnEnabled();
    }

    /// <inheritdoc/>
    public override void OnDisabled()
    {
        CustomItem.UnregisterItems();

        Server.ReloadedConfigs -= serverHandler.OnReloadingConfigs;

        base.OnDisabled();
    }
}