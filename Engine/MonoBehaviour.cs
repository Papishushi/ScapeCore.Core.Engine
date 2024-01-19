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
 * MonoBehaviour.cs
 * MonoBehaviour is an abstract class representing a custom
 * updateable behaviour in the ScapeCore game engine. It provides
 * functionality for handling the creation, destruction, start,
 * and update events of a game object.
 */

using Baksteen.Extensions.DeepCopy;
using Microsoft.Xna.Framework;
using ScapeCore.Core.Batching.Events;
using ScapeCore.Core.Engine.Components;
using System.Linq;

namespace ScapeCore.Core.Engine
{
    public abstract class MonoBehaviour : Behaviour, IEntityComponentModel
    {
        private bool _started = false;
        private GameTime? _time;
        protected StartBatchEventHandler? _startEvent;
        protected UpdateBatchEventHandler? _updateEvent;
        public GameTime? Time { get => _time; }
        public GameObject? gameObject { get; set; }
        public Transform? transform { get => gameObject?.transform; }


        public MonoBehaviour() : base(nameof(MonoBehaviour)) => gameObject = new(this);

        public MonoBehaviour(StartBatchEventHandler start, UpdateBatchEventHandler update) : this()
        {
            _startEvent = start;
            _updateEvent = update;
        }

        public MonoBehaviour(StartBatchEventHandler start, UpdateBatchEventHandler update, params Behaviour[] behaviours) : this(start, update) => gameObject!.AddBehaviours(behaviours);

        public static T? Clone<T>(T monoBehaviour) where T : MonoBehaviour => DeepCopyObjectExtensions.DeepCopy(monoBehaviour);

        protected override void OnCreate()
        {
            _startEvent += StartWrapper;
            _updateEvent += UpdateWrapper;
        }

        protected override void OnDestroy()
        {
            _startEvent -= StartWrapper;
            _updateEvent -= UpdateWrapper;
            gameObject = null;
        }

        protected abstract void Start();
        protected abstract void Update();

        private void StartWrapper(object source, StartBatchEventArgs args)
        {
            if (_started) return;
            if (gameObject == null) return;
            if (IsDestroyed || !IsActive || gameObject.IsDestroyed || !gameObject.IsActive) return;
            Start();
            _started = true;
        }
        private void UpdateWrapper(object source, UpdateBatchEventArgs args)
        {
            if (gameObject == null) return;
            if (IsDestroyed || !IsActive || gameObject.IsDestroyed || !gameObject.IsActive) return;
            _time = args.GetTime();
            Update();
        }
    }
}