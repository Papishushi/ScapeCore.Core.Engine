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
using ScapeCore.Core.Targets;
using System.Threading.Tasks;

namespace ScapeCore.Core.Engine
{
    /// <summary>
    /// This is the base class for any custom updateable <see cref="Behaviour"/>. Implements <see cref="IEntityComponentModel"/>.
    /// </summary>
    public abstract class MonoBehaviour : Behaviour, IEntityComponentModel
    {
        private bool _started = false;
        private GameTime? _time;
        /// <summary>
        /// The current <see cref="GameTime"/> or <see langword="null"/> if the first <see cref="Update"/> cycle has not been invoked yet.
        /// </summary>
        public GameTime? Time { get => _time; }
        /// <summary>
        /// The <see cref="GameObject"/> of this entity.
        /// </summary>
        public GameObject? gameObject { get; set; }
        /// <summary>
        /// The <see cref="Transform"/> of this entity or <see langword="null"/> if <see cref="gameObject"/> is <see langword="null"/>.
        /// This is the same as: <code> <see cref="gameObject"/>?.transform</code>
        /// </summary>
        public Transform? transform { get => gameObject?.transform; }

        public MonoBehaviour() : base(nameof(MonoBehaviour)) => gameObject = new(this);

        public MonoBehaviour(params Behaviour[] behaviours) : this() => gameObject!.AddBehaviours(behaviours);

        public static T? Clone<T>(T monoBehaviour) where T : MonoBehaviour => DeepCopyObjectExtensions.DeepCopy(monoBehaviour);

        protected override void OnCreate()
        {
            //Any MonoBehaviour has the capacity to initialize a new LLAM if it has not been created.
            if (game == null && !LLAM.Instance.TryGetTarget(out game))
            {
                using var llam = new LLAM();
                game = llam;
            }

            game.OnStart += StartWrapper;
            game.OnUpdate += UpdateWrapper;
        }

        protected override void OnDestroy()
        {
            if (game != null)
            {
                game.OnStart -= StartWrapper;
                game.OnUpdate -= UpdateWrapper;
            }

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