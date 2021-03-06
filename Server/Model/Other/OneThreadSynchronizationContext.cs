﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace Model
{
	public class OneThreadSynchronizationContext : SynchronizationContext
	{
		// 线程同步队列,发送接收socket回调都放到该队列,由poll线程统一执行
		private Queue<Action> queue = new Queue<Action>();

		private Queue<Action> localQueue = new Queue<Action>();

		private readonly object lockObject = new object();

		private void Add(Action action)
		{
			lock (lockObject)
			{
				this.queue.Enqueue(action);
			}
		}

		public void Update()
		{
			lock (lockObject)
			{
				localQueue = queue;
				queue = new Queue<Action>();
			}

			while (this.localQueue.Count > 0)
			{
				Action a = this.localQueue.Dequeue();
				a();
			}
		}

		public override void Post(SendOrPostCallback callback, object state)
		{
			this.Add(() => { callback(state); });
		}
	}
}
