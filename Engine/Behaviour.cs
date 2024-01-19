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
 * Behaviour.cs
 * It is the base class for all attacheable behaviours used on ScapeCore.
 */

using Microsoft.Xna.Framework;
using ScapeCore.Core.Targets;
using System;

namespace ScapeCore.Core.Engine
{
    public abstract class Behaviour
    {
        private readonly Guid _id = new();

        private bool _isActive;
        private bool _isDestroyed;

        public string name = nameof(Behaviour);

        public Guid Id { get => _id; }
        public bool IsActive { get => _isActive; }
        public bool IsDestroyed { get => _isDestroyed; }
        public LLAM? game;

        ~Behaviour() => Destroy();
        public Behaviour()
        {
            LLAM.Instance.TryGetTarget(out var target);
            game = target;
            _isActive = true;
            _isDestroyed = false;
            OnCreate();
        }
        protected Behaviour(string name) : this() => this.name = name;

        public T? To<T>() where T : Behaviour => this as T;

        public void SetActive(bool isActive) => _isActive = isActive;

        public override string ToString() => name;

        protected abstract void OnCreate();
        protected abstract void OnDestroy();

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