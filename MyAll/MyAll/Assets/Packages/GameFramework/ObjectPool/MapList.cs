using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameFramework.ObjectPool {
    public class DoublyLinkedListNode<T>  where T : class { 
        
    }

    public class DoublyLinkedList<T> where T : class {

        public DoublyLinkedListNode<T> AddToHeader(T t) {
            return null;
        }

        public DoublyLinkedListNode<T> AddToHeader(DoublyLinkedListNode<T> pNode)
        {
            return null;
        }

        public DoublyLinkedListNode<T> AddToTail(T t)
        {
            return null;
        }

        public DoublyLinkedListNode<T> AddToTail(DoublyLinkedListNode<T> pNode)
        {
            return null;
        }

        public void MoveToTail(DoublyLinkedListNode<T> pNode)
        {
        }

        public void RemoveNode(DoublyLinkedListNode<T> pNode) { 
        }

        public void Clear() { 
        
        }
    }
}
