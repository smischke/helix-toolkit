// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Subscriber.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A Subscriber can be attached to DependencyObjects and subscribed to DependencyProperties.
//   One can inherit from this class or just use it.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;

    /// <summary>
    /// A <see cref="Subscriber"/> can be can be attached to <see cref="DependencyObject"/>
    /// and subscribed to <see cref="DependencyProperty"/>. One can inherit from this class or just use it.
    /// </summary>
    public class Subscriber
    {
        private readonly HashSet<Subscription> subscriptions = new HashSet<Subscription>();

        /// <summary>
        /// Gets the target <see cref="DependencyObject"/>.
        /// </summary>
        public DependencyObject Target { get; private set; }

        /// <summary>
        /// Attach this <see cref="Subscriber"/> to a <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="target">
        /// The respective <see cref="DependencyObject"/>.
        /// </param>
        public void Attach(DependencyObject target)
        {
            this.Target = target;
            this.OnAttach();
        }

        /// <summary>
        /// Detaches this <see cref="Subscriber"/> from a <see cref="DependencyObject"/>, if attached.
        /// </summary>
        public void Detach()
        {
            this.UnsubscribeAll();
            this.OnDetach();
            this.Target = null;
        }

        /// <summary>
        /// Called on attach. Subscriptions can be made here.
        /// </summary>
        protected virtual void OnAttach()
        {
        }

        /// <summary>
        /// Called on detach. All subscriptions are automatically undone just before.
        /// </summary>
        protected virtual void OnDetach()
        {
        }

        /// <summary>
        /// Subscribes to a <see cref="DependencyProperty"/>.
        /// </summary>
        /// <param name="dp">
        /// The respective <see cref="DependencyProperty"/>.
        /// </param>
        /// <param name="eh">
        /// The respective <see cref="EventHandler"/>.
        /// </param>
        public void Subscribe(DependencyProperty dp, EventHandler eh)
        {
            this.Subscribe(this.Target, dp, eh);
        }

        /// <summary>
        /// Subscribes to a <see cref="DependencyProperty"/>.
        /// </summary>
        /// <param name="d">
        /// The target <see cref="DependencyObject"/> if different from <see cref="Target"/>.
        /// </param>
        /// <param name="dp">
        /// The respective <see cref="DependencyProperty"/>.
        /// </param>
        /// <param name="eh">
        /// The respective <see cref="EventHandler"/>.
        /// </param>
        public void Subscribe(DependencyObject d, DependencyProperty dp, EventHandler eh)
        {
            this.Subscribe(d, dp, d.GetType(), eh);
        }

        /// <summary>
        /// Subscribes to a <see cref="DependencyProperty"/>.
        /// </summary>
        /// <param name="d">
        /// The target <see cref="DependencyObject"/> if different from <see cref="Target"/>.
        /// </param>
        /// <param name="dp">
        /// The respective <see cref="DependencyProperty"/>.
        /// </param>
        /// <param name="tp">
        /// The target type of <see cref="dp"/> if different from the type of <see cref="Target"/>.
        /// </param>
        /// <param name="eh">
        /// The respective <see cref="EventHandler"/>.
        /// </param>
        public void Subscribe(DependencyObject d, DependencyProperty dp, Type tp, EventHandler eh)
        {
            var dpd = DependencyPropertyDescriptor.FromProperty(dp, tp);
            var subscription = new Subscription(dpd, d, eh);
            if (!this.subscriptions.Contains(subscription))
            {
                dpd.AddValueChanged(d, eh);
                this.subscriptions.Add(subscription);
            }
        }

        /// <summary>
        /// Unsubscribes a <see cref="Subscription"/>.
        /// </summary>
        /// <param name="subscription"></param>
        public void Unsubscribe(Subscription subscription)
        {
            if (this.subscriptions.Remove(subscription))
            {
                var dpd = subscription.Item1;
                dpd.RemoveValueChanged(subscription.Item2, subscription.Item3);
            }
        }

        /// <summary>
        /// Makes all subscriptions undone.
        /// </summary>
        public void UnsubscribeAll()
        {
            foreach (var subscription in this.subscriptions)
            {
                var dpd = subscription.Item1;
                dpd.RemoveValueChanged(subscription.Item2, subscription.Item3);
            }

            this.subscriptions.Clear();
        }

        /// <summary>
        /// Represents a subscription.
        /// </summary>
        public class Subscription : Tuple<DependencyPropertyDescriptor, DependencyObject, EventHandler>
        {
            /// <summary>
            /// Creates a new <see cref="Subscription"/>.
            /// </summary>
            /// <param name="d">
            /// The target <see cref="DependencyObject"/>.
            /// </param>
            /// <param name="dp">
            /// The respective <see cref="DependencyProperty"/>.
            /// </param>
            /// <param name="tp">
            /// The target type of <see cref="dp"/>.
            /// </param>
            /// <param name="eh">
            /// The respective <see cref="EventHandler"/>.
            /// </param>
            public Subscription(DependencyObject d, DependencyProperty dp, Type tp, EventHandler eh)
              : this(DependencyPropertyDescriptor.FromProperty(dp, tp), d, eh)
            {
            }

            /// <summary>
            /// Creates a new <see cref="Subscription"/>.
            /// </summary>
            /// <param name="d">
            /// The target <see cref="DependencyObject"/>.
            /// </param>
            /// <param name="dpd">
            /// The respective <see cref="DependencyProperty"/>.
            /// </param>
            /// <param name="eh">
            /// The respective <see cref="EventHandler"/>.
            /// </param>
            public Subscription(DependencyPropertyDescriptor dpd, DependencyObject d, EventHandler eh)
              : base(dpd, d, eh)
            {
            }
        }
    }

    public class Subscriber<TTarget> : Subscriber
        where TTarget : DependencyObject
    {
        public new TTarget Target => (TTarget)base.Target;

        public void Attach(TTarget target)
        {
            base.Attach(target);
        }
    }
}
