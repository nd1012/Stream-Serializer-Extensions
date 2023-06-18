﻿using System.Runtime;
using wan24.Core;

namespace wan24.StreamSerializerExtensions
{
    /// <summary>
    /// Thrown on serialization errors
    /// </summary>
    public class SerializerException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SerializerException() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        public SerializerException(string? message) : base(message) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="inner">Inner exception</param>
        public SerializerException(string? message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Wrap an exception in a serializer exception
        /// </summary>
        /// <param name="action">Action</param>
        /// <param name="message">Message</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static void Wrap(Action action, string? message = null)
        {
            try
            {
                action();
            }
            catch (SerializerException)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new SerializerException(message ?? ex.Message, ex);
            }
        }

        /// <summary>
        /// Wrap an exception in a serializer exception
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="func">Function</param>
        /// <param name="message">Message</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static T Wrap<T>(Func<T> func, string? message = null)
        {
            try
            {
                return func();
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message ?? ex.Message, ex);
            }
        }

        /// <summary>
        /// Wrap an exception in a serializer exception
        /// </summary>
        /// <param name="action">Action</param>
        /// <param name="message">Message</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task WrapAsync(Func<Task> action, string? message = null)
        {
            try
            {
                await action().DynamicContext();
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message ?? ex.Message, ex);
            }
        }

        /// <summary>
        /// Wrap an exception in a serializer exception
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="func">Function</param>
        /// <param name="message">Message</param>
        [TargetedPatchingOptOut("Tiny method")]
        public static async Task<T> WrapAsync<T>(Func<Task<T>> func, string? message = null)
        {
            try
            {
                return await func().DynamicContext();
            }
            catch (SerializerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializerException(message ?? ex.Message, ex);
            }
        }
    }
}
