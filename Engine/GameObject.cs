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
 * GameObject.cs
 * Represents a fundamental entity within a scene that can have
 * other behaviours like components attached to it.
 */

using ScapeCore.Core.Engine.Components;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using static ScapeCore.Core.Debug.Debugger;
using static ScapeCore.Traceability.Logging.LoggingColor;

namespace ScapeCore.Core.Engine
{
    /// <summary>
    /// Represents a fundamental entity within a scene that can have other <see cref="Behaviour"/> attached to it.
    /// </summary>
    public sealed class GameObject : Behaviour
    {
        /// <summary>
        /// Represents the <see cref="Transform"/> of the current <see cref="GameObject"/>.
        /// </summary>
        public Transform? transform;
        /// <summary>
        /// A simple <see cref="string"/> used to identify the <see cref="GameObject"/>.
        /// </summary>
        public string tag;
        private readonly List<Behaviour> behaviours;

        private static readonly List<string> tagList = new();
        /// <summary>
        /// A reference to a <see cref="ImmutableList"/> of all available tags for all <see cref="GameObject">GameObjects</see>.
        /// </summary>
        public static ImmutableList<string> TagList { get => tagList.ToImmutableList(); }

        /// <summary>
        /// The hierarchical parent of this <see cref="GameObject"/>, <see langword="null"/> if the current <see cref="GameObject"/> is the root.
        /// </summary>
        public GameObject? parent = null;
        /// <summary>
        /// A list containing all the direct children of this <see cref="GameObject"/>.
        /// </summary>
        public readonly List<GameObject> children = new();

        /// <summary>
        /// Creates a new <see cref="GameObject"/> instance.
        /// </summary>
        public GameObject() : base(nameof(GameObject))
        {
            transform = new();
            tag = string.Empty;
            behaviours = new()
            {
                transform
            };
        }
        /// <summary>
        /// Creates a new <see cref="GameObject"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="GameObject"/>.</param>
        public GameObject(string name) : base(name)
        {
            transform=new();
            tag = string.Empty;
            behaviours=new()
            {
                transform
            };
        }
        /// <summary>
        /// Creates a new <see cref="GameObject"/> instance with the specified <see cref="Behaviour">Behaviours</see>.
        /// </summary>
        /// <param name="behaviours">The <see cref="Behaviour">Behaviours</see> to be attached to this <see cref="GameObject"/>.</param>
        public GameObject(params Behaviour[] behaviours) : this()
        {
            foreach (Behaviour behaviour in behaviours)
                this.behaviours.Add(behaviour);
        }
        /// <summary>
        /// Creates a new <see cref="GameObject"/> instance with the specified name and <see cref="Behaviour">Behaviours</see>.
        /// </summary>
        /// <param name="name">The name of the <see cref="GameObject"/>.</param>
        /// <param name="behaviours">The <see cref="Behaviour">Behaviours</see> to be attached to this <see cref="GameObject"/>.</param>
        public GameObject(string name, params Behaviour[] behaviours) : this(name)
        {
            foreach (Behaviour behaviour in behaviours)
                this.behaviours.Add(behaviour);
        }
        /// <summary>
        /// Throws a  <see cref="ArgumentNullException"/> if <see cref="behaviours"/> is null.
        /// </summary>
        /// <exception cref="System.ArgumentNullException"></exception>
        private void BehavioursNullException()
        {
            if (behaviours == null) throw new System.ArgumentNullException($"{nameof(behaviours)} is null");
        }
        /// <summary>
        /// Search for the first <see cref="Behaviour"/> of type <typeparamref name="T"/> on this <see cref="GameObject"/>.
        /// </summary>
        /// <typeparam name="T">The type to search for.</typeparam>
        /// <returns>The first <see cref="Behaviour"/> of type <typeparamref name="T"/> to be found.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public T? GetBehaviour<T>() where T : Behaviour
        {
            try
            {
                BehavioursNullException();
            }
            catch (NullReferenceException nRE)
            {
                var msg = $"Failed to add behaviour on GameObject {name} {{{Id}}}\t{nRE.Message}";
                SCLog.Log(ERROR, msg);
                throw new ArgumentNullException(msg);
            }
            foreach (Behaviour behaviour in behaviours)
                if (behaviour.GetType() == typeof(T)) return (T)behaviour;
            return null;
        }
        /// <summary>
        /// Search for all <see cref="Behaviour">Behaviours</see> of type <typeparamref name="T"/> on this <see cref="GameObject"/>.
        /// </summary>
        /// <typeparam name="T">The type to search for.</typeparam>
        /// <returns>All <see cref="Behaviour">Behaviours</see> of type <typeparamref name="T"/> to be found.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IEnumerator<T> GetBehaviours<T>() where T : Behaviour
        {
            try
            {
                BehavioursNullException();
            }
            catch (NullReferenceException nRE)
            {
                var msg = $"Failed to add behaviour on GameObject {name} {{{Id}}}\t{nRE.Message}";
                SCLog.Log(ERROR, msg);
                throw new ArgumentNullException(msg);
            }
            return behaviours.Where(x => x.GetType() == typeof(T)).Cast<T>().GetEnumerator();
        }

