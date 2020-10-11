using Neat.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat.Util
{
    public class MaxHeap<T>
    {
        ArrayList _elements;
        IComparer<T> comparator;

        public MaxHeap(IComparer<T> comparable) {
            _elements = new ArrayList();
            this.comparator = comparable;
        }
        public void insert(T element) {
            _elements.Add(element);
            heapifyBottomUp(_elements.Count - 1);
        }
        private void heapifyBottomUp(int idx) {
            int parent = (idx - 1) / 2;
            while (idx > 0 && comparator.Compare((T)_elements[idx], (T)_elements[parent]) > 0) {
                T tmp = (T)_elements[idx];
                _elements[idx] = _elements[parent];
                _elements[parent] = tmp;
                idx = parent;
                parent = idx / 2;
            }
        }

        private void heapifyTopDown(int idx) {
            int child1 = idx * 2 + 1;
            int child2 = idx * 2 + 2;
            int replace = idx;
            if (child1 < _elements.Count 
                && comparator.Compare((T)_elements[child1], (T)_elements[replace]) > 0) {
                replace = child1;
            }
            if (child2 < _elements.Count
                && comparator.Compare((T)_elements[child2], (T)_elements[replace]) > 0)
            {
                replace = child2;

            }
            if (replace != idx) {
                T tmp = (T)_elements[replace];
                _elements[replace] = _elements[idx];
                _elements[idx] = tmp;
                heapifyTopDown(replace);
            }
        }
        public T popTop() {
            if (_elements.Count > 0)
            {
                int idx = _elements.Count - 1;
                T tmp = (T)_elements[0];
                _elements[0] = _elements[idx];
                _elements[idx] = tmp;
                _elements.RemoveAt(idx);
                heapifyTopDown(0);
                return tmp;
            }
            return default;
        }

        public T getTop() {
            return (T)_elements[0];
        }

        public void removeKMinElements(int k) {
            int elementsToRemain = _elements.Count - k;
            if (elementsToRemain < 0) {
                // no shit, I can't remove more than what's there is in the heap
                return;
            }
            List<T> remainingElements = new List<T>();
            for(int count = 0; count < elementsToRemain; count++) {
                remainingElements.Add(popTop());
            }
            _elements.Clear();
            foreach(T element in remainingElements) {
                insert(element);
            }
        }
        public String toString() {
            StringBuilder sb = new StringBuilder();
            foreach(T element in _elements) {
                sb.Append(element.ToString());
                sb.Append("\n");
            }
            return sb.ToString();
        }
    }
}
