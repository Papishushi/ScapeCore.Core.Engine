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
 * GameObject.cs
 * Represents a fundamental entity within a scene that can have
 * behaviours attached to it.
 */

using ScapeCore.Core.Engine.Components;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ScapeCore.Core.Engine
{
    public sealed class GameObject : Behaviour
    {
        public Transform? transform;
        public string tag;
        private readonly List<Behaviour> behaviours;

        private static readonly List<string> tagList = new();
        public static ImmutableList<string> TagList { get => tagList.ToImmutableList(); }

        public GameObject? parent = null;
        public readonly List<GameObject> children = new();

        public GameObject() : base(nameof(GameObject))
        {
            transform = new();
            tag = string.Empty;
            behaviours = new()
            {
                transform
            };
        }
        public GameObject(string name) : base(name)
        {
            transform=new();
            tag = string.Empty;
            behaviours=new()
            {
                transform
            };
        }
        public GameObject(params Behaviour[] behaviours) : this()
        {
            foreach (Behaviour behaviour in behaviours)
                this.behaviours.Add(behaviour);
        }
        public GameObject(string name, params Behaviour[] behaviours) : this(name)
        {
            foreach (Behaviour behaviour in behaviours)
                this.behaviours.Add(behaviour);
        }
        /// <summary>
        /// The best contraceptive for old people is nudity.
        /// </summary>
        /// <exception cref="System.NullReferenceException"></exception>
        private void BehavioursNullException()
        {
            if (behaviours == null) throw new System.ArgumentNullException($"{nameof(behaviours)} is null");
        }

        public T? GetBehaviour<T>() where T : Behaviour
        {
            try
            {
                BehavioursNullException();
            }
            catch (NullReferenceException nRE)
            {
                var msg = $"Failed to add behaviour on GameObject {name} {{{Id}}}\t{nRE.Message}";
                Log.Error(msg);
                throw new ArgumentNullException(msg);
            }
            foreach (Behaviour behaviour in behaviours)
                if (behaviour.GetType() == typeof(T)) return (T)behaviour;
            return null;
        }

        public IEnumerator<T> GetBehaviours<T>() where T : Behaviour
        {
            try
            {
                BehavioursNullException();
            }
            catch (NullReferenceException nRE)
            {
                var msg = $"Failed to add behaviour on GameObject {name} {{{Id}}}\t{nRE.Message}";
                Log.Error(msg);
                throw new ArgumentNullException(msg);
            }
            return behaviours.Where(x => x.GetType() == typeof(T)).Cast<T>().GetEnumerator();
        }

        public T AddBehaviour<T>() where T : Behaviour, new()
        {
            try
            {
                BehavioursNullException();
            }
            catch (NullReferenceException nRE)
            {
                var msg = $"Failed to add behaviour on GameObject {name} {{{Id}}}\t{nRE.Message}";
                Log.Error(msg);
                throw new ArgumentNullException(msg);
            }
            var temp = new T();
            behaviours.Add(temp);
            if (typeof(IEntityComponentModel).IsAssignableFrom(temp.GetType()))
                ((IEntityComponentModel)temp).gameObject = this;
            return temp;
        }
        public T? AddBehaviour<T>(T behaviour) where T : Behaviour
        {
            try
            {
                BehavioursNullException();
            }
            catch (NullReferenceException nRE)
            {
                var msg = $"Failed to add behaviour on GameObject {name} {{{Id}}}\t{nRE.Message}";
                Log.Error(msg);
                throw new ArgumentNullException(msg);
            }
            if (behaviour == null) return null;
            behaviours.Add(behaviour);
            if (typeof(IEntityComponentModel).IsAssignableFrom(behaviour.GetType()))
                ((IEntityComponentModel)behaviour).gameObject = this;
            return behaviour;
        }

        public IEnumerator<T>? AddBehaviours<T>(params T[] behaviours) where T : Behaviour
        {
            try
            {
                BehavioursNullException();
            }
            catch (NullReferenceException nRE)
            {
                var msg = $"Failed to add behaviour on GameObject {name} {{{Id}}}\t{nRE.Message}";
                Log.Error(msg);
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

        public T? RemoveBehaviour<T>() where T : Behaviour
        {
            try
            {
                BehavioursNullException();
            }
            catch (NullReferenceException nRE)
            {
                var msg = $"Failed to remove behaviour on GameObject {name} {{{Id}}}\t{nRE.Message}";
                Log.Error(msg);
                throw new ArgumentNullException(msg);
            }
            T? temp = behaviours.Find(x => x.GetType() == typeof(T))?.To<T>();
            if (temp == null) return null;
            behaviours.Remove(temp);
            if (typeof(IEntityComponentModel).IsAssignableFrom(temp.GetType()))
                ((IEntityComponentModel)temp).gameObject = null;
            return temp;
        }

        public T? RemoveBehaviour<T>(T behaviour) where T : Behaviour
        {
            try
            {
                BehavioursNullException();
            }
            catch (NullReferenceException nRE)
            {
                var msg = $"Failed to remove behaviour on GameObject {name} {{{Id}}}\t{nRE.Message}";
                Log.Error(msg);
                throw new ArgumentNullException(msg);
            }
            if (behaviour == null) return null;
            behaviours.Remove(behaviour);
            if (typeof(IEntityComponentModel).IsAssignableFrom(behaviour.GetType()))
                ((IEntityComponentModel)behaviour).gameObject = null;
            return behaviour;
        }

        public IEnumerator<T> RemoveBehaviours<T>() where T : Behaviour
        {
            try
            {
                BehavioursNullException();
            }
            catch (NullReferenceException nRE)
            {
                var msg = $"Failed to remove behaviour on GameObject {name} {{{Id}}}\t{nRE.Message}";
                Log.Error(msg);
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

        public IEnumerator<T> RemoveBehaviours<T>(params T[] behaviours) where T : Behaviour
        {
            try
            {
                BehavioursNullException();
            }
            catch (NullReferenceException nRE)
            {
                var msg = $"Failed to remove behaviour on GameObject {name} {{{Id}}}\t{nRE.Message}";
                Log.Error(msg);
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

        protected override void OnDestroy()
        {
            foreach (var behaviour in behaviours)
                behaviour.Destroy();
            transform = null;
        }

        protected override void OnCreate()
        {
            throw new NotImplementedException();
        }
    }
}