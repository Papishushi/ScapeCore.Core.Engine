/*
 * -*- encoding: utf-8 with BOM -*-
 * .▄▄ ·  ▄▄·  ▄▄▄·  ▄▄▄·▄▄▄ .     ▄▄·       ▄▄▄  ▄▄▄ .
 * ▐█ ▀. ▐█ ▌▪▐█ ▀█ ▐█ ▄█▀▄.▀·    ▐█ ▌▪▪     ▀▄ █·▀▄.▀·
 * ▄▀▀▀█▄██ ▄▄▄█▀▀█  ██▀·▐▀▀▪▄    ██ ▄▄ ▄█▀▄ ▐▀▀▄ ▐▀▀▪▄
 * ▐█▄▪▐█▐███▌▐█ ▪▐▌▐█▪·•▐█▄▄▌    ▐███▌▐█▌.▐▌▐█•█▌▐█▄▄▌
 *  ▀▀▀▀ ·▀▀▀  ▀  ▀ .▀    ▀▀▀     ·▀▀▀  ▀█▄▀▪.▀  ▀ ▀▀▀ 
 * https://github.com/Papishushi/ScapeCore
 * 
 * Copyright (c) 2024 Daniel Molinero Lucas
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 * 
 * Behaviour.cs
 * It is the base class for all attacheable behaviours used on ScapeCore.
 */

using ScapeCore.Core.Targets;
using System;

namespace ScapeCore.Core.Engine
{
    /// <summary>
    /// This is the base class for all attacheable behaviours used on ScapeCore.
    /// </summary>
    /// <remarks>
    /// Inherit from this class to create a simple behaviour that can be added and registered in the engine.
    /// </remarks>
    public abstract class Behaviour
    {
        private readonly Guid _id = new();

        private bool _isActive;
        private bool _isDestroyed;
   
        /// <summary>
        /// This is the current name for this behaviour.
        /// </summary>
        public string name = nameof(Behaviour);

        /// <summary>
        /// This is the behaviour unique identifier across multiple references.
        /// </summary>
        public Guid Id { get => _id; }
        /// <summary>
        /// <see langword="true"/> if this behaviour is active. Otherwise <see langword="false"/>.
        /// </summary>
        public bool IsActive { get => _isActive; }
        /// <summary>
        /// <see langword="true"/> if this behaviour has been destroyed. Otherwise <see langword="false"/>.
        /// </summary>
        public bool IsDestroyed { get => _isDestroyed; }

        /// <summary>
        /// A reference to the running game. This is used to link to events and other low level management.
        /// </summary>
        protected LLAM? game;

        /// <summary>
        /// Run destruction logic.
        /// </summary>
        ~Behaviour() => Destroy();
        /// <summary>
        /// Create a new instance of a behaviour initializing the game reference and calling <see cref="OnCreate"/> on the process.
        /// </summary>
        public Behaviour()
        {
            LLAM.Instance.TryGetTarget(out var target);
            game = target;
            _isActive = true;
            _isDestroyed = false;
            OnCreate();
        }
        /// <summary>
        /// Create a new instance of a behaviour with a specific name, initializing the game reference and calling <see cref="OnCreate"/> on the process.
        /// </summary>
        /// <param name="name"> The name this behaviour is initialized to.</param>
        protected Behaviour(string name) : this() => this.name = name;

        /// <summary>
        /// Box this <see cref="Behaviour"/> to any other type of <see cref="Behaviour"/>.
        /// </summary>
        /// <typeparam name="T">The type to box to.</typeparam>
        /// <returns>A new <typeparamref name="T"/> boxed from this <see cref="Behaviour"/>.</returns>
        public T? To<T>() where T : Behaviour => this as T;
        /// <summary>
        /// Set this behaviour active state.
        /// </summary>
        /// <param name="isActive"><see langword="true"/> to activating the behaviour. Otherwise <see langword="false"/>.</param>
        public void SetActive(bool isActive) => _isActive = isActive;
        /// <summary>
        /// Get the current name for this instance.
        /// </summary>
        /// <returns>This <see cref="Behaviour"/> current name.</returns>
        public override string ToString() => name;

        /// <summary>
        /// Override this method with the actions your custom behaviour will take on creation.
        /// </summary>
        protected abstract void OnCreate();
        /// <summary>
        /// Override this method with the actions your custom behaviour will take on destruction.
        /// </summary>
        protected abstract void OnDestroy();

        /// <summary>
        /// Sets this <see cref="Behaviour"/> for destruction.
        /// </summary>
        public void Destroy()
        {
            if (_isDestroyed) return;
            _isActive = false;
            OnDestroy();
            game = null;
            _isDestroyed = true;
        }

    }
}