        /// <summary>
        /// Adds a new <see cref="Behaviour"/> of type <typeparamref name="T"/> to this <see cref="GameObject"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Behaviour"/> type to add.</typeparam>
        /// <returns>The newly attached <see cref="Behaviour"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public T AddBehaviour<T>() where T : Behaviour, new()
        {
            try
            {
                BehavioursNullException();
            }
            catch (NullReferenceException nRE)
            {
                var msg = $"Failed to add behaviour on GameObject {name} {{{Id}}}\t{nRE.Message}";
                SCLog.Log(ERROR, msg);
                throw new ArgumentNullException(msg);
            }
            var temp = new T();
            behaviours.Add(temp);
            if (typeof(IEntityComponentModel).IsAssignableFrom(temp.GetType()))
                ((IEntityComponentModel)temp).gameObject = this;
            return temp;
        }
        /// <summary>
        /// Adds <see href="behaviour"/> to this <see cref="GameObject"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Behaviour"/> type to add.</typeparam>
        /// <param name="behaviour">The <see cref="Behaviour"/> to be added.</param>
        /// <returns>The newly attached <see cref="Behaviour"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public T? AddBehaviour<T>(T behaviour) where T : Behaviour
        {
            try
            {
                BehavioursNullException();
            }
            catch (NullReferenceException nRE)
            {
                var msg = $"Failed to add behaviour on GameObject {name} {{{Id}}}\t{nRE.Message}";
                SCLog.Log(ERROR, msg);
                throw new ArgumentNullException(msg);
            }
            if (behaviour == null) return null;
            behaviours.Add(behaviour);
            if (typeof(IEntityComponentModel).IsAssignableFrom(behaviour.GetType()))
                ((IEntityComponentModel)behaviour).gameObject = this;
            return behaviour;
        }

        /// <summary>
        /// Adds the specified <see href="behaviours"/> to this <see cref="GameObject"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Behaviour"/> type to add.</typeparam>
        /// <param name="behaviours">The <see cref="Behaviour">Behaviours</see> to be added.</param>
        /// <returns>A collection containing all the newly attached <see cref="Behaviour">Behaviours</see>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IEnumerator<T>? AddBehaviours<T>(params T[] behaviours) where T : Behaviour
        {
            try
            {
                BehavioursNullException();
            }
            catch (NullReferenceException nRE)
            {
                var msg = $"Failed to add behaviour on GameObject {name} {{{Id}}}\t{nRE.Message}";
                SCLog.Log(ERROR, msg);
                throw new ArgumentNullException(msg);
            }
            if (behaviours == null) return null;
            var l = new List<T>();
            foreach (T behaviour in behaviours.Cast<T>())
            {
                if (behaviour == null) continue;
                this.behaviours.Add(behaviour);
                if (typeof(IEntityComponentModel).IsAssignableFrom(behaviour.GetType()))
                    ((IEntityComponentModel)behaviour).gameObject = this;
                l.Add(behaviour);
            }
            return l.GetEnumerator();
        }

