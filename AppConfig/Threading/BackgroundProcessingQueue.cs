using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AppConfig.Threading
{
    /// <summary>
    /// Creates a First in First out list of items to be processed in a background thread pool of a defined size.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BackgroundProcessingQueue<T>
    {
        private Queue<T> itemsToProcess = new Queue<T>();
        private ItemProcessingMethodDelegate itemProcessingMethod;
        private Thread[] availableTheads;

        public delegate void ItemProcessingMethodDelegate(T item);

        /// <summary>
        /// Creates a new background processing queue.
        /// </summary>
        /// <param name="ItemProcessingMethod">A delegate pointing to a procedure that will process each item.</param>
        /// <param name="MaximumProcessingThreads">The maximum number of simulations processes running in background threads.  Additional items will wait in the queue for processing.</param>
        public BackgroundProcessingQueue(ItemProcessingMethodDelegate ItemProcessingMethod, int MaximumProcessingThreads)
        {
            if (MaximumProcessingThreads <= 0)
                throw new ArgumentException("A minimum of one processing thread is required.  The parameter 'MaximumProcessingThreads' must be greater than zero.");

            this.itemProcessingMethod = ItemProcessingMethod;
            this.MaximumProcessingTheads = MaximumProcessingThreads;
            this.availableTheads = new Thread[MaximumProcessingThreads];
        }

        /// <summary>
        /// The maximum number of simulations processes running in background threads.  Additional items will wait in the queue for processing.
        /// </summary>
        public int MaximumProcessingTheads { get; private set; }

        /// <summary>
        /// Add an item to be processed in the background.  The item will take the next available thread or wait until a one is free.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            itemsToProcess.Enqueue(item);
            StartProcessing();
        }

        /// <summary>
        /// Add a range of items to be processed in the background.  Each item will take an available thread or wait until one is free.
        /// </summary>
        /// <param name="items"></param>
        public void Add(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                itemsToProcess.Enqueue(item);
                StartProcessing();
            }
        }

        /// <summary>
        /// Start Async Background Processing
        /// </summary>
        private void StartProcessing()
        {
            int position = -1;

            //Find the next available position to put a new thread
            for(int i=0; i < MaximumProcessingTheads; i++)
                if (availableTheads[i] == null || !availableTheads[i].IsAlive)
                {
                    position = i;
                    break;
                }

            //If all the positions are full then return and let one
            //of the existing threads process this item.
            if (position == -1)
                return;

            //Create the new thread to process this and possibly other new items.
            availableTheads[position] = new Thread(ProcessItems);
            availableTheads[position].IsBackground = true;
            availableTheads[position].Start();
        }

        /// <summary>
        /// Runs in the background to process available items in the queue.
        /// </summary>
        private void ProcessItems()
        {
            //Loop until the code decides to quit
            while (true)
            {
                T nextDataItem;
                //Check for an item in the queue. remove it if it exists and return if all items are completed.
                lock (itemsToProcess)
                {
                    if (itemsToProcess.Count == 0)
                        return;
                    nextDataItem = itemsToProcess.Dequeue();
                }
                //Call the delegate specified to process the item.
                itemProcessingMethod.Invoke(nextDataItem);
            }
        }

    }
}
