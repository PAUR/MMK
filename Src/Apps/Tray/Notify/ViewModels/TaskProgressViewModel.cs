﻿using System;
using System.Diagnostics.Contracts;
using MMK.ApplicationServiceModel;
using MMK.Notify.Observer;
using MMK.Notify.Observer.Tasking.Observing;
using MMK.Notify.Services;
using MMK.Wpf.ViewModel;

namespace MMK.Notify.ViewModels
{
    public class TaskProgressViewModel : ViewModel
    {
        private static NotificationService Notification
        {
            get { return IoC.Get<NotificationService>(); }
        }

        private INotifyable currentInfo;

        private bool isVisible;
        private bool isProgress;
        private int observedCount;
        private int queuedCount;
        private int failedCount;

        public INotifyable CurrentInfo
        {
            get { return currentInfo; }
            set
            {
                currentInfo = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if(value == isVisible)
                    return;
                isVisible = value;
                NotifyPropertyChanged();
            }
        }


        public bool IsProgress
        {
            get { return isProgress; }
            private set
            {
                if(value == isProgress)
                    return;
                
                isProgress = value;

                IsVisible = IsProgress;

                NotifyPropertyChanged();
            }
        }

        public int ObservedCount
        {
            get { return observedCount; }
            set
            {
                if (value < 0)
                    throw new ArgumentException(@"must be >= 0", "value");
                Contract.EndContractBlock();

                if (value == observedCount)
                    return;
                observedCount = value;
                NotifyPropertyChanged();
            }
        }

        public int QueuedCount
        {
            get { return queuedCount; }
            set
            {
                if (value < 0)
                    throw new ArgumentException(@"must be >= 0", "value");
                if (value < ObservedCount)
                    throw new ArgumentException(@"must be < ObservedCount", "value");
                Contract.EndContractBlock();

                if (value == queuedCount)
                    return;

                queuedCount = value;
                IsProgress = queuedCount != 0;
                NotifyPropertyChanged();
            }
        }

        protected override void OnLoadData()
        {
            var observer = IoC.Get<TaskObserver>();
            observer.TaskQueued += OnTaskQueued;
            observer.QueueEmpty += OnQueueEmpty;
            observer.TaskObserved += OnTaskObserved;
            observer.TaskFailed += OnTaskFailed;
        }

        protected override void OnUnloadData()
        {
            var observer = IoC.Get<TaskObserver>();
            observer.TaskQueued -= OnTaskQueued;
            observer.QueueEmpty -= OnQueueEmpty;
            observer.TaskObserved -= OnTaskObserved;
            observer.TaskFailed -= OnTaskFailed;
        }

        public void OnTaskQueued(object sender, TaskObserver.TaskQueuedEventArgs e)
        {
            QueuedCount += e.TaskCount;
        }

        public void OnTaskObserved(object sender, TaskObserver.NotifyEventArgs e)
        {
            ++ObservedCount;
            CurrentInfo = e.Message;
        }

        private void OnTaskFailed(object sender, TaskObserver.NotifyEventArgs e)
        {
            ++failedCount;
            
            if(failedCount == QueuedCount)
                return;
            
            Notification.Push(e.Message);
        }

        public void OnQueueEmpty(object sender, EventArgs e)
        {
            Notify();
            Reset();
        }

        private void Notify()
        {
            if (QueuedCount == 0)
                return;

            var message = BuildNotifyMessage();

            Notification.Push(message);
        }

        private void Reset()
        {
            ObservedCount = 0;
            QueuedCount = 0;
            failedCount = 0;
            IsVisible = false;
            currentInfo = null;
        }

        private INotifyable BuildNotifyMessage()
        {
            if (QueuedCount == 1)
                return CurrentInfo;

            if(failedCount == 0)
                return new NotifyMessage
                {
                    Type = NotifyType.Success,
                    CommonDescription = "All tasks done.",
                    DetailedDescription = String.Format("{0} Tasks", QueuedCount)
                };

            if (failedCount == ObservedCount)
                return new NotifyMessage
                {
                    Type = NotifyType.Error,
                    CommonDescription = "All tasks failed.",
                    DetailedDescription = String.Format("{0}/{1} Tasks",failedCount, QueuedCount)
                };

            return new NotifyMessage
            {
                Type = NotifyType.Warning,
                CommonDescription = "Some tasks failed.",
                DetailedDescription = String.Format("Done {1}/{0};\nFailed {2}", QueuedCount, ObservedCount - failedCount, failedCount )
            };
        }
    }
}