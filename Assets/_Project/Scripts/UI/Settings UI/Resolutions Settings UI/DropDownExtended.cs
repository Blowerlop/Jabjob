using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Project
{
    public class DropDownExtended : TMP_Dropdown
    {
        public Event onBeforeDropdownShowEvent = new Event(nameof(onBeforeDropdownShowEvent));
        public Event onAfterDropdownShowEvent = new Event(nameof(onAfterDropdownShowEvent));
        public Event<RectTransform, int> onDropdownItemCreatedEvent = new Event<RectTransform, int>(nameof(onDropdownItemCreatedEvent));
        public int itemCreationIndex { get; private set; } = 0;
        public List<RectTransform> dropdownItems = new List<RectTransform>();
        
        
        public override void OnPointerClick(PointerEventData eventData)
        {
            itemCreationIndex = 0;
            dropdownItems = new List<RectTransform>();
            onBeforeDropdownShowEvent.Invoke(this);
            base.OnPointerClick(eventData);
            onAfterDropdownShowEvent.Invoke(this);
        }

        public override void OnCancel(BaseEventData eventData)
        {
            base.OnCancel(eventData);
            dropdownItems = null;
            itemCreationIndex = 0;
        }

        protected override DropdownItem CreateItem(DropdownItem itemTemplate)
        {
            // return base.CreateItem(itemTemplate);
            //
            DropdownItem dropdownItem = (DropdownItem)Instantiate(itemTemplate);
            dropdownItems.Add(dropdownItem.rectTransform);
            onDropdownItemCreatedEvent.Invoke(this, false, dropdownItem.rectTransform, itemCreationIndex);
            itemCreationIndex++;
            return dropdownItem;
        }
        
    }
}
