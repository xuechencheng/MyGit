using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameFramework.ObjectPool
{
    public class DoublyLinkedListNode<T> where T : class
    {
        public DoublyLinkedListNode<T> prev = null;
        public DoublyLinkedListNode<T> next = null;
        public T t = null;
        public virtual void Clear() {
            prev = null;
            next = null;
            t = null;
        }
    }

    public class DoublyLinkedList<T> where T : class {
        public DoublyLinkedListNode<T> head = null;
        public DoublyLinkedListNode<T> tail = null;
        protected int mCount = 0;
        public int Count {
            get {
                return mCount;
            }
        }
        public DoublyLinkedListNode<T> AddToHeader(T pNode)
        {
            return null;
        }

        public DoublyLinkedListNode<T> AddToHeader(DoublyLinkedListNode<T> pNode) {
            return null;
        }
        public DoublyLinkedListNode<T> AddToTail(T pNode)
        {
            return null;
        }
        public DoublyLinkedListNode<T> AddToTail(DoublyLinkedListNode<T> pNode)
        {
            return null;
        }

        public void MoveToHead(DoublyLinkedListNode<T> pNode) { 
        }

        public void RemoveNode(DoublyLinkedListNode<T> pNode) { 
        
        }
    }
}