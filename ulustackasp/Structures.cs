using System;

namespace ulustackasp
{
    public class Structures
    {
        public class Node<T>
        {
            public T Data;
            public Node<T> Next;

            public Node(T data)
            {
                Data = data;
                Next = null;
            }
        }
        public class Queue<T> //fifo
        {
            private Node<T> head;
            private Node<T> tail;
            private int count;

            public Queue()
            {
                head = null;
                tail = null;
                count = 0;
            }

            public void Push(T item)
            {
                Node<T> newNode = new Node<T>(item);
                if (tail == null)
                {
                    head = newNode;
                    tail = newNode;
                }
                else
                {
                    tail.Next = newNode;
                    tail = newNode;
                }

                count++;
            }

            public T Pop()
            {
                if (IsEmpty())
                {
                    throw new InvalidOperationException("");
                }

                T value = head.Data;
                head = head.Next;

                if (head == null)
                {
                    tail = null;
                }

                count--;
                return value;
            }

            public T Peek()
            {
                if (IsEmpty())
                {
                    throw new InvalidOperationException("");
                }

                return head.Data;
            }
            public bool IsEmpty()
            {
                return head == null;
            }

            public int Count()
            {
                return count;
            }
        }

        public class Stack<T>
        {
            private Node<T> top; 
            private int count;  

            public Stack()
            {
                top = null;
                count = 0;
            }

            public void Push(T item)
            {
                Node<T> newNode = new Node<T>(item);

                newNode.Next = top;
                top = newNode;

                count++; 
            }

            public T Pop()
            {
                if (IsEmpty())
                {
                    throw new InvalidOperationException("");
                }

                T value = top.Data;
                top = top.Next; 

                count--; 
                return value;
            }

            
            public T Peek()
            {
                if (IsEmpty())
                {
                    throw new InvalidOperationException("");
                }

                return top.Data;
            }

            public bool IsEmpty()
            {
                return top == null; 
            }

            public int Count()
            {
                return count; 
            }
        }
    }
}
//intihar etmeme az kaldı