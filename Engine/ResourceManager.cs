/*
 * -*- encoding: utf-8 with BOM -*-
 * .▄▄ ·  ▄▄·  ▄▄▄·  ▄▄▄·▄▄▄ .     ▄▄·       ▄▄▄  ▄▄▄ .
 * ▐█ ▀. ▐█ ▌▪▐█ ▀█ ▐█ ▄█▀▄.▀·    ▐█ ▌▪▪     ▀▄ █·▀▄.▀·
 * ▄▀▀▀█▄██ ▄▄▄█▀▀█  ██▀·▐▀▀▪▄    ██ ▄▄ ▄█▀▄ ▐▀▀▄ ▐▀▀▪▄
 * ▐█▄▪▐█▐███▌▐█ ▪▐▌▐█▪·•▐█▄▄▌    ▐███▌▐█▌.▐▌▐█•█▌▐█▄▄▌
 *  ▀▀▀▀ ·▀▀▀  ▀  ▀ .▀    ▀▀▀     ·▀▀▀  ▀█▄▀▪.▀  ▀ ▀▀▀ 
 * https://github.com/Papishushi/ScapeCore
 * 
 * Copyright (c) 2023 Daniel Molinero Lucas
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 * 
 * ResourceManager.cs
 * This manager is responsible for managing resources,
 * their dependencies, and loading referenced resources.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ScapeCore.Core.Batching.Events;
using ScapeCore.Core.Batching.Resources;
using ScapeCore.Core.Tools;
using ScapeCore.Core.Targets;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

using static ScapeCore.Core.Debug.Debugger;
using static ScapeCore.Traceability.Logging.LoggingColor;
using System.Collections.Generic;
using ScapeCore.Core.Serialization;

namespace ScapeCore.Core.Engine
{
    public class ResourceManager : IScapeCoreManager
    {
        private static ResourceManager? _defaultManager = null;
        public static ResourceManager? Default { get => _defaultManager; set => _defaultManager = value; }

        private readonly ResourceDependencyTree _tree = new();
        public ResourceDependencyTree Content { get => _tree; }
        List<IScapeCoreService?> IScapeCoreManager.Services { get => throw new NotImplementedException(); }

        private LLAM? _game;

        public ResourceManager()
        {
            LLAM.Instance.TryGetTarget(out var target);
            _game = target;
            _game!.OnLoad += LoadAllReferencedResources;
            _defaultManager ??=this;
        }

        public StrongBox<T?> GetResource<T>(string key) => new(new DeeplyMutable<T>(_tree.Dependencies[new(key, typeof(T))].resource).Value);

        private void LoadAllReferencedResources(object source, LoadBatchEventArgs args)
        {
            SCLog.Log(DEBUG, $"{source.GetHashCode()} {args.GetInfo()}");

            foreach (var type in ReflectiveEnumerator.GetEnumerableOfType<MonoBehaviour>())
            {
                foreach (var rsrcLoadAttr in Attribute.GetCustomAttributes(type).Where(attr => attr is ResourceLoadAttribute && attr != null).Cast<ResourceLoadAttribute>())
                {
                    foreach (var loadName in rsrcLoadAttr.names)
                    {
                        var info = new ResourceInfo(loadName, rsrcLoadAttr.loadType);
                        if (_tree.ContainsResource(info))
                            _tree.GetResource(info).dependencies.Add(type);
                        else
                        {
                            var method = typeof(ContentManager).GetMethod(nameof(_game.Content.Load));
                            method = method?.MakeGenericMethod(info.TargetType);
                            var result = method?.Invoke(_game?.Content, new object[1] { info.ResourceName });
                            if (result == null)
                            {
                                SCLog.Log(ERROR, $"Resource Manager encountered an error while loading a resource. Resource load returned {Yellow}null{default}.");
                                continue;
                            }
                            var obj = Convert.ChangeType(result, info.TargetType);
                            if (obj == null)
                            {
                                SCLog.Log(ERROR, $"Resource Manager encountered an error while loading a resource. Loaded resource wasnt successfully changed to type {info.TargetType} and returned null.");
                                continue;
                            }
                            dynamic changedObject = obj;
                            _tree.Add(info, type, changedObject);
                        }
                    }
                }
            }

            foreach (var dependency in _tree.Dependencies)
            {
                var msg = $"{string.Join(',', dependency.Value.dependencies)} types loaded resource {dependency.Key.ResourceName} of type {dependency.Key.TargetType}";
                SCLog.Log(DEBUG, msg);
            }

        }

        bool IScapeCoreManager.InjectDependencies(params IScapeCoreService[] services)
        {
            throw new NotImplementedException();
        }
    }
}