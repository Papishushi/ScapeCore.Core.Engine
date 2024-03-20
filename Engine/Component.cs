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
 * Component.cs
 * Represents a non-updateable behaviour.
 */

using ScapeCore.Core.Engine.Components;

namespace ScapeCore.Core.Engine
{
    /// <summary>
    /// This is the base class for all components.
    /// Components are used to create <seealso cref="Behaviour"/> that are not dependant on game events.
    /// </summary>
    /// <remarks>
    /// Inherit from this class to create a component that can be attached to a <seealso cref="GameObject"/>.
    /// </remarks>
    public abstract class Component : Behaviour, IEntityComponentModel
    {
        /// <summary>
        /// The attached component <seealso cref="GameObject"/>.
        /// </summary>
        public GameObject? gameObject { get; set; }
        /// <summary>
        /// The attached component <seealso cref="Transform"/>.
        /// </summary>
        public Transform? transform { get => gameObject?.transform; }
        /// <summary>
        /// Creates a new <seealso cref="Component"/> instance.
        /// </summary>
        public Component() : base(nameof(Component)) { }
        /// <summary>
        /// Creates a new <seealso cref="Component"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name of the component.</param>
        protected Component(string name) : base(name) { }

        /// <summary>
        /// OnCreate executes when this instance is created.
        /// </summary>
        /// <remarks>Override this method to add your creation logic.</remarks>
        protected override void OnCreate() { /*In this way there is no need to implement it by default on children classes*/ }
        /// <summary>
        /// OnDestroy executes when this instance is destroyed.
        /// </summary>
        /// <remarks>Override this method to add your destruction logic.</remarks>
        protected override void OnDestroy() => gameObject = null;
    }
}