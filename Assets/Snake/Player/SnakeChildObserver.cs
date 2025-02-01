using Snake.Unit;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Unity.Cinemachine;
using UnityEngine;

namespace Snake.Player
{
    public class SnakeChildObserver : CustomMonoBehaviour
    {
        public CinemachineTargetGroup cinemachineTargetGroup;
        private ObservableCollection<IUnit> observableCollection = new ObservableCollection<IUnit>();

        public ObservableCollection<IUnit> ObservableCollection
        {
            get => observableCollection;
            set
            {
                if (observableCollection != null && observableCollection != value)
                {
                    foreach (IUnit item in observableCollection)
                        cinemachineTargetGroup.RemoveMember(item.GameObject.transform);
                    observableCollection.CollectionChanged -= ChildHero_CollectionChanged;
                }

                observableCollection = value;
                if (observableCollection != null)
                {
                    foreach (IUnit item in observableCollection)
                        cinemachineTargetGroup.AddMember(item.GameObject.transform, 10, 1);
                    observableCollection.CollectionChanged += ChildHero_CollectionChanged;
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnDestroy()
        {
            if (ObservableCollection != null)
                ObservableCollection.CollectionChanged -= ChildHero_CollectionChanged;
            base.OnDestroy();
        }

        public void Init(ObservableCollection<IUnit> observableCollection)
        {
            ObservableCollection = observableCollection;
        }

        private void ChildHero_CollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            try
            {
                IList oldItems = eventArgs.OldItems;
                IList newItems = eventArgs.NewItems;
                switch (eventArgs.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (newItems != null)
                        {
                            foreach (IUnit item in newItems.Cast<IUnit>())
                                cinemachineTargetGroup.AddMember(item.GameObject.transform, 10, 1);
                        }
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        if (oldItems != null)
                        {
                            foreach (IUnit item in oldItems.Cast<IUnit>())
                                cinemachineTargetGroup.RemoveMember(item.GameObject.transform);
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        break;
                    default:
                        throw new System.NotImplementedException(eventArgs.Action.ToString());
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                eventArgs.PrintDebug(sender);
            }
        }
    }

    public static class NotifyCollectionChangedEventArgsExtension
    {
        public static void PrintDebug(this NotifyCollectionChangedEventArgs eventArgs, object sender)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"ChildHero_CollectionChanged {eventArgs.Action} {sender}");
            stringBuilder.AppendLine("NewItems");
            stringBuilder.AppendLine($"NewStartingIndex {eventArgs.NewStartingIndex}");
            stringBuilder.AppendLine($"OldStartingIndex {eventArgs.OldStartingIndex}");
            if (eventArgs.NewItems != null)
            {
                foreach (object item in eventArgs.NewItems)
                    stringBuilder.AppendLine(item.ToString());
            }

            stringBuilder.AppendLine("OldItems");
            if (eventArgs.OldItems != null)
            {
                foreach (object item in eventArgs.OldItems)
                    stringBuilder.AppendLine(item.ToString());
            }

            Debug.Log(stringBuilder.ToString());
        }
    }
}