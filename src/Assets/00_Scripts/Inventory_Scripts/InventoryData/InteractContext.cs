
using HBDinosaur_ER_Project.ItemData;

namespace HBDinosaur_ER_Project.InventoryData
{
    public class InteractContext
    {
        // A 003 상호작용 가능한가? 상호작용하면 무슨 일이 일어나는가? 판단의 자료로, item, 이동 전 슬롯 인덱스, 이동 후 슬롯 인덱스, 상호작용 타입 등을 알게 됨.
        // 상호작용 타입은 아이템이 어떤 상태인지를 구분하는 FSM에 가까움.
        public Item item;
        public int fromSlotIndex;
        public int toSlotIndex;
        public InventoryInteractType interactType;


        // A 004 생성자로 매개변수와 필드변수의 이름이 같으므로, this를 사용하여 구분.
        public InteractContext(Item item, int fromSlotIndex, int toSlotIndex, InventoryInteractType interactType)
        {
            this.item = item;
            this.fromSlotIndex = fromSlotIndex;
            this.toSlotIndex = toSlotIndex;
            this.interactType = interactType;
        }
    }
}