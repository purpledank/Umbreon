﻿using System;

namespace Espeon {
    public class LockedBinaryHeap<T>  where T : IComparable<T> {
        public T Root {
            get {
                lock (this._heapLock) {
                    return this._binaryHeap.Root;
                }
            }
        }

        public int Size {
            get {
                lock (this._heapLock) {
                    return this._binaryHeap.Size;
                }
            }
        }

        public bool IsEmpty {
            get {
                lock (this._heapLock) {
                    return this._binaryHeap.IsEmpty;
                }
            }
        }

        private readonly object _heapLock;
        private readonly BinaryHeap<T> _binaryHeap;

        private LockedBinaryHeap(BinaryHeap<T> binaryHeap) {
            this._heapLock = new object();
            this._binaryHeap = binaryHeap;
        }

        public static LockedBinaryHeap<T> CreateMinHeap(int initialHeapSize = 16) {
            return new LockedBinaryHeap<T>(BinaryHeap<T>.CreateMinHeap(initialHeapSize));
        }

        public static LockedBinaryHeap<T> CreateMaxHeap(int initialHeapSize = 16) {
            return new LockedBinaryHeap<T>(BinaryHeap<T>.CreateMaxHeap(initialHeapSize));
        }

        public void Insert(T node) {
            lock (this._heapLock) {
                this._binaryHeap.Insert(node);
            }
        }

        public bool TryRemoveRoot(out T root) {
            lock (this._heapLock) {
                return this._binaryHeap.TryRemoveRoot(out root);
            }
        }
    }
}