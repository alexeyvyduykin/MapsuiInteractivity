using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace InteractivityWPFSample.ViewModels
{
    public class RadioButtonItem : ReactiveObject
    {
        public RadioButtonItem(string name)
        {
            Name = name;
        }

        public string Name { get; }

        [Reactive]
        public bool IsSelected { get; set; }
    }

    public class RadioButtonList
    {
        public RadioButtonList()
        {
            Items = new List<RadioButtonItem>();
        }

        public void Register(RadioButtonItem item, Action selected, Action unselected)
        {
            item.WhenAnyValue(s => s.IsSelected).Where(s => s == true).Subscribe(_ => Reset(item));
            item.WhenAnyValue(s => s.IsSelected).Where(s => s == true).Subscribe(_ => selected.Invoke());
            item.WhenAnyValue(s => s.IsSelected).Where(s => s == false).Subscribe(_ => unselected.Invoke());

            Items.Add(item);
        }

        private void Reset(RadioButtonItem excludeItem)
        {
            foreach (var item in Items)
            {
                if (string.Equals(item.Name, excludeItem.Name) == false)
                {
                    item.IsSelected = false;
                }
            }
        }

        public List<RadioButtonItem> Items { get; }
    }
}