        /// <summary>
        /// Removes a <see cref="Behaviour"/> of type <typeparamref name="T"/> from this <see cref="GameObject"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Behaviour"/> type to remove.</typeparam>
        /// <returns>The removed <see cref="Behaviour"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public T? RemoveBehaviour<T>() where T : Behaviour
        {
            try
            {
                BehavioursNullException();
            }
            catch (NullReferenceException nRE)
            {
                var msg = $"Failed to remove behaviour on GameObject {name} {{{Id}}}\t{nRE.Message}";
                SCLog.Log(ERROR, msg);
                throw new ArgumentNullException(msg);
            }
            T? temp = behaviours.Find(x => x.GetType() == typeof(T))?.To<T>();
            if (temp == null) return null;
            behaviours.Remove(temp);
            if (typeof(IEntityComponentModel).IsAssignableFrom(temp.GetType()))
                ((IEntityComponentModel)temp).gameObject = null;
            return temp;
        }
        /// <summary>
        /// Removes <see href="behaviour"/> from this <see cref="GameObject"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Behaviour"/> type to remove.</typeparam>
        /// <param name="behaviour">This is the <see cref="Behaviour"/> that will be removed.</param>
        /// <returns>The same <see cref="Behaviour"/> input as a parameter.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public T? RemoveBehaviour<T>(T behaviour) where T : Behaviour
        {
            try
            {
                BehavioursNullException();
            }
            catch (NullReferenceException nRE)
            {
                var msg = $"Failed to remove behaviour on GameObject {name} {{{Id}}}\t{nRE.Message}";
                SCLog.Log(ERROR, msg);
                throw new ArgumentNullException(msg);
            }
            if (behaviour == null) return null;
            behaviours.Remove(behaviour);
            if (typeof(IEntityComponentModel).IsAssignableFrom(behaviour.GetType()))
                ((IEntityComponentModel)behaviour).gameObject = null;
            return behaviour;
        }

        /// <summary>
        /// Removes all <see cref="Behaviour">Behaviours</see> of type <typeparamref name="T"/> from this <see cref="GameObject"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Behaviour"/> type to remove.</typeparam>
        /// <returns>A collection that contains all the removed <see cref="Behaviour">Behaviours</see>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IEnumerator<T> RemoveBehaviours<T>() where T : Behaviour
        {
            try
            {
                BehavioursNullException();
            }
            catch (NullReferenceException nRE)
            {
                var msg = $"Failed to remove behaviour on GameObject {name} {{{Id}}}\t{nRE.Message}";
                SCLog.Log(ERROR, msg);
                throw new ArgumentNullException(msg);
            }
            var l = new List<T>();
            foreach (T behaviour in behaviours.Cast<T>())
            {
                if (behaviour == null) continue;
                behaviours.Remove(behaviour);
                if (typeof(IEntityComponentModel).IsAssignableFrom(behaviour.GetType()))
                    ((IEntityComponentModel)behaviour).gameObject = null;
                l.Add(behaviour);
            }
            return l.GetEnumerator();
        }
        /// <summary>
        /// Removes all the specified <see href="behaviours"/> from this <see cref="GameObject"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Behaviour"/> type to remove.</typeparam>
        /// <param name="behaviours">These are all the <see cref="Behaviour">Behaviours</see> to be removed.</param>
        /// <returns>A collection that contains all the removed <see cref="Behaviour">Behaviours</see>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IEnumerator<T> RemoveBehaviours<T>(params T[] behaviours) where T : Behaviour
        {
            try
            {
                BehavioursNullException();
            }
            catch (NullReferenceException nRE)
            {
                var msg = $"Failed to remove behaviour on GameObject {name} {{{Id}}}\t{nRE.Message}";
                SCLog.Log(ERROR, msg);
                throw new ArgumentNullException(msg);
            }
            var l = new List<T>();
            foreach (T behaviour in behaviours.Cast<T>())
            {
                if (behaviour == null) continue;
                if (!behaviours.Contains(behaviour)) continue;
                this.behaviours.Remove(behaviour);
                if (typeof(IEntityComponentModel).IsAssignableFrom(behaviour.GetType()))
                    ((IEntityComponentModel)behaviour).gameObject = null;
                l.Add(behaviour);
            }
            return l.GetEnumerator();
        }

        /// <summary>
        /// The actions that will take place on destruction.
        /// </summary>
        protected override void OnDestroy()
        {
            foreach (var behaviour in behaviours)
                behaviour.Destroy();
            transform = null;
        }

        /// <summary>
        /// The actions that will take place on creation.
        /// </summary>
        protected override void OnCreate() { /*There is no need to implement it.*/ }
    }
}