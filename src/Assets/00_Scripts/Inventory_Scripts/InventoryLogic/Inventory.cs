using HBDinosaur_ER_Project.InventoryData;
using HBDinosaur_ER_Project.ItemData;
using UnityEngine;
namespace HBDinosaur_ER_Project.InventoryLogic
{
    public class Inventory : IInteractable
    {
        // A 008 아이템을 배열로 소지하고 있는다.
        public Item[] items { get; private set; }

        // A 009 아이템 배열을 인벤토리 size 크기로 생성자에서 생성.
        public Inventory(int size)
        {
            items = new Item[size];
        }

        // A 010 아이템 추가 매서드 (item을 받아서 채워졌는지 못 채웠는지를 반환)
        public bool AddItem(Item item)
        {
            for (int i = 0; i < items.Length; i++)
            {
                // A 011 낮은 인덱스부터 null 아닌 경우에 아이템을 채워넣는다.
                if (items[i] == null)
                {
                    items[i] = item;
                    // A 012 추가된 아이템과, 추가된 인벤토리 인덱스를 갱신되어야 하는 클래스에 전달. PlayerInventoryHud 등.
                    InventoryEventBus.Instance.PublishItemAdded(item, i);
                    // A 013 아이템 추가가 되었으면 추가 성공 반환
                    return true;
                }
            }
            // A 014 아이템 추가가 되지 않았으면 추가 실패 반환
            return false;
        }

        // A 015 인덱스가 0이상이고, 인벤토리 크기 이하인 인덱스인지 판단하는 매서드
        private bool IsValidIndex(int index) => index >= 0 && index < items.Length;


        // A 016 아이템 제거 매서드 
        public bool RemoveItem(int slotIndex)
        {
            // A 017 유효한지? 또는 비어있는지? 그러면 제거 실패 반환
            if (!IsValidIndex(slotIndex) || items[slotIndex] == null) return false;


            var removed = items[slotIndex];
            items[slotIndex] = null;
            InventoryEventBus.Instance.PublishItemRemoved(removed, slotIndex);
            return true;
        }

        public Item GetItem(int slotIndex)
        {
            if (!IsValidIndex(slotIndex)) return null;
            return items[slotIndex];
        }

        // 두 슬롯의 아이템을 교환 (한쪽이 비어 있어도 동작)
        public void SwapItems(int fromIndex, int toIndex)
        {
            if (!IsValidIndex(fromIndex) || !IsValidIndex(toIndex)) return;

            var temp = items[fromIndex];
            items[fromIndex] = items[toIndex];
            items[toIndex] = temp;

            InventoryEventBus.Instance.PublishSlotChanged(this, fromIndex);
            InventoryEventBus.Instance.PublishSlotChanged(this, toIndex);
        }

        // fromIndex 슬롯의 아이템을 제거하고 toIndex에 넣음 (다른 인벤토리로 이동할 때)
        public Item TakeItem(int slotIndex)
        {
            if (!IsValidIndex(slotIndex) || items[slotIndex] == null) return null;

            var item = items[slotIndex];
            items[slotIndex] = null;
            InventoryEventBus.Instance.PublishSlotChanged(this, slotIndex);
            return item;
        }

        public bool CanInteract(InteractContext context)
        {
            return context.interactType switch
            {
                InventoryInteractType.TAKEITEM => IsValidIndex(context.fromSlotIndex) && items[context.fromSlotIndex] != null,
                InventoryInteractType.DROPITEM => IsValidIndex(context.fromSlotIndex) && items[context.fromSlotIndex] != null,
                InventoryInteractType.SWAPITEM => IsValidIndex(context.fromSlotIndex) && IsValidIndex(context.toSlotIndex),
                InventoryInteractType.EQUIPITEM => IsValidIndex(context.fromSlotIndex) && items[context.fromSlotIndex] != null,
                InventoryInteractType.UNEQUIPITEM => true,
                _ => false
            };
        }

        public void Interact(InteractContext context)
        {
            if (!CanInteract(context)) return;

            switch (context.interactType)
            {
                case InventoryInteractType.DROPITEM:
                    RemoveItem(context.fromSlotIndex);
                    break;
                case InventoryInteractType.SWAPITEM:
                    SwapItems(context.fromSlotIndex, context.toSlotIndex);
                    break;
                case InventoryInteractType.UNEQUIPITEM:
                    AddItem(context.item);
                    break;
            }

            InventoryEventBus.Instance.PublishInteracted(context.interactType);
        }


    }

}
