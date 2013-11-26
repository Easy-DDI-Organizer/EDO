using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using EDO.Core.ViewModel;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace EDO.Core.Util
{
    public class ModelSyncher<ViewModelType, ModelType> 
        where ViewModelType: BaseVM 
    {
        private BaseVM parent;
        private ObservableCollection<ViewModelType> viewModelCollection;
        private List<ModelType> modelCollection;

        public ModelSyncher(BaseVM parent, ObservableCollection<ViewModelType> viewModelCollection,  List<ModelType> modelCollection)
        {
            this.parent = parent;
            this.viewModelCollection = viewModelCollection;
            this.modelCollection = modelCollection;
            this.IsStopSync = false;
            viewModelCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(model_CollectionChanged);
        }

        public bool IsStopSync { get; set; }
        public Action<ViewModelType> AddActionHandler { get; set; }
        private UndoManager UndoManager
        {
            get
            {
                return parent.UndoManager;
            }
        }

        private void Memorize()
        {
            if (UndoManager != null)
            {
                UndoManager.Memorize();
            }
        }

        private void RemoveOldItems(NotifyCollectionChangedEventArgs e)
        {
            for (int i = 0; i < e.OldItems.Count; i++)
            {
                modelCollection.RemoveAt(e.OldStartingIndex);
            }
        }

        private void InsertNewItems(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                for (int i = 0; i < e.NewItems.Count; i++)
                {
                    ViewModelType vm = (ViewModelType)e.NewItems[i];
                    vm.Parent = parent;
                    if (AddActionHandler != null)
                    {
                        AddActionHandler(vm);
                    }
                    ModelType model = (ModelType)vm.Model;
                    Debug.Assert(model != null, "model is null!");
                    modelCollection.Insert(e.NewStartingIndex + i, model);
                }
            }
        }

        private void MoveItems(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems.Count == 1)
            {
                ModelType oldModel = modelCollection[e.OldStartingIndex];
                modelCollection.RemoveAt(e.OldStartingIndex);
                modelCollection.Insert(e.NewStartingIndex, oldModel);
            }
            else
            {
                List<ModelType> moveItems = modelCollection.Skip(e.OldStartingIndex).Take(e.OldItems.Count).ToList();
                RemoveOldItems(e);
                for (int i = 0; i < moveItems.Count; i++)
                {
                    modelCollection.Insert(e.NewStartingIndex + i, moveItems[i]);
                }
            }
        }

        private void model_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsStopSync) 
            {
                return;
            }
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    InsertNewItems(e);
                    break;

                case NotifyCollectionChangedAction.Move:
                    MoveItems(e);
                    Memorize();
                    break;

                case NotifyCollectionChangedAction.Remove:
                    RemoveOldItems(e);
                    Memorize();
                    break;

                case NotifyCollectionChangedAction.Replace:
                    RemoveOldItems(e);
                    InsertNewItems(e);
                    Memorize();
                    break;

                case NotifyCollectionChangedAction.Reset:
                    modelCollection.Clear();
                    InsertNewItems(e);
                    Memorize();
                    break;

                default:
                    break;
            }
        }
    }

}
