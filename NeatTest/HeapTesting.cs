using NUnit.Framework;
using Neat.Util;
using System.Collections;
using System.Collections.Generic;

namespace NeatTest
{
    class HeapTesting
    {
        [SetUp]
        public void Setup()
        {
        }
        [TearDown]
        public void tearDown()
        {
        }
        [Test]
        public void HeapTest1() {
            int[] what = { 8, 7, 0, 4, 5, 632, 768 };
            MaxHeap<int> heap = new MaxHeap<int>(new IntComparer());
            foreach (int x in what) {
                heap.insert(x);
            }
            int[] result = { 768, 632, 8, 7, 5, 4, 0};
            int w;
            int idx = 0;
            while((w = heap.popTop()) != default) {
                Assert.AreEqual(w, result[idx++]);
            }
            Assert.Pass();
        }
        [Test]
        public void HeapTest2()
        {
            int[] what = { 8, 7, 0, 4, 5, 632, 768 };
            MaxHeap<int> heap = new MaxHeap<int>(new IntComparer());
            foreach (int x in what)
            {
                heap.insert(x);
            }
            int[] result = { 768, 632, 8};
            int w;
            int idx = 0;
            heap.removeKMinElements(4);
            while ((w = heap.popTop()) != default)
            {
                Assert.AreEqual(w, result[idx++]);
            }
            Assert.Pass();
        }
    }
    class IntComparer : IComparer<int>
    {
        int IComparer<int>.Compare(int x, int y)
        {
            if (x == y) {
                return 0;
            } else if (x > y) {
                return 1;
            }
            return -1;
        }
    }
}
